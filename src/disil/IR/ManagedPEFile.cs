//===================================================================
//Copyright (C) 2011 Scott Wisniewski (scott@scottdw2.com)
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//of the Software, and to permit persons to whom the Software is furnished to do
//so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//===================================================================

using System;
using System.IO;

using disil.Collections;
using disil.IO;

namespace disil.IR
{
    internal class ManagedPEFile
    {
        private const string s_tablesStreamName = "#~";
        private const string s_stringHeapStreamName = "#Strings";
        private const string s_guidHeapStreamName = "#GUID";

        private static readonly byte[] s_peSignature = { (byte)'P', (byte)'E', 0, 0 };
        
        private PEHeader m_header;
        private RegionMap<SectionHeader> m_sections;
        private ClrHeader m_clrHeader;
        private ClrMetadataRoot m_metadataRoot;
		private TablesHeader m_tablesHeader;
        private byte[] m_stringHeap;
        private Guid[] m_guids;
        private Module m_module;

        private void Reset()
        {
            m_header = null;
            m_sections = null;
            m_clrHeader = null;
        }

        public void Load(Stream s)
        {
            Reset();
            s.AssumeArgNotNull("s");

            try
            {
                VerifyPESignature(s);
                ParsePEHeader(s);
                ParseSectionMap(s);
                ParseClrHeader(s);
                ParseMetadaRoot(s);
                ParseStringHeap(s);
                ParseGuidHeap(s);
                ParseMetaDataTables(s);
            }
            catch
            {
                Reset();
                throw;
            }
        }

        private void VerifyPESignature(Stream s)
        {
            s.AssumeNotNull();

            var signatureOffset = ReadSignatureOffset(s);
            s.Seek(signatureOffset);
            s.ReadBytes(4).AssumeEquals(
                s_peSignature,
                "Couldn't verify PE signature"
            );
        }

        private static uint ReadSignatureOffset(Stream s)
        {
            const int sigOffset = 0x3c;
            s.Seek(sigOffset);
            return s.ReadUIntLE();
        }

        private void ParsePEHeader(Stream s)
        {
            s.AssumeNotNull();
            m_header = new PEHeader();
            m_header.Load(s);
        }

        private void ParseSectionMap(Stream s)
        {
            //NOTE: We loose the ordering sections were specified in the file 
            //here. That's OK for now, because we have a read-only structure.
            //If we ever need to use this code for round-tripping PE files,
            //then this needs to be modified to also track section order.

            s.AssumeNotNull();
            m_header.AssumeNotNull();

            m_sections = 
                new RegionMap<SectionHeader>(
                    x => x.SectionRVA,
                    x => x.VirtualSize
                );
            
            for (int i = 0; i < m_header.NumberOfSections; ++i)
            {
                var section = new SectionHeader();
                section.Load(s);
                m_sections.Add(section);
            }
        }
        
        private void ParseClrHeader(Stream s)
        {
            s.AssumeNotNull();
            m_sections.AssumeNotNull();
            m_header.AssumeNotNull();

            var sectionIndex = (int) DataDirectoryName.CLRRuntimeHeader;

            if (
                m_header.DataDirectories != null  
                && sectionIndex < m_header.DataDirectories.Count 
                && sectionIndex >=0
            )
            {
                var dd = m_header.DataDirectories[sectionIndex];

                //find the section containing the RVA of the data directory.
                //For C# code this will almost always end up being the 
                //".rdata" section, but the CLR Ecma spec does not require 
                //this, and we are likely to find our selfs dissasembling
                //mixed-mode assemblies at some point in time anyways
                //(which can do arbitrarily weird things with executable 
                //layout). This also makes us somewhat future proof.
                var section = m_sections[dd.RVA];

                if (section == null || dd.Size <= 0 || dd.RVA <= 0)
                {
                    return;
                }

                if (!section.IsInitializedData(dd))
                {
                    //If we reach here, it means that the CIL header is (at 
                    //least partially) located in uinitialized data, or crosses
                    //section boundaries. Neither is likely to happen in 
                    //practice. The Ecma-335 spec (4th edition) 
                    //says that the CIL header should be in a "read only, 
                    //shared section of the image".
                    //
                    //If we see something like this, chances are we 
                    //screwed up somewhere along the way during parsing, or 
                    //the image we are reading is bad. However, it is also 
                    //possible that someone is trying to do something clever, 
                    //by initializing CLR meta-data in native code before 
                    //invoking the CLR's native entry point (_CorExeMain /
                    // _CorDllMain). In either case, this means we can't
                    //dissasemble this managed executable (because we don't
                    //know what the meta-data is), so we throw an exception.
                    //
                    //It is technically possible to handle the cross-section
                    //case, if the initialized data from the first section
                    //terminates neatly on a page boundary (or the presence
                    //of zero padding still yields a valid header), and the
                    //"second" section contains initialized data. However,
                    //such cases are not likely to be produced by a real 
                    //compiler, and likely come from either a bad image, 
                    //from a hand crafted edge case, or from a user with 
                    //malicious intent. In any case, it's best to throw here.
                    throw new InternalErrorException(
                        "Refusing to read non standard clr header."
                    );
                }

                s.Seek(
                    section.PointerToRawData 
                    + (dd.RVA - section.SectionRVA).AssumeGte(0)
                );

                m_clrHeader = new ClrHeader();
                m_clrHeader.Load(s);
            }
        }

        private void ParseMetadaRoot(Stream s)
        {
            s.AssumeNotNull();
            var rootPointer =
                m_clrHeader.AssumeNotNull("Missing CLR Header")
                .MetaDataPointer.AssumeNotNull("Missing meta-data Root");

            rootPointer.RVA.AssumeGt(0, "Missing meta-data root");
            rootPointer.Size.AssumeGt(0, "Missing meta-data root");

            SectionHeader section;
            
            m_sections.TryGetValue(
                rootPointer.RVA, 
                out section
            ).AssumeTrue("Invalid meta-data root.");

            section.AssumeNotNull();
            section.IsInitializedData(rootPointer).AssumeTrue("Invalid meta-data root.");

            s.Seek(
                section.PointerToRawData + 
                (rootPointer.RVA - section.SectionRVA).AssumeGte(0)
            );

            m_metadataRoot = new ClrMetadataRoot();
            m_metadataRoot.Load(s, rootPointer.Size);
        }

        private void ParseStringHeap(Stream s)
        {
            var subStream = 
                GetSubStream(
                    s,
                    s_stringHeapStreamName,
                    "Error reading string heap"
                ).AssumeNotNull("Missing sting heap");

            using (subStream)
            {
                m_stringHeap = new byte[subStream.Length];
                subStream.ReadBytes(m_stringHeap.Length);
            }
        }


        private void ParseGuidHeap(Stream s)
        {
            var subStream =
                GetSubStream(
                    s,
                    s_guidHeapStreamName,
                    "Error reading guid heap"                    
                );

            using (subStream)
            {
                const int guidSize = 16;

                Util.Assume(subStream.Length % guidSize == 0, "Invalud GUID heap size");

                m_guids = new Guid[subStream.Length / guidSize];

                for (uint i = 0; i < m_guids.Length; ++i)
                {
                    m_guids[i] = new Guid(s.ReadBytes(guidSize));
                }
            }
        }

        private void ParseMetaDataTables(Stream s)
        {
            var subStream = 
                GetSubStream(
                    s, 
                    s_tablesStreamName, 
                    "Error reading meta-data tables"
                ).AssumeNotNull("Missing meta-data tables");

            using (subStream)
            {
                m_tablesHeader = new TablesHeader();
                m_tablesHeader.Load(subStream);
				
				int tableNumber = 0;
				
				for (int i = 0; i <= (int)TableID.MAX_TABLE_ID; ++i)
				{
					if ((m_tablesHeader.ValidTables & (1UL << i)) != 0)
					{
						ParseTable(subStream, m_tablesHeader.RowCounts[tableNumber], (TableID)i);
						++tableNumber;
					}
				}
            }
		}

        private SubStream GetSubStream(Stream s, string streamName, string errorMessage)
        {
            s.AssumeNotNull();
            m_metadataRoot.AssumeNotNull();
            m_clrHeader.AssumeNotNull();
            m_clrHeader.MetaDataPointer.AssumeNotNull();
            streamName.AssumeNotNull();
            errorMessage.AssumeNotNull();

            var streamHeader = m_metadataRoot.StreamHeaders[streamName];

            if (streamHeader != null && streamHeader.Size > 0)
            {
                var streamRVA = 
                    m_clrHeader.MetaDataPointer.RVA 
                    + streamHeader.Offset;

                var section = m_sections[streamRVA];

                section.IsInitializedData(streamRVA, streamHeader.Size)
                    .AssumeTrue(
                        "Invalid metadata stream"
                    );

                s.Seek(
                    section.PointerToRawData
                    + (streamRVA - section.SectionRVA).AssumeGte(0)
                );

                return new SubStream(s, streamHeader.Size, errorMessage);
            }

            return null;
        }

        public void ParseTable(Stream s, uint rowCount, TableID id)
		{
		    s.AssumeNotNull();
            switch (id)
            {
                case TableID.Module:
                    ParseModuleTable(s, rowCount);
                    break;
                case TableID.TypeRef:
                    ParseTypeRefTable(s, rowCount);
                    break;
                case TableID.TypeDef:
                    ParseTypeDefTable(s, rowCount);
                    break;
                case TableID.Field:
                    ParseFieldTable(s, rowCount);
                    break;
                case TableID.MethodDef:
                    ParseMethodDefTable(s, rowCount);
                    break;
                case TableID.Param:
                    ParseParamTable(s, rowCount);
                    break;
                case TableID.InterfaceImpl:
                    ParseInterfaceImplTable(s, rowCount);
                    break;
                case TableID.MemberRef:
                    ParseMemberRefTable(s, rowCount);
                    break;
                case TableID.CustomAttribute:
                    ParseCustomAttributeTable(s, rowCount);
                    break;
                case TableID.FieldMarshal:
                    ParseFieldMarshalTable(s, rowCount);
                    break;
                case TableID.DeclSecurty:
                    ParseDeclSecurityTable(s, rowCount);
                    break;
                case TableID.ClassLayout:
                    ParseClassLayoutTable(s, rowCount);
                    break;
                case TableID.FieldLayout:
                    ParseFieldLayoutTable(s, rowCount);
                    break;
                case TableID.StandAloneSig:
                    ParseStandAloneSigTable(s, rowCount);
                    break;
                case TableID.EventMap:
                    ParseEventMapTable(s, rowCount);
                    break;
                case TableID.Event:
                    ParseEventTable(s, rowCount);
                    break;
                case TableID.PropertyMap:
                    ParsePropertyMapTable(s, rowCount);
                    break;
                case TableID.Property:
                    ParsePRopertyTable(s, rowCount);
                    break;
                case TableID.MethodSemantics:
                    ParseMethodSemanticsTable(s, rowCount);
                    break;
                case TableID.MethodImp:
                    ParseMethodImplTable(s, rowCount);
                    break;
                case TableID.ModuleRef:
                    ParseModuleRefTable(s, rowCount);
                    break;
                case TableID.TypeSpec:
                    ParseTypeSpecTable(s, rowCount);
                    break;
                case TableID.ImplMap:
                    ParseImplMapTabble(s, rowCount);
                    break;
                case TableID.FieldRVA:
                    ParseFieldRVATable(s, rowCount);
                    break;
                case TableID.Assembly:
                    ParseAssemblyTable(s, rowCount);
                    break;
                case TableID.AssemblyProcessor:
                    ParseAssemblyProcessorTable(s, rowCount);
                    break;
                case TableID.AssemblyOS:
                    ParseAssemblyOSTable(s, rowCount);
                    break;
                case TableID.AssemblyRef:
                    ParseAssemblyRefTable(s, rowCount);
                    break;
                case TableID.AssemblyRefProcessor:
                    ParseAssemblyRefProcessorTable(s, rowCount);
                    break;
                case TableID.AssemblyRefOS:
                    ParseAssemblyRefOsTable(s, rowCount);
                    break;
                case TableID.File:
                    ParseAssemblyFileTable(s, rowCount);
                    break;
                case TableID.ExportedType:
                    ParseExportedTypeTable(s, rowCount);
                    break;
                case TableID.ManifestResource:
                    ParseMainfestResourceTable(s, rowCount);
                    break;
                case TableID.NestedClass:
                    ParseNestedClassTable(s, rowCount);
                    break;
                case TableID.GenericParam:
                    ParseGenericParamTable(s, rowCount);
                    break;
                case TableID.MethodSpec:
                    ParseMethodSpecTable(s, rowCount);
                    break;
                case TableID.GenericParamConstraint:
                    ParseGenericParamConstraintTable(s, rowCount);
                    break;
                default:
                    throw new InternalErrorException("Unknown table id");
            }
		}

        private void ParseModuleTable(Stream s, uint rowCount)
        {
            rowCount.AssumeEquals(1U, "Invalid module table.");
            m_module = new Module();
            m_module.Load(s);
        }

        private void ParseTypeRefTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseTypeDefTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseFieldTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseMethodDefTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseParamTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseInterfaceImplTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseMemberRefTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseCustomAttributeTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseFieldMarshalTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseDeclSecurityTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseClassLayoutTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseFieldLayoutTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseStandAloneSigTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseEventMapTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseEventTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParsePropertyMapTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParsePRopertyTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseMethodSemanticsTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseMethodImplTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseModuleRefTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseTypeSpecTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseImplMapTabble(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseFieldRVATable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseAssemblyTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseAssemblyProcessorTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseAssemblyOSTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseAssemblyRefTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseAssemblyRefProcessorTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseAssemblyRefOsTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseAssemblyFileTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseExportedTypeTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseMainfestResourceTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseNestedClassTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseGenericParamTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseMethodSpecTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }

        private void ParseGenericParamConstraintTable(Stream s, uint rowCount)
        {
            throw new NotImplementedException();
        }
    }
}

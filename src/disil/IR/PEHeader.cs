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

using System.Collections.ObjectModel;
using System.IO;

namespace disil.IR
{
    //=========================================================================
    //Represents the "image header" in a Windows PE file, as defined by the 
    //Microsoft PE/COFF spec version 8.2. Here we've combined the
    //COFF File Header" and the PE Optional Header" into a single structure.
    //See sections "2.3" and "2.4" for details.
    //=========================================================================
    internal class PEHeader
    {
        public MachineType MachineType{ get; private set; }
        public ushort NumberOfSections{ get; private set; }
        public uint TimeDateStamp{ get; private set; }
        public uint PointerToSymbolTable{ get; private set; }
        public uint NumberOfSymbols{ get; private set; }
        public uint SizeOfOptionalHeader{ get; private set; }
        public FileCharacteristics Characteristics { get; private set; }
        
        //NOTE: ImageType is called "MagicNumber" in the PE/COFF spec.
        public ImageType ImageType { get; private set;  }
        
        public byte MajorLinkerVersion { get; private set; }
        public byte MinorLinkerVersion { get; private set; }
        public uint SizeOfCode { get; private set; }
        public uint SizeOfInitializedData { get; private set; }
        public uint SizeOfUnInitializedData { get; private set; }

        //NOTE: EntryPointRVA is called "AddressOfEntryPoint" in the PE/COFF 
        //spec. We use "EntryPointRVA" for 2 reasons:
        //  1) It's the name used in the Ecma Spec
        //  2) It emphasies that the value is an RVA not a VA, eliminating 
        //     a potential source of confusion.
        public uint EntryPointRVA { get; private set;  }

        public uint BaseOfCode { get; private set; }

        //Note: Will be null if ImageType == PE32PlusFile
        public uint? BaseOfData { get; private set; }

        public ulong ImageBase { get; private set; }
        public uint SectionAlignment { get; private set; }
        public uint FileAlignment { get; private set; }
        public ushort MajorOSVersion { get; private set; }
        public ushort MinorOSVersion { get; private set; }
        public ushort MajorImageVersion { get; private set; }
        public ushort MinorImageVersion { get; private set; }
        public ushort MajorSubsystemVersion { get; private set; }
        public ushort MinorSubsystemVersion { get; private set; }
        public uint Win32VersionValue { get; private set; }
        public uint SizeOfImage { get; private set; }
        public uint SizeOfHeaders { get; private set; }
        public uint CheckSum { get; private set; }
        public WindowsSubsystem Subsystem { get; private set; }
        public DllCharacteristics DllCharacteristics { get; private set; }
        public ulong SizeOfStackReserve { get; private set; }
        public ulong SizeOfStackCommit { get; private set; }
        public ulong SizeOfHeapReserve { get; private set; }
        public ulong SizeOfHeapCommit { get; private set; }
        public uint LoaderFlags { get; private set; }
        public ReadOnlyCollection<BlockReference> DataDirectories { get; private set; }

        internal void Load(Stream s)
        {
            s.AssumeArgNotNull("s");

            MachineType = ((MachineType)s.ReadUShortLE()).AssumeDefined();
            NumberOfSections = s.ReadUShortLE().AssumeGt(0, "Empty section table detected. All valid PE files should contain at least one section.");
            TimeDateStamp = s.ReadUIntLE();
            PointerToSymbolTable = s.ReadUIntLE().AssumeEquals(0u);
            NumberOfSymbols = s.ReadUIntLE().AssumeEquals(0u);
            SizeOfOptionalHeader = s.ReadUShortLE();
            Characteristics = (FileCharacteristics)s.ReadUShortLE();

            Util.Assume(SizeOfOptionalHeader >= 92, "Unexpected optional header size");

            var optHeaderStart = s.Position;

            ImageType = ((ImageType)s.ReadUShortLE()).AssumeDefined();

            if (ImageType == ImageType.PE32PlusFile)
            {
                Util.Assume(SizeOfOptionalHeader >= 108, "Unexpected optional header size");
            }

            MajorLinkerVersion = s.ReadByteOrThrow();
            MinorLinkerVersion = s.ReadByteOrThrow();
            SizeOfCode = s.ReadUIntLE();
            SizeOfInitializedData = s.ReadUIntLE();
            SizeOfUnInitializedData = s.ReadUIntLE();
            EntryPointRVA = s.ReadUIntLE();
            BaseOfCode = s.ReadUIntLE();
            if (ImageType == ImageType.PEFile)
            {
                BaseOfData = s.ReadUIntLE();
                ImageBase = s.ReadUIntLE();
            }
            else
            {
                ImageBase = s.ReadULongLE();
            }

            SectionAlignment = s.ReadUIntLE();
            FileAlignment = s.ReadUIntLE();
            MajorOSVersion = s.ReadUShortLE();
            MinorOSVersion = s.ReadUShortLE();
            MajorImageVersion = s.ReadUShortLE();
            MinorImageVersion = s.ReadUShortLE();
            MajorSubsystemVersion = s.ReadUShortLE();
            MinorSubsystemVersion = s.ReadUShortLE();
            Win32VersionValue = s.ReadUIntLE();
            SizeOfImage = s.ReadUIntLE();
            SizeOfHeaders = s.ReadUIntLE();
            CheckSum = s.ReadUIntLE();
            Subsystem = ((WindowsSubsystem) s.ReadUShortLE()).AssumeDefined();
            DllCharacteristics = (DllCharacteristics) s.ReadUShortLE();

            if (ImageType == ImageType.PEFile)
            {
                SizeOfStackReserve = s.ReadUIntLE();
                SizeOfStackCommit = s.ReadUIntLE();
                SizeOfHeapReserve = s.ReadUIntLE();
                SizeOfHeapCommit = s.ReadUIntLE();
            }
            else
            {
                SizeOfStackReserve = s.ReadULongLE();
                SizeOfStackCommit = s.ReadULongLE();
                SizeOfHeapReserve = s.ReadULongLE();
                SizeOfHeapCommit = s.ReadULongLE();
            }
            LoaderFlags = s.ReadUIntLE();

            var dataDirCount = s.ReadUIntLE();
            
            Util.Assume(dataDirCount * 8 <= SizeOfOptionalHeader - (s.Position - optHeaderStart), "Inconsistent header fields");

            if (dataDirCount != 0)
            {
                var dirs = new BlockReference[dataDirCount];

                for (int i = 0; i < dirs.Length; ++i)
                {
                    dirs[i] = new BlockReference(s);
                }

                DataDirectories = new ReadOnlyCollection<BlockReference>(dirs);
            }
        }
    }
}

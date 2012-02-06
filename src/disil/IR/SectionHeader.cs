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

using System.IO;
using System.Text;

namespace disil.IR
{
    //=========================================================================
    // Represents a "section header" from the section table of a Windows PE 
    // file. See section 3 of version 8.2 of Microsoft PE/COFF spec for 
    // details.
    //=========================================================================
    internal class SectionHeader
    {
        public string Name { get; private set; }
        public uint VirtualSize { get; private set; }
        
        //NOTE: SectionRVA is called "VirtualAddress" in the PE spec.
        //We call it "SectionRVA" here to emphasize that it's an RVA
        //not a VA.
        public uint SectionRVA { get; private set; }

        public uint SizeOfRawData { get; private set; }
        public uint PointerToRawData { get; private set; }
        public uint PointerToRelocations { get; private set; }
        public uint PointerToLineNumbers { get; private set; }
        public ushort NumberOfRelocations { get; private set; }
        public ushort NumberOfLineNumbers { get; private set; }
        public SectionFlags SectionFlags{ get; private set;}

        internal void Load(Stream s)
        {
            Name = Encoding.ASCII.GetString(s.ReadBytes(8)).TrimEnd('\0');
            VirtualSize = s.ReadUIntLE();
            SectionRVA = s.ReadUIntLE();
            SizeOfRawData = s.ReadUIntLE();
            PointerToRawData = s.ReadUIntLE();
            PointerToRelocations = s.ReadUIntLE();
            PointerToLineNumbers = s.ReadUIntLE();
            NumberOfRelocations = s.ReadUShortLE();
            NumberOfLineNumbers = s.ReadUShortLE();
            SectionFlags = (SectionFlags)s.ReadUIntLE();
        }

        //Returns true if dd is contained entirely within the section's
        //initialized data, and false otherwise.
        public bool IsInitializedData(BlockReference dd)
        {
            return
                dd != null
                && dd.RVA >= SectionRVA
                && (dd.RVA + dd.Size) <= (SectionRVA + SizeOfRawData);

        }

        public bool IsInitializedData(uint rva, uint size)
        {
            return
                rva >= SectionRVA
                && (rva + size) <= (SectionRVA + SizeOfRawData);
        }
    }
}
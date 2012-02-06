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

namespace disil.IR
{
    //=========================================================================
    // Denotes a reference to a block of memory in a loaded Windows PE image.
    // The reference is defined by an RVA (relative virtual address) and a 
    // size. A relative virtual address is a simple offset, relative to
    // PEHeader.ImageBase (i.e. an RVA of 10 would represent the virtual 
    // address "PEHeader.ImageBase + 10"). 
    //=========================================================================
    internal class BlockReference
    {
        public uint RVA { get; set; }
        public uint Size { get; set; }

        internal BlockReference()
        {
            
        }

        internal BlockReference(Stream s)
        {
            Load(s);
        }
               
        internal void Load(Stream s)
        {
            s.AssumeArgNotNull("s");

            RVA = s.ReadUIntLE();
            Size = s.ReadUIntLE();
        }
    }
}

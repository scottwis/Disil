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
    internal class ClrHeader
    {
        public uint HeaderSize { get; internal set; }
        public ushort MajorRuntimeVersion { get; internal set; }
        public ushort MinorRuntimeVersion { get; internal set; }
        public BlockReference MetaDataPointer { get; internal set; }
        public ClrHeaderFlags Flags { get; internal set; }
        public uint EntryPointToken { get; internal set; }
        public BlockReference Resources { get; internal set; }
        public BlockReference StrongNameSignature { get; internal set; }
        public BlockReference CodeManagerTable { get; internal set; }
        public BlockReference VTableFixups { get; internal set; }
        public BlockReference ExportAddressTableJumps { get; internal set; }
        public BlockReference ManagedNativeHeader { get; internal set; }

        internal ClrHeader()
        {
            MetaDataPointer = new BlockReference();
            Resources = new BlockReference();
            StrongNameSignature = new BlockReference();
            CodeManagerTable = new BlockReference();
            VTableFixups = new BlockReference();
            ExportAddressTableJumps = new BlockReference();
            ManagedNativeHeader = new BlockReference();
        }

        internal void Load(Stream s)
        {
            s.AssumeArgNotNull("s");

            HeaderSize = s.ReadUIntLE();
            MajorRuntimeVersion = s.ReadUShortLE();
            MinorRuntimeVersion = s.ReadUShortLE();
            MetaDataPointer.Load(s);
            Flags = (ClrHeaderFlags) s.ReadUIntLE();
            EntryPointToken = s.ReadUIntLE();
            Resources.Load(s);
            StrongNameSignature.Load(s);
            CodeManagerTable.Load(s);
            VTableFixups.Load(s);
            ExportAddressTableJumps.Load(s);
            ManagedNativeHeader.Load(s);
        }
    }
}
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace disil.IR
{
	//Represents the "header" for the contents of the #~ stream in
	//a managed PE file. The header constitutes everything inside the
	//#~ stream that precedes the actualy meta-data tables. 
    internal class TablesHeader
    {
        private static readonly uint s_validTableMask;

        static TablesHeader()
        {
            foreach (var i in Enum.GetValues(typeof(TableID)).Cast<int>())
            {
                s_validTableMask |= (1U << i);
            }
        }

        public uint Reserved0 { get; private set; }
        public byte MajorVersion { get; private set; }
        public byte MinorVersion { get; private set; }
        private byte m_heapSizes;

        public bool LargeStringIndexes
        {
            get { return (m_heapSizes & 0x1) != 0; }
        }

        public bool LargeGuidIndexes
        {
            get { return (m_heapSizes & 0x2) != 0; }
        }
        public bool LargeBlobIndexes
        {
            get { return (m_heapSizes & 0x4) != 0; }
        }

        public byte Reserved1 { get; private set; }
        public ulong ValidTables { get; private set; }
        public ulong SortedTables { get; private set; }
        public ReadOnlyCollection<uint> RowCounts { get; private set; }

        internal void Load(Stream s)
        {
            s.AssumeNotNull();
            Reserved0 = s.ReadUIntLE();
            MajorVersion = s.ReadByteOrThrow();
            MinorVersion = s.ReadByteOrThrow();
            m_heapSizes = s.ReadByteOrThrow();
            Reserved1 = s.ReadByteOrThrow();
            ValidTables = s.ReadULongLE();

            Util.Assume(
                (ValidTables & ~s_validTableMask) == 0,
                "Unknown table specified in ValidTables mask"
            );

            SortedTables = s.ReadULongLE();
			
			var numberOfTables = ValidTables.NumberOfSetBits();
			
			Util.Assume((ValidTables & ~((1UL << (int)TableID.MAX_TABLE_ID) - 1)) == 0UL, "Unrecogonized meta-data table detected");
			Util.Assume(numberOfTables >= 1, "Too fiew meta-data tables");
			
			var rowCounts = new uint[numberOfTables];
			
			for (int i = 0; i < numberOfTables; ++i)
			{
				rowCounts[i] = s.ReadUIntLE();
			}
			
			RowCounts = new ReadOnlyCollection<uint>(rowCounts);
        }
    }
}
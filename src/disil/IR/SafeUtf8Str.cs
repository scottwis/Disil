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

using System.Text;

namespace disil.IR
{
    //Represents an entry in the meta-data string heap.
    //System.Reflection (per the MS reference sources, anyways) uses a similar
    //structure, although it just uses a raw pointer. Given that
    //our assembly is not fully trusted by default, we use an array + offset
    //representation instead.
    internal struct SafeUtf8Str
    {
        public SafeUtf8Str(byte[] bytes, int offset)
        {
            bytes.AssumeArgNotNull("bytes");
            offset.AssumeArgGte(0, "offset");
            offset.AssumeArgLte(bytes.Length, "offset");
            m_bytes = bytes;
            m_offset = offset;
        }

        private readonly byte[] m_bytes;
        private readonly int m_offset;

        public override string ToString()
        {
            int i;

            for (i = m_offset; i < m_bytes.Length && m_bytes[i] != 0; ++i)
            {
            }

            return Encoding.UTF8.GetString(m_bytes, m_offset, i - m_offset);
        }
    }
}

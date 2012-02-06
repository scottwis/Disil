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

using disil.IO;

namespace disil.IR
{
    internal class StreamHeader
    {
        private const int s_maxNameLen = 32;

        public uint Offset { get; internal set; }
        public uint Size { get; internal set; }
        public string Name { get; internal set; }

        public void Load(Stream s, StringBuilder builder)
        {
            Offset = s.ReadUIntLE();
            Size = s.ReadUIntLE();
            
            Util.Assume(s.AbosultePosition() % 4 == 0, "Name field not alligned");
            using (var subStream = new SubStream(s, s_maxNameLen, "Invalid stream header (name too long)"))
            {
                var c = (char) subStream.ReadByteOrThrow();
                while (c != '\0')
                {
                    builder.Append(c);
                    c = (char)subStream.ReadByteOrThrow();
                }

                var padding = (builder.Length + 1)%4;
                for (; padding > 0 && padding < 4 ; ++padding)
                {
                    subStream.ReadByteOrThrow().AssumeEquals(
                        (byte)0, 
                        "Expected a padding byte"
                    );
                }

                Name = builder.ToString();
                builder.Remove(0, builder.Length);
            }
        }
    }
}
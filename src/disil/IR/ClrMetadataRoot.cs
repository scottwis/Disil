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
using System.Linq;
using System.Text;

using disil.Collections;
using disil.IO;

namespace disil.IR
{
    internal class ClrMetadataRoot
    {
        private const int s_minSize = 20;

        public ushort MajorVersion { get; internal set; }
        public ushort MinorVersion { get; internal set; }
        public uint Reserved { get; internal set; }
        public string Version { get; internal set; }
        public ushort Flags { get; internal set; }
        public ReadOnlyDictionary<string,StreamHeader> StreamHeaders { get; internal set; }

        internal void Load(Stream stream, long size)
        {
            stream.AssumeArgNotNull("stream");
            size.AssumeArgGte(s_minSize, "size");
            
            using(var subStream = new SubStream(stream, size, "Invalid meta-data root."))
            {
                subStream.ReadUIntLE().AssumeEquals(
                    0x424A5342U, 
                    "Incorrect meta-data signature."
                );

                MajorVersion = subStream.ReadUShortLE();
                MinorVersion = subStream.ReadUShortLE();
                Reserved = subStream.ReadUIntLE();
                var versionLen = 
                    (int)(
                        subStream.ReadUIntLE().AssumeLte(
                            256, 
                            "CLR Version string too long"
                        ));
                
                Util.Assume(
                    versionLen % 4 == 0, 
                    "Mis-algned version string length in CLR Header."
                );
                
                Version = 
                    Encoding.UTF8.GetString(
                        subStream.ReadBytes(versionLen)
                    ).TrimEnd('\0');

                Flags = subStream.ReadUShortLE();

                var numberOfStreams = subStream.ReadUShortLE();
                var streamHeaders = new StreamHeader[numberOfStreams];

                var b = new StringBuilder();

                for (var i = 0; i < numberOfStreams; ++i)
                {
                    streamHeaders[i] = new StreamHeader();
                    streamHeaders[i].Load(subStream, b);
                }

                StreamHeaders = 
                    new ReadOnlyDictionary<string,StreamHeader>(
                        streamHeaders.ToDictionary(h=>h.Name)
                    );
            }
       }
    }
}
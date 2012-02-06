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

namespace disil.IO
{
    //Defines a bounded "sub set" of a given stream. Any attempts to read or 
    //write outside the bounds of the substream will result in an exception.
    //This class is mainly used when parsing CLR metada out of a PE file.
    //It makes it easier to ensure consistency between declared structure
    //sizes in different areas of the file, and to prevent reading beyond
    //the boundary of "initialized data" within a section. This is particulary
    //true for structures that contain variable length fields.
    internal class SubStream : Stream
    {
        private const string s_defaultReadMessage =
            "An attempt was made to read beyond the bounds of the sub-stream.";

        private const string s_defaultWriteMessage =
            "An attempt was made to write beyond the bounds of the sub-stream.";

        private readonly Stream m_stream;
        private readonly long m_startPos;
        private long m_size;
        private readonly string m_readMessage;
        private readonly string m_writeMessage;

        public SubStream(
            Stream stream, 
            long size
        ) : this(stream, size, s_defaultReadMessage, s_defaultWriteMessage)
        {
            
        }

        public SubStream(
            Stream stream, 
            long size, 
            string readErrorMessage
        ) : this (stream, size, readErrorMessage, s_defaultWriteMessage)
        {
            
        }

        public SubStream(
            Stream stream, 
            long size, 
            string readErrorMessage, 
            string writeErrorMessage
        )
        {
            m_stream = stream.AssumeArgNotNull("stream");
            m_size = size.AssumeArgGte(0, "size");
            m_startPos = stream.Position;
            m_readMessage = readErrorMessage;
            m_writeMessage = writeErrorMessage;
        }

        public override void Flush()
        {
            m_stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Util.Assume(m_stream.Position >= m_startPos, "The underlying stream has been invalidated.");

            //1. Translate offset into an absolute offset from the beginig of m_stream
            switch (origin)
            {
                case SeekOrigin.Begin:
                    offset += m_startPos;
                    break;
                case SeekOrigin.Current:
                    offset += m_stream.Position;
                    break;
                case SeekOrigin.End:
                    offset += m_startPos + m_size + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }

            //2. If the translated offset < m_starPos, throw an exception.
            if (offset < m_startPos)
            {
                throw new IOException("An attempt was made to move the position before the begining of the sub-stream");
            }
                       
            //3. Otherwise position the stream at the translated offset. 
            //Note that we don't check to ensure offset <= m_startPos + size.
            //It's ok to seek beyond the end of the stream, it just not ok
            //to read or write there.

            var sPos = m_stream.Seek(offset, SeekOrigin.Begin);
            Util.Assume(sPos >= m_startPos, "m_stream.Seek returned an unexpected value");
            return sPos - m_startPos;
        }

        public override void SetLength(long value)
        {
            m_size = value.AssumeArgGte(0, "value");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Util.Assume(m_stream.Position >= m_startPos, "The underlying stream has been invalidated!");
            if (
                (m_stream.Position >= m_startPos + m_size)
                || (m_stream.Position + count  >= m_startPos + m_size)
            )
            {
                throw new IOException(m_readMessage);
            }

            return m_stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Util.Assume(m_stream.Position >= m_startPos, "The underlying stream has been invalidated!");
            if (
                (m_stream.Position >= m_startPos + m_size)
                || (m_stream.Position + count >= m_startPos + count)
            )
            {
                throw new IOException(m_writeMessage);
            }

            m_stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return m_stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_stream.CanWrite; }
        }

        public override long Length
        {
            get { return m_size; }
        }

        public override long Position
        {
            get
            {
                Util.Assume(m_stream.Position >= m_startPos, "The underlying stream has been invalidated.");

                return m_stream.Position - m_startPos;
            }
            set
            {
                if (value < 0)
                {
                    throw new IOException("An attempt was made to move the position before the begining of the sub-stream");
                }
                value += m_startPos;
                m_stream.Position = value;
            }
        }

        public Stream UnderlyingStream
        {
            get { return m_stream; }
        }
    }
}

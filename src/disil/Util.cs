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
using System.Collections.Generic;
using System.IO;

using disil.IO;

namespace disil
{
    internal static class Util
    {
        private const string s_sawExpected = "Error. Expected '{0}', but saw '{1}'";

        public static T AssumeNotNull<T>(this T item) where T : class
        {
            if (item == null)
            {
                throw new InternalErrorException("Unexpected null value.");
            }
            return item;
        }

        public static T AssumeNotNull<T>(this T item, string message) where T : class
        {
            if (item == null)
            {
                throw new InternalErrorException(message);
            }
            return item;
        }

        public static T AssumeArgNotNull<T>(this T item, string paramName) where T : class
        {
            if (item == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return item;
        }

        public static T AssumeEquals<T>(this T item, T other) 
        {
            if (!EqualityComparer<T>.Default.Equals(item, other))
            {
                throw new InternalErrorException(
                    String.Format(
                        s_sawExpected, 
                        other,
                        item
                    ));
            }
            return item;
        }

        
        public static T AssumeEquals<T>(this T item, T other, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(item, other))
            {
                throw new InternalErrorException(message);
            }
            return item;
        }

        public static string AssumeEquals(this string item, string other)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Compare(item, other) != 0)
            {
                throw new InternalErrorException(
                    String.Format(s_sawExpected,item,other)
                );    
            }
            return item;
        }

        public static T[] AssumeEquals<T>(this T[] item, T[] other, string message)
        {
            if ((item == null) != (other == null))
            {
                throw new InternalErrorException(message);
            }
            
            if (item != null)
            {
                if (item.Length != other.Length)
                {
                    throw new InternalErrorException(message);
                }

                for (var i = 0; i < item.Length; ++i)
                {
                    item[i].AssumeEquals(other[i], message);
                }
            }

            return item;
        }

        public static void Assume(bool condition, string message)
        {
            if (!condition)
            {
                throw new InternalErrorException(message);
            }
        }

        public static bool AssumeTrue(this bool condition, string message)
        {
            Assume(condition,message);
            return condition;
        }

        public static T AssumeDefined<T>(this T value, string message)
        {
            var type = typeof(T);
            if (!Enum.IsDefined(type, value))
            {
                throw new InternalErrorException(
                    string.Format(
                        message,
                        value,
                        type
                    ));
            }

            return value;

        }

        public static T AssumeDefined<T>(this T value)
        {
            var type = typeof(T);
            if (!Enum.IsDefined(type, value))
            {
                throw new InternalErrorException(
                    string.Format(
                        "The value {0} is not a valid member of {1}",
                        value,
                        type
                    ));
            }

            return value;
        }

        public static ushort AssumeGt(this ushort value, ushort other, string message)
        {
            if (value <= other)
            {
                throw new InternalErrorException(message);
            }
            return value;
        }

        public static ulong AssumeGt(this ulong value, ulong other, string message)
        {
            if (value <= other)
            {
                throw new InternalErrorException(message);
            }
            return value;
        }

        public static uint AssumeGt(this uint value, uint other, string message)
        {
            if (value <= other)
            {
                throw new InternalErrorException(message);
            }
            return value;
        }

        public static uint AssumeGte(this uint value, uint other)
        {
            if (value <= other)
            {
                throw new InternalErrorException(
                    string.Format(
                        "Expected a value >= '{0}', but saw '{1}' instead.",
                        other,
                        value
                    )
                );
            }
            return value;
        }

        public static uint AssumeLte(this uint value, uint other, string message)
        {
            if (value > other)
            {
                throw new InternalErrorException(message);
            }
            return value;
        }

        public static long AssumeArgGt(this long value, long other, string paramName)
        {
            if (value <= other)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
            return value;
        }

        public static long AssumeArgGte(this long value, long other, string paramName)
        {
            if (value < other)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
            return value;
        }

        public static int AssumeArgGte(this int value, int other, string paramName)
        {
            if (value < other)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
            return value;
        }

        public static int AssumeArgLte(this int value, int other, string paramName)
        {
            if (value > other)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
            return value;
        }

        public static void Seek(this Stream s, long offset)
        {
            s.Seek(offset, SeekOrigin.Begin);
        }

        public static byte[] ReadBytes(this Stream s, int count)
        {
            var bytes = new byte[count];
            s.Read(bytes, 0, count).AssumeEquals(count);
            return bytes;
        }

        public static uint ReadUIntLE(this Stream s)
        {
            sizeof (uint).AssumeEquals(4);

            var bytes = s.ReadBytes(sizeof (uint));
            return
                bytes[0]
                | ((uint)bytes[1] << 8)
                | ((uint)bytes[2] << 16)
                | ((uint)bytes[3] << 24);
        }

        public static ushort ReadUShortLE(this Stream s)
        {
            sizeof (ushort).AssumeEquals(2);

            var bytes = s.ReadBytes(sizeof(ushort));
            return
                (ushort) 
                (
                    bytes[0]
                    | (bytes[1] << 8)
                );
        }

        public static ulong ReadULongLE(this Stream s)
        {
            sizeof(ulong).AssumeEquals(8);

            var bytes = s.ReadBytes(sizeof (ulong));
            return
                bytes[0]
                | ((ulong) bytes[1] << 8)
                | ((ulong) bytes[2] << 16)
                | ((ulong) bytes[3] << 24)
                | ((ulong) bytes[4] << 32)
                | ((ulong) bytes[5] << 40)
                | ((ulong) bytes[6] << 48)
                | ((ulong) bytes[7] << 56);
        }

        public static byte ReadByteOrThrow(this Stream s)
        {
            var i = s.ReadByte();
            Assume(i >= 0, "Couldn't read from stream");
            return (byte) i;
        }

        // Get's the "abosulte position" of a stream, digging through any substreams.
        public static long AbosultePosition(this Stream s)
        {
            for (
                var subS = s as SubStream; 
                subS != null; 
                s = subS.UnderlyingStream, subS = s as SubStream
            ) {}

            return s.AssumeArgNotNull("s").Position;
        }

        //From http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
        //extended to 64 bits.
        public static int NumberOfSetBits(this ulong v)
        {
            //01010101 01010101 01010101 01010101 01010101 01010101 01010101 01010101
            const ulong b0 = 0x5555555555555555UL;
            //00110011 00110011 00110011 00110011 00110011 00110011 00110011 00110011
            const ulong b1 = 0x3333333333333333UL;
            //00001111 00001111 00001111 00001111 00001111 00001111 00001111 00001111
            const ulong b2 = 0x0F0F0F0F0F0F0F0FUL;
            //00000000 11111111 00000000 11111111 00000000 11111111 00000000 11111111
            const ulong b3 = 0x00FF00FF00FF00FFUL;
            //00000000 00000000 11111111 11111111 00000000 00000000 11111111 11111111
            const ulong b4 = 0x0000FFFF0000FFFFUL;
            //00000000 00000000 00000000 00000000 11111111 11111111 11111111 11111111
            const ulong b5 = 0x00000000FFFFFFFFUL;

            var c = v - ((v >> 1) & b0);
            c = ((c >> 2) & b1) + (c & b1);
            c = ((c >> 4) + c) & b2;
            c = ((c >> 8) + c) & b3;
            c = ((c >> 16) + c) & b4;
            c = ((c >> 32) + c) & b5;

            return (int)c;
        }
    }
}

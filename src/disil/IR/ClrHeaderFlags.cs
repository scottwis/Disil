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

namespace disil.IR
{
    [Flags]
    internal enum ClrHeaderFlags : uint 
    {
        //COMIMAGE_FLAGS_ILONLY
        ILOnly = 0x00000001,
        //COMIMAGE_FLAGS_32BITREQUIRED
        //Image can only be loaded into a 32-bit process, for instance if 
        //there are 32-bit vtablefixups, or casts from native integers to 
        //int32. CLI implementations that have 64-bit native integers shall 
        //refuse loading binaries with this flag set.
        Requires32Bit = 0x00000002,
        //COMIMAGE_FLAGS_STRONGNAMESIGNED
        //Image has a strong name signature.
        StrongNameSigned = 0x00000008,
        //COMIMAGE_FLAGS_TRACKDEBUGDATA
        TrackDebugData = 0x00010000
    }
}

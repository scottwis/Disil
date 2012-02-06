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
    //=========================================================================
    //Defines an enum describing the elements of the "DllCharacteristics" bit 
    //field in the "COFF Optional Image Header" in a Windows PE file as 
    //described by the Microsoft PE/COFF spec version 8.2. See section 2.4.2 
    //for details.
    //=========================================================================
    [Flags]
    public enum DllCharacteristics : ushort
    {
        //Reserved, must be zero.
        //0x0001,0x0002,0x0004,0x0008,0x1000

        //IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE
        //DLL can be relocated at load time.
        DynamicBase = 0x0040,
        //IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY
        //Code Integrity checks are enforced.
        ForceIntegrity = 0x0080,
        //IMAGE_DLL_CHARACTERISTICS_NX_COMPAT
        //Image is NX compatible.
        NoExecuteCompatible = 0x0100,
        //IMAGE_DLLCHARACTERISTICS_ NO_ISOLATION
        //Isolation aware, but do not isolate the image.
        NoIsolation = 0x0200,
        //IMAGE_DLLCHARACTERISTICS_ NO_SEH
        //Does not use structured exception (SE) handling. No SE handler may
        //be called in this image.
        NoSeh = 0x0400,
        //IMAGE_DLLCHARACTERISTICS_ NO_BIND
        //Do not bind the image.
        NoBind = 0x0800,
        //IMAGE_DLLCHARACTERISTICS_ WDM_DRIVER
        //A WDM driver.
        WdmDriver = 0x2000,
        //IMAGE_DLLCHARACTERISTICS_ TERMINAL_SERVER_AWARE
        //Terminal Server aware.
        TerminalServerAware = 0x8000
    }
}

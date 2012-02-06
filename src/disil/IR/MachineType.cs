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

namespace disil.IR
{
    //=========================================================================
    //Defines an enum describing the "PE/COFF MachineType" constants described 
    //in the Microsoft PE/COFF spec version 8.2. See section 2.3 for details.
    //=========================================================================
    internal enum MachineType : ushort
    {
        //IMAGE_FILE_MACHINE_UNKNOWN
        //The contents of this field are assumed to be applicable to any machine type
        Unknown = 0x0,
        //IMAGE_FILE_MACHINE_AM33
        //Matsushita AM33
        Am33 = 0x1d3,
        //IMAGE_FILE_MACHINE_AMD64
        //x64
        Amd64 = 0x8664,
        //IMAGE_FILE_MACHINE_ARM
        //ARM little endian
        Arm = 0x1c0,
        //IMAGE_FILE_MACHINE_ARMV7
        //ARMv7 (or higher) Thumb mode only
        ArmV7 = 0x1c4,
        //IMAGE_FILE_MACHINE_EBC
        //EFI byte code
        Ebc = 0xebc,
        //IMAGE_FILE_MACHINE_I386
        //Intel 386 or later processors and compatible processors
        I386 = 0x14c,
        //IMAGE_FILE_MACHINE_IA64
        //Intel Itanium processor family
        IA64 = 0x200,
        //IMAGE_FILE_MACHINE_M32R
        //Mitsubishi M32R little endian
        M32R = 0x9041,
        //IMAGE_FILE_MACHINE_MIPS16
        //MIPS16
        Mips = 0x266,
        //IMAGE_FILE_MACHINE_MIPSFPU
        //MIPS with FPU
        MipsFpu = 0x366,
        //IMAGE_FILE_MACHINE_MIPSFPU16
        //MIPS16 with FPU
        Mips16Fpu = 0x466,
        //IMAGE_FILE_MACHINE_POWERPC
        //Power PC little endian
        PowerPc = 0x1f0,
        //IMAGE_FILE_MACHINE_POWERPCFP
        //Power PC with floating point support
        PowerPcFp = 0x1f1,
        //IMAGE_FILE_MACHINE_R4000
        //0x166
        //MIPS little endian
        R4000 = 0x166,
        //IMAGE_FILE_MACHINE_SH3
        //Hitachi SH3
        Sh3 = 0x1a2,
        //IMAGE_FILE_MACHINE_SH3DSP
        //Hitachi SH3 DSP
        Sh3Dsp = 0x1a3,
        //IMAGE_FILE_MACHINE_SH4
        //Hitachi SH4
        Sh4 = 0x1a6,
        //IMAGE_FILE_MACHINE_SH5
        //Hitachi SH5
        Sh5 = 0x1a8,
        //IMAGE_FILE_MACHINE_THUMB
        //ARM or Thumb (“interworking”)
        Thumb = 0x1c2,
        //IMAGE_FILE_MACHINE_WCEMIPSV2
        //MIPS little-endian WCE v2
        WceMpisV2 = 0x169,
    }
}

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
    //Defines an enum representing the elements of the "Characteristics" bit 
    //field in the "COFF/PE" head, as described in the Microsoft PE/COFF spec 
    //version 8.2. See section 2.3.2 for details.
    //=========================================================================
    [Flags]
    public enum FileCharacteristics : ushort
    {
        //===========================================================
        //RESERVED Flags:
        //    0x0040
        //===========================================================

        //IMAGE_FILE_RELOCS_STRIPPED
        //Image only, Windows CE, and Windows NT® and later. This indicates
        //that the file does not contain base relocations and must therefore
        //be loaded at its preferred base address. If the base address is not
        //available, the loader reports an error. The default behavior of the
        //linker is to strip base relocations from executable (EXE) files.
        RelocsStripped = 0x0001,
        //IMAGE_FILE_EXECUTABLE_IMAGE
        //Image only. This indicates that the image file is valid and can be
        //run. If this flag is not set, it indicates a linker error.
        Image = 0x0002,
        //IMAGE_FILE_LINE_NUMS_STRIPPED
        //COFF line numbers have been removed. This flag is deprecated and
        //should be zero.
        LineNumberStripped = 0x0004,
        //IMAGE_FILE_LOCAL_SYMS_STRIPPED
        //COFF symbol table entries for local symbols have been removed. This
        //flag is deprecated and should be zero.
        LocalSymsStripped = 0x0008,
        //IMAGE_FILE_AGGRESSIVE_WS_TRIM
        //Obsolete. Aggressively trim working set. This flag is deprecated for
        //Windows 2000 and later and must be zero.
        AggressivelyTrimWorkingSet = 0x0010,
        //IMAGE_FILE_LARGE_ADDRESS_AWARE
        //Application can handle > 2GB addresses.
        LargeAddressAware = 0x0020,
        //IMAGE_FILE_BYTES_REVERSED_LO
        //Little endian: the least significant bit (LSB) precedes the most
        //significant bit (MSB) in memory. This flag is deprecated and should
        //be zero.
        LittleEndian = 0x0080,
        //IMAGE_FILE_32BIT_MACHINE
        //Machine is based on a 32-bit-word architecture.
        Machine32Bit = 0x0100,
        //IMAGE_FILE_DEBUG_STRIPPED
        //Debugging information is removed from the image file.
        DebugStripped = 0x0200,
        //IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP
        //If the image is on removable media, fully load it and copy it to the
        //swap file.
        RunFromSwapIfOnRemovableMedia = 0x0400,
        //IMAGE_FILE_NET_RUN_FROM_SWAP
        //If the image is on network media, fully load it and copy it to the
        //swap file.
        RunFromSwapIfOnNetworkMedia = 0x0800,
        //IMAGE_FILE_SYSTEM
        //The image file is a system file, not a user program.
        SystemFile = 0x1000,
        //IMAGE_FILE_DLL
        //The image file is a dynamic-link library (DLL). Such files are
        //considered executable files for almost all purposes, although they
        //cannot be directly run.
        Dll = 0x2000,
        //IMAGE_FILE_UP_SYSTEM_ONLY
        //The file should be run only on a uniprocessor machine.
        UniProcessorOnly = 0x4000,
        //IMAGE_FILE_BYTES_REVERSED_HI
        //Big endian: the MSB precedes the LSB in memory. This flag is
        //deprecated and should be zero.
        BigEndian = 0x8000,
    }
}

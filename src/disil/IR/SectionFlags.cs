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
    // Defines an enum representing the "Section Flags" (characteristics) bit 
    // field stored in Section Headers inside Windows PE Files.
    // See section 3.1 of version 8.2 of the Microsoft PE/COFF spec for 
    // details.
    //=========================================================================
    [Flags]
    public enum SectionFlags : uint
    {
        //Reserved
        //0x00000000,0x00000001,0x00000002,0x00000004,0x00000010,0x00000400

        //IMAGE_SCN_TYPE_NO_PAD
        //The section should not be padded to the next boundary. This flag is
        //obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid
        //only for object files.
        NoPad = 0x00000008,
        //IMAGE_SCN_CNT_CODE
        //The section contains executable code.
        Code = 0x00000020,
        //IMAGE_SCN_CNT_INITIALIZED_DATA
        //The section contains initialized data.
        InitializedData = 0x00000040,
        //IMAGE_SCN_CNT_UNINITIALIZED_DATA
        //The section contains uninitialized data.
        UninitializedData = 0x00000080,
        //IMAGE_SCN_LNK_OTHER
        //Reserved for future use.
        Other = 0x00000100,
        //IMAGE_SCN_LNK_INFO
        //The section contains comments or other information. The .drectve
        //section has this type. This is valid for object files only.
        Info=0x00000200,
        //IMAGE_SCN_LNK_REMOVE
        //The section will not become part of the image. This is valid only
        //for object files.
        Remove=0x00000800,
        //IMAGE_SCN_LNK_COMDAT
        //The section contains COMDAT data. For more information, see section
        //5.5.6, “COMDAT Sections (Object Only).” This is valid only for object
        //files.
        Comdat = 0x00001000,
        //IMAGE_SCN_GPREL
        //The section contains data referenced through the global pointer (GP).
        GpRelative = 0x00008000,
        //IMAGE_SCN_MEM_PURGEABLE
        //Reserved for future use.
        Purgable = 0x00020000,
        //IMAGE_SCN_MEM_16BIT
        //For ARM machine types, the section contains Thumb code.  Reserved for
        //future use with other machine types.
        ThumbCode = 0x00020000,
        //IMAGE_SCN_MEM_LOCKED
        //Reserved for future use.
        Locked = 0x00040000,
        //IMAGE_SCN_MEM_PRELOAD
        //Reserved for future use.
        Preload = 0x00080000,
        //IMAGE_SCN_ALIGN_1BYTES
        //Align data on a 1-byte boundary. Valid only for object files.
        Align1Bytes = 0x00100000,
        //IMAGE_SCN_ALIGN_2BYTES
        //Align data on a 2-byte boundary. Valid only for object files.
        Align2Bytes = 0x00200000,
        //IMAGE_SCN_ALIGN_4BYTES
        //Align data on a 4-byte boundary. Valid only for object files.
        Align4Bytes = 0x00300000,
        //IMAGE_SCN_ALIGN_8BYTES
        //Align data on an 8-byte boundary. Valid only for object files.
        Align8Bytes = 0x00400000,
        //IMAGE_SCN_ALIGN_16BYTES
        //Align data on a 16-byte boundary. Valid only for object files.
        Align16Bytes = 0x00500000,
        //IMAGE_SCN_ALIGN_32BYTES
        //Align data on a 32-byte boundary. Valid only for object files.
        Align32Bytes = 0x00600000,
        //IMAGE_SCN_ALIGN_64BYTES
        //Align data on a 64-byte boundary. Valid only for object files.
        Align64Bytes = 0x00700000,
        //IMAGE_SCN_ALIGN_128BYTES
        //Align data on a 128-byte boundary. Valid only for object files.
        Align128Bytes = 0x00800000,
        //IMAGE_SCN_ALIGN_256BYTES
        //Align data on a 256-byte boundary. Valid only for object files.
        Align256Bytes = 0x00900000,
        //IMAGE_SCN_ALIGN_512BYTES
        //Align data on a 512-byte boundary. Valid only for object files.
        Align512Bytes = 0x00A00000,
        //IMAGE_SCN_ALIGN_1024BYTES
        //Align data on a 1024-byte boundary. Valid only for object files.
        Align1024Bytes = 0x00B00000,
        //IMAGE_SCN_ALIGN_2048BYTES
        //Align data on a 2048-byte boundary. Valid only for object files.
        Align2048Bytes = 0x00C00000,
        //IMAGE_SCN_ALIGN_4096BYTES
        //Align data on a 4096-byte boundary. Valid only for object files.
        Align4096Bytes = 0x00D00000,
        //IMAGE_SCN_ALIGN_8192BYTES
        //Align data on an 8192-byte boundary. Valid only for object files.
        Align8192Bytes = 0x00E00000,
        //IMAGE_SCN_LNK_NRELOC_OVFL
        //The section contains extended relocations.
        ExtendedReolocations = 0x01000000,
        //IMAGE_SCN_MEM_DISCARDABLE
        //The section can be discarded as needed.
        Discardable = 0x02000000,
        //IMAGE_SCN_MEM_NOT_CACHED
        //The section cannot be cached.
        NotCacheable = 0x04000000,
        //IMAGE_SCN_MEM_NOT_PAGED
        //The section is not pageable.
        NotPageable = 0x08000000,
        //IMAGE_SCN_MEM_SHARED
        //The section can be shared in memory.
        Shared = 0x10000000,
        //IMAGE_SCN_MEM_EXECUTE
        //The section can be executed as code.
        Executable = 0x20000000,
        //IMAGE_SCN_MEM_READ
        //The section can be read.
        Readable = 0x40000000,
        //IMAGE_SCN_MEM_WRITE
        //The section can be written to.
        Writable = 0x80000000,
    }
}

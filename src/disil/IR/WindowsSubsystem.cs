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
    //Defines an enum describing the valid values of the "Subsystem" field in 
    //the "Optional COFF Image Header" in a Windows PE file as described by
    //the Microsoft PE/COFF spec version 8.2. See section 2.4.2 for details.
    //=========================================================================
    internal enum WindowsSubsystem : ushort
    {
        //IMAGE_SUBSYSTEM_UNKNOWN
        //An unknown subsystem
        Uknown = 0,
        //IMAGE_SUBSYSTEM_NATIVE
        //Device drivers and native Windows processes
        Native = 1,
        //IMAGE_SUBSYSTEM_WINDOWS_GUI
        //The Windows graphical user interface (GUI) subsystem
        Gui = 2,
        //IMAGE_SUBSYSTEM_WINDOWS_CUI
        //The Windows character subsystem
        Character = 3,
        //IMAGE_SUBSYSTEM_POSIX_CUI
        //The Posix character subsystem
        Posix = 7,
        //IMAGE_SUBSYSTEM_WINDOWS_CE_GUI
        //Windows CE
        CeGui = 9,
        //IMAGE_SUBSYSTEM_EFI_APPLICATION
        //An Extensible Firmware Interface (EFI) application
        EfiApplication = 10,
        //IMAGE_SUBSYSTEM_EFI_BOOT_ SERVICE_DRIVER
        //An EFI driver with boot services
        EfiBootServiceDriver = 11,
        //IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER
        //An EFI driver with run-time services
        EfiRuntimeDriver = 12,
        //IMAGE_SUBSYSTEM_EFI_ROM
        //An EFI ROM image
        EfiRom = 13,
        //IMAGE_SUBSYSTEM_XBOX
        GrandTheftAuto = 14
    }
}

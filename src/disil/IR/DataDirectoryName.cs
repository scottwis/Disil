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
    // Defines an enum representing the indicies into the "data directory 
    // table" stored in the "Optional COFF Image Header" in a "Windows PE File"
    // as defined in version 8.2 of the Microsoft PE/COFF spec.
    //
    // Pleae note that a valid PE file may contain more or fewer data 
    // directories than what are included in this file. You should always 
    // check PEHeader.DataDirectories.Count when reading the data directory 
    // table. 
    //=========================================================================
    internal enum DataDirectoryName
    {
        ExportTable = 0,
        ImportTable,
        ResourceTable,
        ExceptionTable,
        CertificateTable,
        BaseRelocationTable,
        DebugTable,
        Architecture,
        GlobalPtr,
        ThreadLocalStorageTable,
        LoadConfigTable,
        BoundImportTable,
        ImportAddressTable,
        DelayImportDescriptor,
        CLRRuntimeHeader,
        Reserved
    }
}

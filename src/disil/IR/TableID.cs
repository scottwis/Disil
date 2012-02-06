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
    internal enum TableID
    {
        Module = 0x0,
        TypeRef = 0x1,
        TypeDef = 0x2,
        Field = 0x4,
        MethodDef = 0x06,
        Param = 0x8,
        InterfaceImpl = 0x09,
        MemberRef = 0x0A,
        CustomAttribute = 0x0B,
        FieldMarshal = 0x0D,
        DeclSecurty = 0x0E,
        ClassLayout = 0x0F,
        FieldLayout = 0x10,
        StandAloneSig = 0x11,
        EventMap = 0x12,
        Event = 0x14,
        PropertyMap = 0x15,
        Property = 0x17,
        MethodSemantics = 0x18,
        MethodImp = 0x19,
        ModuleRef = 0x1A,
        TypeSpec = 0x1B,
        ImplMap = 0x1C,
        FieldRVA = 0x1D,
        Assembly = 0x20,
        AssemblyProcessor = 0x21,
        AssemblyOS = 0x22,
        AssemblyRef = 0x23,
        AssemblyRefProcessor = 0x24,
        AssemblyRefOS = 0x25,
        File = 0x26,
        ExportedType = 0x27,
        ManifestResource = 0x28,
        NestedClass = 0x29,
        GenericParam = 0x2A,
        MethodSpec = 0x2B,
        GenericParamConstraint = 0x2C,
        MAX_TABLE_ID = 0x2c
    }
}

using System;

namespace SCUMSLang.SyntaxTree
{
    [Flags]
    public enum RecognizableReferences
    {
        Import = 1 << 0,
        UsingStatic = Import << 1,
        Typedef = UsingStatic << 1,
        Enumeration = Typedef << 1,
        Attribute = Enumeration << 1,
        FunctionOrEventHandler = Attribute << 1,
        Field = FunctionOrEventHandler << 1,
        Assignment = Field << 1,
        TemplateFor = Assignment << 1
    }
}

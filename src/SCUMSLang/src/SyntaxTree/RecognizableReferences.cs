using System;

namespace SCUMSLang.SyntaxTree
{
    [Flags]
    public enum RecognizableReferences
    {
        Import = 1 << 0,
        UsingStatic = Import << 1,
        TypeAlias = UsingStatic << 1,
        Enumeration = TypeAlias << 1,
        Attribute = Enumeration << 1,
        FunctionOrEventHandler = Attribute << 1,
        Declaration = FunctionOrEventHandler << 1,
        Assignment = Declaration << 1
    }
}

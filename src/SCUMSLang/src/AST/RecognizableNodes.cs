using System;

namespace SCUMSLang.AST
{
    [Flags]
    public enum RecognizableNodes
    {
        Import = 1 << 0,
        TypeAlias = Import << 1,
        Enumeration = TypeAlias << 1,
        Attribute = Enumeration << 1,
        FunctionOrEventHandler = Attribute << 1,
        Declaration = FunctionOrEventHandler << 1,
        Assignment = Declaration << 1
    }
}

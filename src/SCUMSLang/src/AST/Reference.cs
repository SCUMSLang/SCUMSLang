using System;

namespace SCUMSLang.AST
{
    public abstract class Reference
    {
        public abstract TreeTokenType ReferenceType { get; }

        public override bool Equals(object? obj) => 
            obj is Reference node && ReferenceType == node.ReferenceType;

        public override int GetHashCode() =>
            HashCode.Combine(ReferenceType);
    }
}

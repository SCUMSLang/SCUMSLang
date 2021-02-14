using System;

namespace SCUMSLang.AST
{
    public abstract class Reference
    {
        public abstract TreeTokenType TokenType { get; }

        public override bool Equals(object? obj) => 
            obj is Reference node && TokenType == node.TokenType;

        public override int GetHashCode() =>
            HashCode.Combine(TokenType);
    }
}

using SCUMSLang.AST;
using System;

namespace SCUMSLang.Tokenization
{
    public class TokenTypeTools
    {
        internal static NodeValueType GetNodeValueType(TokenType tokenType)
        {
            var tokenTypeName = Enum.GetName(TokenTypeLibrary.TypeOfTokenType, tokenType)!;
            var memberInfo = TokenTypeLibrary.TypeOfTokenType.GetField(tokenTypeName)!;

            var nodeValueTypeAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(NodeValueTypeAttribute)) as NodeValueTypeAttribute
                ?? throw new ArgumentException("Token type is not a node value type.");

            return nodeValueTypeAttribute.ValueType;
        }
    }
}

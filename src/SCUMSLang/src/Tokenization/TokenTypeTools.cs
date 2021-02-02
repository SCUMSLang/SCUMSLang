using SCUMSLang.AST;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.Tokenization
{
    public class TokenTypeTools
    {
        internal static bool TryGetNodeValueType(TokenType tokenType, [NotNullWhen(true)] out NodeValueType? valueType)
        {
            var tokenTypeName = Enum.GetName(TokenTypeLibrary.TypeOfTokenType, tokenType)!;
            var memberInfo = TokenTypeLibrary.TypeOfTokenType.GetField(tokenTypeName)!;

            var nodeValueTypeAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(NodeValueTypeAttribute)) as NodeValueTypeAttribute;

            if (nodeValueTypeAttribute is null) {
                valueType = null;
                return false;
            }

            valueType = nodeValueTypeAttribute.ValueType;
            return true;
        }

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

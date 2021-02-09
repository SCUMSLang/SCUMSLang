using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class EnumerationMemberNode : TypeDefinitionNode, INameReservableNode
    {
        public override DefinitionType DefinitionType => DefinitionType.EnumerationMember;
        public EnumerationDefinitionNode Enumeration { get; }
        public int Value { get; }

        public EnumerationMemberNode(EnumerationDefinitionNode enumeration, string name, int value)
            : base(name)
        {
            Enumeration = enumeration;
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is EnumerationMemberNode member)) {
                return false;
            }

            var equals = Name == member.Name
                && DefinitionType == member.DefinitionType
                && Value == member.Value;

            Trace.WriteLineIf(!equals, $"{nameof(EnumerationMemberNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(Name, Value);
    }
}

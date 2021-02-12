using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class EnumerationMemberReference : TypeDefinition
    {
        public override SystemType SystemType => SystemType.EnumerationMember;
        public EnumerationTypeReference Enumeration { get; }
        public int Value { get; }

        public EnumerationMemberReference(EnumerationTypeReference enumeration, string name, int value)
            : base(name)
        {
            Enumeration = enumeration;
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is EnumerationMemberReference member)) {
                return false;
            }

            var equals = Name == member.Name
                && SystemType == member.SystemType
                && Value == member.Value;

            Trace.WriteLineIf(!equals, $"{nameof(EnumerationMemberReference)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(Name, Value);
    }
}

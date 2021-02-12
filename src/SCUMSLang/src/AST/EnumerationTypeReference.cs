using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EnumerationTypeReference : TypeDefinition, INamesReservableReference
    {
        public IReadOnlyDictionary<string, EnumerationMemberReference> Members => members;
        public override SystemType SystemType { get => systemType; }
        /// <summary>
        /// Specified whether the types are accessible without enum type.
        /// </summary>
        public bool HasReservedNames { get; }

        private SystemType systemType;

        private int currentMemberIndex;
        private Dictionary<string, EnumerationMemberReference> members;

        public EnumerationTypeReference(string name, bool hasReservedNames, IReadOnlyList<string> memberNames)
            : base(name)
        {
            systemType = SystemType.Enumeration;
            members = new Dictionary<string, EnumerationMemberReference>();
            HasReservedNames = hasReservedNames;

            foreach (var memberName in memberNames) {
                AddMember(memberName);
            }
        }

        internal EnumerationTypeReference(string name, bool hasReservedNames, IReadOnlyList<string> memberNames, SystemType definitionType)
            : this(name, hasReservedNames, memberNames) =>
            this.systemType = definitionType;

        IEnumerable<Reference> INamesReservableReference.GetReservedNames() =>
            members.Values.Select(x => (Reference)x);

        /// <exception cref="ArgumentException">Member already exists</exception>
        protected void AddMember(string memberName)
        {
            if (members.ContainsKey(memberName)) {
                throw new ArgumentException($"The enumeration {Name} has already a member called '{memberName}'.");
            }

            var memberIndex = currentMemberIndex++;
            members.Add(memberName, new EnumerationMemberReference(this, memberName, memberIndex));
        }

        public EnumerationMemberReference GetMemberByName(string name)
        {
            foreach (var member in Members.Values) {
                if (member.Name == name) {
                    return member;
                }
            }

            throw new ArgumentException($"The enumeration '{Name}' does not have a member called '{name}'.");
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is EnumerationTypeReference enumeration)) {
                return false;
            }

            var equals = SystemType == enumeration.SystemType
                && Name == enumeration.Name
                && HasReservedNames == enumeration.HasReservedNames
                && Enumerable.SequenceEqual(Members, enumeration.Members);

            Trace.WriteLineIf(!equals, $"{nameof(EnumerationTypeReference)} not equals.");
            return equals;
        }

        public override bool IsSubsetOf(object? obj)
        {
            if (obj is EnumerationMemberReference otherEnumerationMember) {
                var equals = Members.TryGetValue(otherEnumerationMember.Name, out var enumerationMember)
                    && enumerationMember.Equals(otherEnumerationMember);

                Trace.WriteLineIf(!equals, $"{nameof(EnumerationTypeReference)} is not subset of {nameof(EnumerationMemberReference)}.");
                return equals;
            } else {
                return Equals(obj);
            }
        }

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), ReferenceType, Members, Name);
    }
}

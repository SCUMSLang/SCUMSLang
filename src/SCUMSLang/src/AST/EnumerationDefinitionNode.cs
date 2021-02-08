using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EnumerationDefinitionNode : TypeDefinitionNode, IHasReservedNames
    {
        public IReadOnlyDictionary<string, EnumerationMemberNode> Members => members;
        public override InBuiltType Type => InBuiltType.Enumeration;
        /// <summary>
        /// Specified whether the types are accessible without enum type.
        /// </summary>
        public bool HasReservedNames { get; }

        private int currentMemberIndex;
        private Dictionary<string, EnumerationMemberNode> members;

        public EnumerationDefinitionNode(string name, bool hasReservedNames)
            : base(name)
        {
            members = new Dictionary<string, EnumerationMemberNode>();
            HasReservedNames = hasReservedNames;
        }

        public EnumerationDefinitionNode(string name, bool hasReservedNames, IReadOnlyList<string> memberNames)
            : this(name, hasReservedNames)
        {
            foreach (var memberName in memberNames) {
                AddMember(memberName);
            }
        }

        IEnumerable<(string, Node)> IHasReservedNames.GetReservedNames() =>
            members.Values.Select(x => (x.Name, (Node)x));

        /// <exception cref="ArgumentException">Member already exists</exception>
        internal void AddMember(string memberName)
        {
            if (members.ContainsKey(memberName)) {
                throw new ArgumentException($"The enumeration {Name} has already a member called '{memberName}'.");
            }

            var memberIndex = currentMemberIndex++;
            members.Add(memberName, new EnumerationMemberNode(this, memberName, memberIndex));
        }

        public EnumerationMemberNode GetMemberByName(string name)
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
            if (!(obj is EnumerationDefinitionNode enumeration)) {
                return false;
            }

            var equals = Type == enumeration.Type
                && Name == enumeration.Name
                && HasReservedNames == enumeration.HasReservedNames
                && Enumerable.SequenceEqual(Members, enumeration.Members);

            Debug.WriteLineIf(!equals, $"{nameof(EnumerationDefinitionNode)} not equals.");
            return equals;
        }

        public override bool IsSubsetOf(object? obj)
        {
            if (obj is EnumerationMemberNode otherEnumerationMember) {
                var equals = Members.TryGetValue(otherEnumerationMember.Name, out var enumerationMember)
                    && enumerationMember.Equals(otherEnumerationMember);

                Debug.WriteLineIf(!equals, $"{nameof(EnumerationDefinitionNode)} is not subset of {nameof(EnumerationMemberNode)}.");
                return equals;
            } else {
                return Equals(obj);
            }
        }

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), NodeType, Members, Name);
    }
}

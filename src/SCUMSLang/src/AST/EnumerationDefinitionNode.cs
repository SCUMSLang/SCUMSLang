using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EnumerationDefinitionNode : TypeDefinitionNode, INamesReservableNode
    {
        public IReadOnlyDictionary<string, EnumerationMemberNode> Members => members;
        public override DefinitionType DefinitionType { get => definitionType; }
        /// <summary>
        /// Specified whether the types are accessible without enum type.
        /// </summary>
        public bool HasReservedNames { get; }

        private DefinitionType definitionType;

        private int currentMemberIndex;
        private Dictionary<string, EnumerationMemberNode> members;

        public EnumerationDefinitionNode(string name, bool hasReservedNames, IReadOnlyList<string> memberNames)
            : base(name)
        {
            definitionType = DefinitionType.Enumeration;
            members = new Dictionary<string, EnumerationMemberNode>();
            HasReservedNames = hasReservedNames;

            foreach (var memberName in memberNames) {
                AddMember(memberName);
            }
        }

        internal EnumerationDefinitionNode(string name, bool hasReservedNames, IReadOnlyList<string> memberNames, DefinitionType definitionType)
            : this(name, hasReservedNames, memberNames) =>
            this.definitionType = definitionType;

        IEnumerable<Node> INamesReservableNode.GetReservedNames() =>
            members.Values.Select(x => (Node)x);

        /// <exception cref="ArgumentException">Member already exists</exception>
        protected void AddMember(string memberName)
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

            var equals = DefinitionType == enumeration.DefinitionType
                && Name == enumeration.Name
                && HasReservedNames == enumeration.HasReservedNames
                && Enumerable.SequenceEqual(Members, enumeration.Members);

            Trace.WriteLineIf(!equals, $"{nameof(EnumerationDefinitionNode)} not equals.");
            return equals;
        }

        public override bool IsSubsetOf(object? obj)
        {
            if (obj is EnumerationMemberNode otherEnumerationMember) {
                var equals = Members.TryGetValue(otherEnumerationMember.Name, out var enumerationMember)
                    && enumerationMember.Equals(otherEnumerationMember);

                Trace.WriteLineIf(!equals, $"{nameof(EnumerationDefinitionNode)} is not subset of {nameof(EnumerationMemberNode)}.");
                return equals;
            } else {
                return Equals(obj);
            }
        }

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), NodeType, Members, Name);
    }
}

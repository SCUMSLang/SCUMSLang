using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class TypeDefinitionNode : Node, INameReservableNode, INameDuplicationHandleableNode
    {
        public override NodeType NodeType => NodeType.TypeDefinition;

        public string Name { get; }
        public virtual DefinitionType DefinitionType { get; }

        internal bool AllowOverwriteOnce { get; set; }

        public virtual TypeDefinitionNode SourceType => this;

        protected TypeDefinitionNode(string name) =>
            Name = name ?? throw new ArgumentNullException(nameof(name));

        public TypeDefinitionNode(string name, DefinitionType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DefinitionType = type;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is TypeDefinitionNode node)) {
                return false;
            }

            var equals = NodeType == node.NodeType
                && Name == node.Name
                && DefinitionType == node.DefinitionType;

            Trace.WriteLineIf(!equals, $"{nameof(TypeDefinitionNode)} not equals.");
            return equals;
        }

        public virtual bool IsSubsetOf(object? obj) =>
            Equals(obj);

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), NodeType, Name, DefinitionType);

        #region IConditionalNameReservableNode

        protected virtual ConditionalNameReservationResult CanReserveName(BlockNode blockNode) {
            var candidates = blockNode.GetCastedNodesByName<TypeDefinitionNode>(Name);

            if (candidates is null) {
                return ConditionalNameReservationResult.True;
            } else if (candidates.Count == 1) {
                var candidate = candidates[0];

                if (candidate.AllowOverwriteOnce && candidate.Equals(this)) {
                    AllowOverwriteOnce = false;
                    return ConditionalNameReservationResult.Skip;
                } else {
                    return ConditionalNameReservationResult.False;
                }
            }

            throw new NotImplementedException("More than two type definition with the name have been found that got name reserved without checking.");
        }

        ConditionalNameReservationResult INameDuplicationHandleableNode.CanReserveName(BlockNode blockNode) =>
            CanReserveName(blockNode);

        #endregion
    }
}

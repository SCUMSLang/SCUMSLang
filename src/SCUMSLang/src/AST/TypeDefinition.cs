using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class TypeDefinition : TypeReference, INameReservableReference, INameDuplicationHandleableReference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.TypeDefinition;

        internal bool AllowOverwriteOnce { get; set; }

        public virtual TypeDefinition SourceType => this;

        protected TypeDefinition(string name)
            : base(name) { }

        public TypeDefinition(string name, SystemType systemType)
            : base(name, systemType) { }

        public override bool Equals(object? obj)
        {
            if (!(obj is TypeDefinition node)) {
                return false;
            }

            var equals = ReferenceType == node.ReferenceType
                && Name == node.Name
                && SystemType == node.SystemType;

            Trace.WriteLineIf(!equals, $"{nameof(TypeDefinition)} not equals.");
            return equals;
        }

        public virtual bool IsSubsetOf(object? obj) =>
            Equals(obj);

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), ReferenceType, Name, SystemType);

        #region IConditionalNameReservableNode

        protected virtual ConditionalNameReservationResult CanReserveName(BlockDefinition blockNode) {
            var candidates = blockNode.GetCastedNodesByName<TypeDefinition>(Name);

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

        ConditionalNameReservationResult INameDuplicationHandleableReference.CanReserveName(BlockDefinition blockNode) =>
            CanReserveName(blockNode);

        #endregion
    }
}

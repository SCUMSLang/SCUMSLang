using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class FunctionReference : Reference, INameReservableReference, INameDuplicationHandleableReference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.Function;

        public string Name { get; }
        public IReadOnlyList<DeclarationReference> GenericParameters { get; }
        public IReadOnlyList<DeclarationReference> Parameters { get; }
        public bool IsAbstract { get; }

        public FunctionReference(
            string name,
            IReadOnlyList<DeclarationReference>? genericParameters,
            IReadOnlyList<DeclarationReference>? parameters,
            bool isAbstract = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GenericParameters = genericParameters ?? new List<DeclarationReference>();
            Parameters = parameters ?? new List<DeclarationReference>();
            IsAbstract = isAbstract;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is FunctionReference function)) {
                return false;
            }

            var equals = Enumerable.SequenceEqual(GenericParameters, function.GenericParameters)
                && Enumerable.SequenceEqual(Parameters, function.Parameters)
                && IsAbstract == function.IsAbstract;

            Trace.WriteLineIf(!equals, $"{nameof(FunctionReference)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(ReferenceType, Name, GenericParameters, Parameters, IsAbstract);

        #region IConditionalNameReservableNode

        ConditionalNameReservationResult INameDuplicationHandleableReference.CanReserveName(BlockDefinition block)
        {
            List<FunctionReference>? candidates = block.GetCastedNodesByName<FunctionReference>(Name);

            if (candidates is null || !block.TryGetFirstNode(candidates, this, out _, AbstractionlessFunctionNodeEqualityComparer.Default)) {
                return ConditionalNameReservationResult.True;
            } else {
                throw new ArgumentException("Function with same name and same overload exists already.");
            }
        }

        #endregion
    }
}

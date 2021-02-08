using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class FunctionNode : Node, INameReservedNode
    {
        public override NodeType NodeType => NodeType.Function;

        public string Name { get; }
        public IReadOnlyList<DeclarationNode> GenericParameters { get; }
        public IReadOnlyList<DeclarationNode> Parameters { get; }
        public bool IsAbstract { get; }

        public FunctionNode(
            string name,
            IReadOnlyList<DeclarationNode>? genericParameters,
            IReadOnlyList<DeclarationNode>? parameters,
            bool isAbstract = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GenericParameters = genericParameters ?? new List<DeclarationNode>();
            Parameters = parameters ?? new List<DeclarationNode>();
            IsAbstract = isAbstract;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is FunctionNode function)) {
                return false;
            }

            var equals = Enumerable.SequenceEqual(GenericParameters, function.GenericParameters)
                && Enumerable.SequenceEqual(Parameters, function.Parameters)
                && IsAbstract == function.IsAbstract;

            Debug.WriteLineIf(!equals, $"{nameof(FunctionNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Name, GenericParameters, Parameters, IsAbstract);
    }
}

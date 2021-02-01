using System;
using System.Collections.Generic;
using System.Linq;

namespace SCUMSLang.AST
{
    public class FunctionNode : Node
    {
        public override NodeType NodeType => NodeType.Function;

        public string Name { get; }
        public IReadOnlyList<DeclarationNode> GenericParameters { get; }
        public IReadOnlyList<DeclarationNode> Parameters { get; }

        public FunctionNode(string name, IReadOnlyList<DeclarationNode>? genericParameters, IReadOnlyList<DeclarationNode>? parameters)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GenericParameters = genericParameters ?? new List<DeclarationNode>();
            Parameters = parameters ?? new List<DeclarationNode>();
        }

        public override bool Equals(object? obj) =>
            obj is FunctionNode function
            && Enumerable.SequenceEqual(GenericParameters, function.GenericParameters)
            && Enumerable.SequenceEqual(Parameters, function.Parameters);

        public override int GetHashCode() => 
            HashCode.Combine(NodeType, Name, GenericParameters, Parameters);
    }
}

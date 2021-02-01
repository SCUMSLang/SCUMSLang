using System;
using System.Collections.Generic;

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
    }
}

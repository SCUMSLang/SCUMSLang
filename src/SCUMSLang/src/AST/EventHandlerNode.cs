using System;
using System.Collections.Generic;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EventHandlerNode : FunctionNode
    {
        public override NodeType NodeType => NodeType.EventHandler;
        public List<FunctionCallNode> Conditions { get; }

        public EventHandlerNode(string name, IReadOnlyList<DeclarationNode>? genericParameters, IReadOnlyList<DeclarationNode>? parameters, List<FunctionCallNode> conditions)
            : base(name, genericParameters, parameters) =>
            Conditions = conditions ?? new List<FunctionCallNode>();

        public override bool Equals(object? obj) =>
            obj is EventHandlerNode node
            && base.Equals(obj)
            && Enumerable.SequenceEqual(Conditions, node.Conditions);

        public override int GetHashCode() => 
            HashCode.Combine(base.GetHashCode(), Conditions);
    }
}

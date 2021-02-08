﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EventHandlerNode : FunctionNode
    {
        public override NodeType NodeType => NodeType.EventHandler;
        public IReadOnlyList<FunctionCallNode> Conditions { get; }

        public EventHandlerNode(
            string name,
            IReadOnlyList<DeclarationNode>? genericParameters, 
            IReadOnlyList<DeclarationNode>? parameters,
            IReadOnlyList<FunctionCallNode>? conditions)
            : base(name, genericParameters, parameters) =>
            Conditions = conditions ?? new List<FunctionCallNode>();

        public override bool Equals(object? obj)
        {
            if (!(obj is EventHandlerNode node)) {
                return false;
            }

            var equals = base.Equals(obj)
                && Enumerable.SequenceEqual(Conditions, node.Conditions);

            Debug.WriteLineIf(!equals, $"{nameof(EventHandlerNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() => 
            HashCode.Combine(base.GetHashCode(), Conditions);
    }
}

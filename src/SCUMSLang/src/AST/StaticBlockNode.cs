using System;
using System.Collections.Generic;
using System.Linq;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public partial class StaticBlockNode : BlockNode
    {
        public override Scope Scope => Scope.Static;
        public override BlockNode StaticBlock => this;

        protected override LinkedBucketList<string, Node> NamedNodes { get; }

        public StaticBlockNode() =>
            NamedNodes = new LinkedBucketList<string, Node>(EqualityComparer<string>.Default);

        public void AddFunction(FunctionNode function)
        {
            List<FunctionNode>? candidates;

            if (TryGetNodes(function.Name, out var nodes)) {
                candidates = nodes.Cast<FunctionNode>().ToList();

                if (nodes.Count != nodes.Count) {
                    throw new ArgumentException($"A programming structure of type {Enum.GetName(typeof(NodeType), candidates[0].NodeType)} with the name {function.Name} exists already.");
                }
            } else {
                candidates = null;
            }

            if (candidates is null || !TryGetFirstNode(candidates, function, out _, AbstractionlessFunctionNodeEqualityComparer.Default)) {
                AddNode(function);
                NamedNodes.AddLast(function.Name, function);
            } else {
                throw new ArgumentException("Function with same name and same overload exists already.");
            }
        }

        /// <summary>
        /// Begins a block in <see cref="BlockNode.CurrentBlock"/>.
        /// </summary>
        /// <param name="function"></param>
        public virtual void BeginBlock(FunctionNode function)
        {
            var functionBlock = new FunctionBlockNode(this, function);
            AddFunction(function);
            BeginBlock(functionBlock);
        }
    }
}

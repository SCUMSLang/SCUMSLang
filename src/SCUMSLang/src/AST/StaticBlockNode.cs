using System;
using System.Collections.Generic;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public partial class StaticBlockNode : BlockNode
    {
        public override Scope Scope => Scope.Static;
        public override BlockNode StaticBlock => this;

        protected override LinkedBucketList<string, Node> ReservedNames { get; }
        protected override Dictionary<InBuiltType, TypeDefinitionNode> InBuiltTypeDefinitions { get; }

        public StaticBlockNode()
        {
            ReservedNames = new LinkedBucketList<string, Node>(EqualityComparer<string>.Default);
            InBuiltTypeDefinitions = new Dictionary<InBuiltType, TypeDefinitionNode>();
        }

        public void AddFunction(FunctionNode function)
        {
            List<FunctionNode>? candidates = GetCastedNodesByName<FunctionNode>(function.Name);

            if (candidates is null || !TryGetFirstNode(candidates, function, out _, AbstractionlessFunctionNodeEqualityComparer.Default)) {
                AddNode(function);
                ReservedNames.AddLast(function.Name, function);
            } else {
                throw new ArgumentException("Function with same name and same overload exists already.");
            }
        }

        /// <summary>
        /// Adds type defition to reserved nodes. Disallows any kind of duplication.
        /// </summary>
        /// <param name="typeDefinition"></param>
        public void AddTypeDefintion(TypeDefinitionNode typeDefinition)
        {
            AddNonCrossBlockNameReservedNode(typeDefinition);
            InBuiltTypeDefinitions[typeDefinition.Type] = typeDefinition;

            if (typeDefinition is EnumerationDefinitionNode enumerationDefinition) {
                AddNonCrossBlockReservedNames(enumerationDefinition);
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

﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class MethodDefinition : MethodReference, ICollectibleMember, IOverloadableReference, IBlockHolder, IAttributesHolder
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.MethodDefinition;
        public bool IsAbstract { get; set; }
        public BlockDefinition? Body => body;
        public IList<AttributeDefinition> Attributes { get; private set; }

        private BlockDefinition? body;
        bool IBlockHolder.IsBlockOwnable => !IsAbstract;
        BlockDefinition? IBlockHolder.Block => Body;

        public MethodDefinition(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            TypeReference declaringType)
            : base(name, genericParameters, parameters, declaringType) =>
            OnConstruction();

        public MethodDefinition(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters)
            : base(name, genericParameters, parameters) =>
            OnConstruction();

        [MemberNotNull(nameof(Attributes))]
        private void OnConstruction() =>
            Attributes = new List<AttributeDefinition>();

        void IBlockHolder.SetupBlock(BlockDefinition parentBlock) =>
            BlockHolders.SetupBlock(ref body, parentBlock, BlockScope.Local);

        public new MethodDefinition Resolve() =>
            this;

        protected override IMember ResolveMember() =>
            Resolve();

        OverloadConflictResult IOverloadableReference.SolveConflict(BlockDefinition block)
        {
            List<MethodReference>? candidates = block.BlocksMembersByName<MethodReference>(Name);

            if (candidates is null || !block.TryGetFirstOfMembers(candidates, this, out _, MethodReferenceEqualityComparer.Default)) {
                return OverloadConflictResult.True;
            } else {
                throw new ArgumentException("Function with same name and same overload exists already.");
            }
        }

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitMethodReference(this);

        public MethodDefinition UpdateDefinition(
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters)
        {
            if (ReferenceEquals(genericParameters, GenericParameters)
                && ReferenceEquals(parameters, Parameters)) {
                return this;
            }

            return new MethodDefinition(Name, genericParameters, parameters) {
                DeclaringType = DeclaringType,
                ParentBlockContainer = ParentBlockContainer,
                body = body
            };
        }
    }
}

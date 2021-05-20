using System;
using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class MethodDefinition : MethodReference, IMemberDefinition, IOverloadableReference, IBlockHolder
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.MethodDefinition;

        public bool IsAbstract { get; set; }
        public BlockDefinition? Block { get; internal set; }

        bool IBlockHolder.IsExpandable =>
            IsAbstract;

        BlockDefinition? IBlockHolder.Block {
            get => Block;
            set => Block = value;
        }

        public MethodDefinition(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            TypeReference declaringType)
            : base(name, genericParameters, parameters, declaringType) { }

        public MethodDefinition(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters)
            : base(name, genericParameters, parameters) { }

        public new MethodDefinition Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
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

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
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
                Module = Module
            };
        }
    }
}

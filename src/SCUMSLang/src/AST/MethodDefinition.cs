using System;
using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class MethodDefinition : MethodReference, IMemberDefinition, IOverloadableReference, IBlockHolder
    {
        public override TreeTokenType TokenType => TreeTokenType.MethodDefinition;
        public bool IsAbstract { get; set; }
        public BlockDefinition? Block { get; internal set; }

        bool IBlockHolder.IsExandable => IsAbstract;

        BlockDefinition? IBlockHolder.Block {
            get => Block;
            set => Block = value;
        }

        public MethodDefinition(
            string name,
            IReadOnlyList<ParameterDefinition> genericParameters,
            IReadOnlyList<ParameterDefinition> parameters,
            TypeReference declaringType)
            : base(name, genericParameters, parameters, declaringType) { }

        protected override IMemberDefinition ResolveDefinition() => 
            Resolve();

        public new MethodDefinition Resolve()
        {
            ResolveDependencies();
            return this;
        }

        OverloadConflictResult IOverloadableReference.SolveConflict(BlockDefinition block)
        {
            List<MethodReference>? candidates = block.GetMembersCasted<MethodReference>(Name);

            if (candidates is null || !block.TryGetMemberFirst(candidates, this, out _, MethodReferenceEqualityComparer.Default)) {
                return OverloadConflictResult.True;
            } else {
                throw new ArgumentException("Function with same name and same overload exists already.");
            }
        }
    }
}

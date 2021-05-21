using SCUMSLang.SyntaxTree.Visitors;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public abstract partial class BlockDefinition
    {
        public sealed class ScopedBlockDefinition : BlockDefinition
        {
            public override BlockScope BlockScope =>
                blockScope;

            public override ModuleDefinition Module =>
                ParentBlock.Module;

            internal protected override LinkedBucketList<string, TypeReference> ModuleTypeList =>
                Module.Block.ModuleTypeList;

            protected override BlockDefinition ParentBlock =>
                parentBlock;

            private readonly BlockDefinition parentBlock;
            private readonly BlockScope blockScope;

            internal ScopedBlockDefinition(BlockDefinition parentBlock, BlockScope blockScope)
            {
                this.parentBlock = parentBlock;
                this.blockScope = blockScope;
            }

            protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
                visitor.VisitScopedBlockDefinition(this);
        }
    }
}

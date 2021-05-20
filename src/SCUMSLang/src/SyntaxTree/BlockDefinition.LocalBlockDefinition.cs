using SCUMSLang.SyntaxTree.Visitors;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public abstract partial class BlockDefinition
    {
        public sealed class LocalBlockDefinition : BlockDefinition
        {
            public override BlockScope BlockScope =>
                BlockScope.Local;

            public override ModuleDefinition Module =>
                ParentBlock.Module;

            internal protected override LinkedBucketList<string, TypeReference> ModuleTypeList =>
                Module.Block.ModuleTypeList;

            protected override BlockDefinition ParentBlock =>
                parentBlock;

            private readonly BlockDefinition parentBlock;
            private readonly IBlockHolder blockHolder;

            internal LocalBlockDefinition(BlockDefinition parentBlock, IBlockHolder blockHolder)
            {
                this.parentBlock = parentBlock;
                this.blockHolder = blockHolder;
            }

            protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
                visitor.VisitLocalBlockDefinition(this);
        }
    }
}

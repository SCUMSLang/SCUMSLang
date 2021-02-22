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

            public override string Name => blockHolder.Name;

            public override TypeReference DeclaringType =>
                blockHolder.DeclaringType;

            internal protected override LinkedBucketList<string, TypeReference> CascadingTypes =>
                Module.Block.CascadingTypes;

            protected override BlockDefinition ParentBlock =>
                parentBlock;

            private readonly BlockDefinition parentBlock;
            private readonly IBlockHolder blockHolder;

            internal LocalBlockDefinition(BlockDefinition parentBlock, IBlockHolder blockHolder)
            {
                this.parentBlock = parentBlock;
                this.blockHolder = blockHolder;
            }
        }
    }
}

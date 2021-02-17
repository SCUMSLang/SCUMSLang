using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public abstract partial class BlockDefinition
    {
        public sealed class LocalBlockDefinition : BlockDefinition
        {
            public override Scope Scope =>
                Scope.Local;

            public override ModuleDefinition Module =>
                ParentBlock.Module;

            public override string Name => blockHolder.Name;

            protected override BlockDefinition ParentBlock =>
                parentBlock;

            protected override LinkedBucketList<string, TypeReference> ModuleTypes =>
                Module.Block.ModuleTypes;

            public override TypeReference DeclaringType =>
                blockHolder.DeclaringType;

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

using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree
{
    internal class ParentBlockFeature : IHasParentBlock
    {
        [AllowNull]
        public BlockContainer ParentBlockContainer {
            get => parentBlockContainer ??= new BlockContainer();
            set => parentBlockContainer = value;
        }

        [AllowNull]
        public BlockDefinition ParentBlock {
            get => AsParentBlock(ParentBlockContainer.Block);
            set => ParentBlockContainer.Block = value;
        }

        [AllowNull]
        BlockDefinition IHasParentBlock.ParentBlock {
            get => ParentBlock;
            set => ParentBlock = value;
        }

        [MemberNotNullWhen(true, nameof(ParentBlock))]
        public virtual bool HasParentBlock =>
            ParentBlockContainer.HasBlock;

        private BlockContainer? parentBlockContainer;
        private readonly Reference owner;

        public ParentBlockFeature(Reference owner) =>
            this.owner = owner;

        public BlockDefinition AsParentBlock(BlockDefinition? block) =>
            block ?? throw SyntaxTreeThrowHelper.InvalidOperation(owner);
    }
}

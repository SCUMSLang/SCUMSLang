using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree
{
    public class BlockContainer
    {
        public static BlockContainer WithBlock(BlockDefinition block) =>
            new BlockContainer() { Block = block };

        public BlockDefinition? Block { get; set; }

        [MemberNotNullWhen(true, nameof(Block))]
        public virtual bool HasBlock =>
            Block is not null;
    }
}

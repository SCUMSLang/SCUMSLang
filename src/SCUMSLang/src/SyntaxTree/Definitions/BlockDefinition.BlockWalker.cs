using System;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public partial class BlockDefinition
    {
        public class BlockWalker
        {
            public BlockWalker(BlockDefinition block) =>
                CurrentBlock = block ?? throw new ArgumentNullException(nameof(block));

            /// <summary>
            /// It is the current begun contextual block in THIS block.
            /// At first it will be THIS instance. But when another block
            /// gets created in this block, the current contextual block
            /// is that block.
            /// Only relevant when constructing this and nested blocks
            /// with new references and definitions.
            /// </summary>
            public BlockDefinition CurrentBlock { get; private set; }

            public void EndCurrentBlock()
            {
                if (ReferenceEquals(CurrentBlock, CurrentBlock.ParentBlock)) {
                    throw new BlockEvaluationException("You cannot end the block of the root block.");
                }

                CurrentBlock = CurrentBlock.ParentBlock;
            }

            public bool TrySetParentBlock(Reference reference)
            {
                if (reference is IOwnableReference ownableReference) {
                    ownableReference.ParentBlock = CurrentBlock;
                    return true;
                }

                return false;
            }

            public bool TryBeginAnotherBlock(Reference reference)
            {
                if (!(reference is IBlockHolder blockHolder && blockHolder.IsBlockOwnable && blockHolder.Block is not null)) {
                    return false;
                }

                if (!ReferenceEquals(blockHolder.Block.ParentBlock, CurrentBlock)) {
                    throw new BlockEvaluationException("You cannot begin another block whose parent is not the current block.");
                }

                CurrentBlock = blockHolder.Block;
                return true;
            }
        }
    }
}

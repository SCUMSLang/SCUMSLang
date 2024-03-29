﻿using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree
{
    public static class BlockHolders
    {
        public static void SetupBlock(ref BlockDefinition? block, BlockDefinition parentBlock, BlockScope blockScope)
        {
            if (block is not null) {
                throw SyntaxTreeThrowHelper.NonNullBlockSetup();
            }

            block = new BlockDefinition.ScopedBlockDefinition(blockScope) {
                ParentBlockContainer = BlockContainer.WithBlock(parentBlock)
            };
        }
    }
}

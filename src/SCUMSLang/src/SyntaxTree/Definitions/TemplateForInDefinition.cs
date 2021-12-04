using System.Collections.Generic;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class TemplateForInDefinition : Reference, IBlockHolder
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TemplateForInDefinition;

        public IReadOnlyList<ForInDefinition> ForInCollection { get; }

        public bool IsBlockOwnable =>
            true;

        public BlockDefinition? Block =>
            block;

        private BlockDefinition? block;

        BlockDefinition? IBlockHolder.Block {
            get => block;
        }

        public TemplateForInDefinition(IReadOnlyList<ForInDefinition>? forInCollection) =>
            ForInCollection = forInCollection ?? new ForInDefinition[0];

        void IBlockHolder.SetupBlock(BlockDefinition parentBlock) =>
            BlockHolderTools.SetupBlock(ref block, parentBlock, parentBlock.BlockScope);

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitTemplateForInDefinition(this);
    }
}

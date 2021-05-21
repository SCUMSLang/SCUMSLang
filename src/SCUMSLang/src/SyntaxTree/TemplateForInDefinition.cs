using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
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
            set => block = value;
        }

        public TemplateForInDefinition(IReadOnlyList<ForInDefinition>? forInCollection) =>
            ForInCollection = forInCollection ?? new ForInDefinition[0];

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitTemplateForInDefinition(this);
    }
}

using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class TemplateForInDefinition : Reference, IBlockHolder
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TemplateForInDefinition;

        public IReadOnlyList<ForInDefinition> ForInCollection { get; }

        public bool IsExandable => throw new System.NotImplementedException();
        public BlockDefinition? Block { get => block; }

        public TypeReference DeclaringType => throw new System.NotImplementedException();

        private BlockDefinition? block;

        BlockDefinition? IBlockHolder.Block { 
            get => block;
            set => block = value;
        }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitTemplateForInDefinition(this);
    }
}

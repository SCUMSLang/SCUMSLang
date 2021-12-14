using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public abstract partial class BlockDefinition
    {
        public sealed class ScopedBlockDefinition : BlockDefinition
        {
            public override BlockScope BlockScope { get; }
            public override ModuleDefinition Module => ParentBlock.Module;

            internal ScopedBlockDefinition(BlockScope blockScope) => 
                BlockScope = blockScope;

            protected internal override Reference Accept(NodeVisitor visitor) =>
                visitor.VisitScopedBlockDefinition(this);
        }
    }
}

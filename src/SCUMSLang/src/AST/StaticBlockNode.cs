using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public partial class StaticBlockNode : BlockNode
    {
        public override Scope Scope => Scope.Static;
        public override BlockNode StaticBlock => this;

        protected override Dictionary<string, Node> NamedNodes { get; }

        public StaticBlockNode() =>
            NamedNodes = new Dictionary<string, Node>();

        public virtual void BeginBlock(FunctionNode function)
        {
            var functionBlock = new FunctionBlockNode(this, function);
            CurrentBlock = functionBlock;
            AddNode(functionBlock);
        }
    }
}

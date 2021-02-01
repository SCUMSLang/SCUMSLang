namespace SCUMSLang.AST
{
    public class FunctionBlockNode : BlockNode.LocalBlockNode
    {
        public FunctionBlockNode(BlockNode parent, FunctionNode owner)
            : base(parent, owner)
        {
            foreach (var parameter in owner.Parameters) {
                AddDeclaration(parameter);
            }
        }
    }
}

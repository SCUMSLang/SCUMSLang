namespace SCUMSLang.AST
{
    public class FunctionBlockDefinition : BlockDefinition.LocalBlockDefinition
    {
        public FunctionBlockDefinition(BlockDefinition parent, FunctionReference owner)
            : base(parent, owner)
        {
            foreach (var parameter in owner.Parameters) {
                AddNode(parameter);
            }
        }
    }
}

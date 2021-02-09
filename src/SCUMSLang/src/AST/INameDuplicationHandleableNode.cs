namespace SCUMSLang.AST
{
    internal interface INameDuplicationHandleableNode
    {
        public bool HandleNameDuplication(BlockNode blockNode);
    }
}

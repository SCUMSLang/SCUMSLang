namespace SCUMSLang.AST
{
    internal interface INameDuplicationHandleableNode
    {
        /// <summary>
        /// Checks whether the duplicate node can be added.
        /// </summary>
        /// <param name="blockNode"></param>
        /// <param name="probe"></param>
        /// <returns></returns>
        public ConditionalNameReservationResult CanReserveName(BlockNode blockNode);
    }
}

namespace SCUMSLang.AST
{
    internal interface INameDuplicationHandleableReference
    {
        /// <summary>
        /// Checks whether the duplicate node can be added.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="probe"></param>
        /// <returns></returns>
        public ConditionalNameReservationResult CanReserveName(BlockDefinition block);
    }
}

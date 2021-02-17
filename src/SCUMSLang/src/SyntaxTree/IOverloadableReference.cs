namespace SCUMSLang.SyntaxTree
{
    internal interface IOverloadableReference
    {
        /// <summary>
        /// Checks whether the duplicate node can be added.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="probe"></param>
        /// <returns></returns>
        public OverloadConflictResult SolveConflict(BlockDefinition block);
    }
}

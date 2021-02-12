namespace SCUMSLang.AST
{
    public partial class StaticBlockNode : BlockNode
    {
        public override Scope Scope => Scope.Static;
        public override StaticBlockNode StaticBlock => this;
        public string WorkingDirectory { get; }

        internal protected override NameReservableNodePool NameReservableNodes => nameReservableNodes;

        internal NameReservableNodePool nameReservableNodes = null!;

        public StaticBlockNode(NameReservableNodePool? nameReservableNodes, string? workingDirectory)
        {
            this.nameReservableNodes = nameReservableNodes ?? new NameReservableNodePool();
            WorkingDirectory = workingDirectory ?? string.Empty;
        }

        public StaticBlockNode(NameReservableNodePool? nameReservableNodes)
            : this(nameReservableNodes, null) { }

        public StaticBlockNode()
            : this(null, null) { }

        /// <summary>
        /// Begins a block in <see cref="BlockNode.CurrentBlock"/>.
        /// </summary>
        /// <param name="function"></param>
        public virtual void BeginBlock(FunctionNode function)
        {
            var functionBlock = new FunctionBlockNode(this, function);
            AddNode(function);
            BeginBlock(functionBlock);
        }
    }
}

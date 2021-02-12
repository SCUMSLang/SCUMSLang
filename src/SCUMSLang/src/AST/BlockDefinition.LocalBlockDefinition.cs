namespace SCUMSLang.AST
{
    public abstract partial class BlockDefinition
    {
        public abstract class LocalBlockDefinition : BlockDefinition
        {
            public override Scope Scope => Scope.Local;
            public override ModuleDefinition Module => Parent.Module;
            public Reference Owner { get; }

            internal protected override NameReservableNodePool NameReservableNodes => Parent.NameReservableNodes;

            public LocalBlockDefinition(BlockDefinition parent, Reference owner)
                : base(parent) =>
                Owner = owner;
        }
    }
}

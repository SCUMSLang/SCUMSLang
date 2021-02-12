using System.IO;

namespace SCUMSLang.AST
{
    public sealed partial class ModuleDefinition : BlockDefinition
    {
        public static ModuleDefinition Create(ModuleParameters? parameters) {
            parameters = parameters ?? new ModuleParameters();

            return new ModuleDefinition() {
                nameReservableDefinitions = parameters.NameReservableDefinitions ?? new NameReservableNodePool(),
                FilePath = parameters.FilePath ?? string.Empty
            };
        }

        public static ModuleDefinition Create() =>
            Create(default(ModuleParameters));

        public override Scope Scope => Scope.Static;
        public override ModuleDefinition Module => this;
        public string FilePath { get; private set; } = null!;
        public string? DirectoryName => Path.GetDirectoryName(FilePath);

        internal protected override NameReservableNodePool NameReservableNodes => nameReservableDefinitions;

        private IMemberResolver? memberResolver;

        internal NameReservableNodePool nameReservableDefinitions = null!;

        private ModuleDefinition() { }

        /// <summary>
        /// Begins a block in <see cref="BlockDefinition.CurrentBlock"/>.
        /// </summary>
        /// <param name="function"></param>
        public void BeginBlock(FunctionReference function)
        {
            var functionBlock = new FunctionBlockDefinition(this, function);
            AddNode(function);
            BeginBlock(functionBlock);
        }

        public TypeDefinition? Resolve(TypeReference type) =>
            memberResolver?.Resolve(type);

        public DeclarationDefinition? Resolve(DeclarationReference declaration) =>
            memberResolver?.Resolve(declaration);
    }
}

using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public sealed partial class ModuleDefinition : TypeReference, IMemberDefinition
    {
        public TypeBlockDefinition Block => block;
        public override BlockDefinition ParentBlock => Block;
        /// <summary>
        /// The file path of the module. Empty if not known.
        /// </summary>
        public string FilePath { get; }
        /// <summary>
        /// Resolves the references that have been imported or are exclusively
        /// part of this block; in this order.
        /// If not found the reference resolver specified at construction time
        /// is considered too.
        /// </summary>
        public IReferenceResolver ModuleReferenceResolver { get; }
        /// <summary>
        /// Resolves the references that are exclusively part of this block.
        /// If not found the reference resolver specified at construction time
        /// is considered too.
        /// </summary>
        public IReferenceResolver BlockReferenceResolver { get; }

        //internal SyntaxNodeModuleFillingVisitor ModuleFillingVisitor { get; }

        private ModuleBlockDefinition block;

        public ModuleDefinition(ModuleParameters? parameters)
            : base(name: string.Empty)
        {
            block = new ModuleBlockDefinition(this);
            //ModuleFillingVisitor = new SyntaxNodeModuleFillingVisitor(this);
            FilePath = parameters?.FilePath ?? string.Empty;

            if (parameters?.ReferenceResolver is null) {
                ModuleReferenceResolver = block.ModuleReferenceResolver;
                BlockReferenceResolver = block.BlockReferenceResolver;
            } else {
                static ReferenceResolverPool createReferenceResolverPool(IReferenceResolver moduleReferenceResolver, IReferenceResolver parametersReferenceResolver)
                {
                    var referenceResolverPool = new ReferenceResolverPool();
                    referenceResolverPool.Add(moduleReferenceResolver);
                    referenceResolverPool.Add(parametersReferenceResolver);
                    return referenceResolverPool;
                }

                ModuleReferenceResolver = createReferenceResolverPool(
                    moduleReferenceResolver: block.ModuleReferenceResolver,
                    parametersReferenceResolver: parameters.ReferenceResolver);

                BlockReferenceResolver = createReferenceResolverPool(
                    moduleReferenceResolver: block.BlockReferenceResolver,
                    parametersReferenceResolver: parameters.ReferenceResolver);
            }
        }

        public ModuleDefinition()
            : this(parameters: null) { }

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        public void ResolveUsingStaticDirectives() =>
            block.ResolveUsingStaticDirectives();

        public TypeDefinition Resolve(TypeReference type) =>
            ModuleReferenceResolver.Resolve(type);

        public FieldDefinition Resolve(FieldReference field) =>
            ModuleReferenceResolver.Resolve(field);

        public MethodDefinition Resolve(MethodReference method) =>
            ModuleReferenceResolver.Resolve(method);

        public EventHandlerDefinition Resolve(EventHandlerReference method) =>
            ModuleReferenceResolver.Resolve(method);

        //public new ModuleDefinition Resolve()
        //{
        //    var visitor = new SyntaxNodeResolvingVisitor();
        //    visitor.Visit(this); // TODO: VERIFY
        //    return this;
        //}
    }
}

using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public sealed partial class ModuleDefinition : TypeReference, IReferenceResolver, IMemberDefinition
    {
        public TypeBlockDefinition Block => block;
        public override ModuleDefinition Module => this;
        public string FilePath { get; }

        internal SyntaxNodeModuleFillingVisitor ModuleFillingVisitor { get; }

        private ModuleBlockDefinition block;
        private readonly IReferenceResolver moduleExclusiveReferenceResolver;
        private readonly IReferenceResolver referenceResolver;

        public ModuleDefinition(ModuleParameters? parameters)
            : base(name: string.Empty)
        {
            block = new ModuleBlockDefinition(this);
            ModuleFillingVisitor = new SyntaxNodeModuleFillingVisitor(this);
            FilePath = parameters?.FilePath ?? string.Empty;

            if (parameters?.ReferenceResolver is null) {
                moduleExclusiveReferenceResolver = block.ModuleExclusiveReferenceResolver;
                referenceResolver = block;
            } else {
                static ReferenceResolverPool createReferenceResolverPool(IReferenceResolver moduleBlockReferenceResolver, IReferenceResolver parametersReferenceResolver)
                {
                    var referenceResolverPool = new ReferenceResolverPool();
                    referenceResolverPool.Add(moduleBlockReferenceResolver);
                    referenceResolverPool.Add(parametersReferenceResolver);
                    return referenceResolverPool;
                }

                moduleExclusiveReferenceResolver = createReferenceResolverPool(
                    block.ModuleExclusiveReferenceResolver,
                    parameters.ReferenceResolver);

                referenceResolver = createReferenceResolverPool(block, parameters.ReferenceResolver);
            }
        }

        public ModuleDefinition()
            : this(parameters: null) { }

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        public void ResolveUsingStaticDirectives() =>
            block.ResolveUsingStaticDirectives();

        public TypeDefinition Resolve(TypeReference type) =>
            moduleExclusiveReferenceResolver.Resolve(type);

        public FieldDefinition Resolve(FieldReference field) =>
            moduleExclusiveReferenceResolver.Resolve(field);

        public MethodDefinition Resolve(MethodReference method) =>
            moduleExclusiveReferenceResolver.Resolve(method);

        public EventHandlerDefinition Resolve(EventHandlerReference method) =>
            moduleExclusiveReferenceResolver.Resolve(method);

        //public new ModuleDefinition Resolve()
        //{
        //    var visitor = new SyntaxNodeResolvingVisitor();
        //    visitor.Visit(this); // TODO: VERIFY
        //    return this;
        //}

        public new ModuleDefinition Resolve() =>
            this;

        public void Import(MemberReference member) =>
            block.Import(member);

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitModuleDefinition(this);

        #region IReferenceResolver

        TypeDefinition IReferenceResolver.Resolve(TypeReference type) =>
            referenceResolver.Resolve(type);

        FieldDefinition IReferenceResolver.Resolve(FieldReference field) =>
            referenceResolver.Resolve(field);

        MethodDefinition IReferenceResolver.Resolve(MethodReference method) =>
            referenceResolver.Resolve(method);

        EventHandlerDefinition IReferenceResolver.Resolve(EventHandlerReference eventHandler) =>
            referenceResolver.Resolve(eventHandler);

        #endregion
    }
}

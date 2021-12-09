using System;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public sealed class ModuleDefinition : TypeReference, IMemberDefinition
    {
        public TypeBlockDefinition Block => ModuleBlock;
        public override BlockDefinition ParentBlock => Block;
        /// <summary>
        /// The file path of the module. Empty if not known.
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// Resolves the references that have been imported or are exclusively
        /// part of this block; in this order.
        /// If not found the reference resolver specified at construction time
        /// is considered too.
        /// </summary>
        public IReferenceResolver ModuleReferenceResolver { get; private set; }
        /// <summary>
        /// Resolves the references that are exclusively part of this block.
        /// If not found the reference resolver specified at construction time
        /// is considered too.
        /// </summary>
        public IReferenceResolver BlockReferenceResolver { get; private set; }

        internal ModuleBlockDefinition ModuleBlock { get; private set; }

        public ModuleDefinition(ModuleParameters? moduleParameters)
            : base(name: string.Empty) =>
            Initialize(moduleParameters, new ModuleBlockDefinition(this));

        public ModuleDefinition()
            : this(moduleParameters: null) { }

        [MemberNotNull(nameof(FilePath), nameof(ModuleReferenceResolver), nameof(BlockReferenceResolver))]
        private void Initialize(ModuleParameters? moduleParameters, ModuleBlockDefinition moduleBlock)
        {
            ModuleBlock = moduleBlock;
            FilePath = moduleParameters?.FilePath ?? string.Empty;

            if (moduleParameters?.ReferenceResolver is null) {
                ModuleReferenceResolver = moduleBlock.ModuleReferenceResolver;
                BlockReferenceResolver = moduleBlock.BlockReferenceResolver;
            } else {
                static ReferenceResolverPool createReferenceResolverPool(IReferenceResolver moduleReferenceResolver, IReferenceResolver parametersReferenceResolver)
                {
                    var referenceResolverPool = new ReferenceResolverPool();
                    referenceResolverPool.Add(moduleReferenceResolver);
                    referenceResolverPool.Add(parametersReferenceResolver);
                    return referenceResolverPool;
                }

                ModuleReferenceResolver = createReferenceResolverPool(
                    moduleReferenceResolver: moduleBlock.ModuleReferenceResolver,
                    parametersReferenceResolver: moduleParameters.ReferenceResolver);

                BlockReferenceResolver = createReferenceResolverPool(
                    moduleReferenceResolver: moduleBlock.BlockReferenceResolver,
                    parametersReferenceResolver: moduleParameters.ReferenceResolver);
            }
        }

        public new ModuleDefinition Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitModuleDefinition(this);

        public void ResolveUsingStaticDirectives() =>
            ModuleBlock.ResolveUsingStaticDirectives();

        public TypeDefinition Resolve(TypeReference type) =>
            ModuleReferenceResolver.Resolve(type);

        public FieldDefinition Resolve(FieldReference field) =>
            ModuleReferenceResolver.Resolve(field);

        public MethodDefinition Resolve(MethodReference method) =>
            ModuleReferenceResolver.Resolve(method);

        public EventHandlerDefinition Resolve(EventHandlerReference method) =>
            ModuleReferenceResolver.Resolve(method);

        internal ModuleDefinition UpdateDefinition(ModuleBlockDefinition block)
        {
            if (ReferenceEquals(ModuleBlock, block)) {
                return this;
            }

            throw new InvalidOperationException("You cannot update the module");
        }

        public void ResolveRecursively() =>
            SyntaxNodeResolvingVisitor.Default.Visit(this); // TODO: VERIFY
    }
}

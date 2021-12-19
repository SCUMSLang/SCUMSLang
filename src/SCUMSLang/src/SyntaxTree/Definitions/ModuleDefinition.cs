using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public delegate ModuleDefinition? ImportResolver(string filePath);

    public sealed class ModuleDefinition : TypeReference, ICollectibleMember, IReferenceResolver
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ModuleDefinition;

        [AllowNull]
        public override BlockContainer ParentBlockContainer {
            get => parentBlockContainer ??= new ModuleBlockContainer(this);
            init => parentBlockContainer = value;
        }

        public BlockDefinition Block => ModuleBlock;
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
        public ILogger? Logger { get; private set; }
        public ILoggerFactory? LoggerFactory { get; private set; }
        public SystemTypesResolver SystemTypes { get; private set; }

        internal ModuleBlockDefinition ModuleBlock { get; private set; }

        private BlockContainer? parentBlockContainer;

        public ModuleDefinition(ModuleParameters? moduleParameters)
            : base(name: string.Empty) =>
            Initialize(new ModuleBlockDefinition(this), moduleParameters);

        public ModuleDefinition()
            : base(name: string.Empty) =>
            Initialize(new ModuleBlockDefinition(this), moduleParameters: null);

        [MemberNotNull(
            nameof(SystemTypes),
            nameof(ModuleBlock),
            nameof(FilePath),
            nameof(ModuleReferenceResolver),
            nameof(BlockReferenceResolver))]
        private void Initialize(ModuleBlockDefinition moduleBlock, ModuleParameters? moduleParameters)
        {
            SystemTypes = new SystemTypesResolver(this);
            LoggerFactory = moduleParameters?.LoggerFactory;
            Logger = LoggerFactory?.CreateLogger<ModuleDefinition>();
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

        public new ModuleDefinition Resolve() => this;

        protected override IMember ResolveMember() => Resolve();

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitModuleDefinition(this);

        public ResolveResult<T> Resolve<T>(TypeReference type)
            where T : TypeReference =>
            ModuleReferenceResolver.Resolve<T>(type);

        public ResolveResult<TypeReference> Resolve(TypeReference type) =>
            Resolve<TypeReference>(type);

        public ResolveResult<FieldDefinition> Resolve(FieldReference field) =>
            ModuleReferenceResolver.Resolve(field);

        public ResolveResult<MethodDefinition> Resolve(MethodReference method) =>
            ModuleReferenceResolver.Resolve(method);

        public ResolveResult<EventHandlerDefinition> Resolve(EventHandlerReference method) =>
            ModuleReferenceResolver.Resolve(method);

        public ResolveResult<TypeDefinition> GetType(string typeName) =>
            ModuleReferenceResolver.GetType(typeName);

        public ResolveResult<EventHandlerDefinition> GetEventHandler(string eventHandlerName) =>
            ModuleReferenceResolver.GetEventHandler(eventHandlerName);

        public ResolveResult<FieldDefinition> GetField(string fieldName) =>
            ModuleReferenceResolver.GetField(fieldName);

        public ResolveResult<MethodDefinition> GetMethod(string methodName) =>
            ModuleReferenceResolver.GetMethod(methodName);

        public ResolveResult<MethodDefinition> GetMethod(MethodReference methodReference) =>
            ModuleReferenceResolver.GetMethod(methodReference);

        internal ModuleDefinition UpdateDefinition(ModuleBlockDefinition block)
        {
            if (ReferenceEquals(ModuleBlock, block)) {
                return this;
            }

            throw new InvalidOperationException("You cannot update the module");
        }

        private class ModuleBlockContainer : BlockContainer
        {
            public override BlockDefinition? Block {
                get => module.ModuleBlock;
                set => throw new InvalidOperationException("You cannot set the parent block of a module");
            }

            private ModuleDefinition module;

            public ModuleBlockContainer(ModuleDefinition module) =>
                this.module = module;
        }
    }
}

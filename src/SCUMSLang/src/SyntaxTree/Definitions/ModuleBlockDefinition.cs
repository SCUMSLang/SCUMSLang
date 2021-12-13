using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree.Definitions
{
    internal class ModuleBlockDefinition : BlockDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.ModuleBlockDefinition;

        public override BlockScope BlockScope =>
            BlockScope.Module;

        [AllowNull]
        public override BlockContainer ParentBlockContainer {
            get => parentBlockContainer ??= new ModuleBlockContainer(this);
            init => parentBlockContainer = value;
        }

        public override ModuleDefinition Module { get; }

        public IReadOnlyDictionary<string, UsingStaticDirective> UsingStaticDirectives =>
            usingStaticDirectives;

        /// <summary>
        /// Resolves the references that have been imported or are exclusively
        /// part of this block; in this order.
        /// </summary>
        internal IReferenceResolver ModuleReferenceResolver { get; }

        private BlockContainer? parentBlockContainer;
        private ReferenceResolverPool importedModules;
        private ReferenceResolver importedTypesResolver;
        private Dictionary<string, UsingStaticDirective> usingStaticDirectives;
        private HashSet<string> usingStaticDirectiveResolvedElementTypeNames;

        public ModuleBlockDefinition(ModuleDefinition module)
        {
            importedModules = new ReferenceResolverPool();
            importedTypesResolver = new ReferenceResolver();

            var exclusiveReferencePool = new ReferenceResolverPool();
            exclusiveReferencePool.Add(importedTypesResolver);
            exclusiveReferencePool.Add(BlockReferenceResolver);
            exclusiveReferencePool.Add(importedModules);
            ModuleReferenceResolver = exclusiveReferencePool;

            usingStaticDirectiveResolvedElementTypeNames = new HashSet<string>();
            usingStaticDirectives = new Dictionary<string, UsingStaticDirective>();
            Module = module;
        }

        protected override bool TryAddMember(Reference member)
        {
            if (member is UsingStaticDirectiveDefinition usingStaticDirective) {
                if (usingStaticDirectives.ContainsKey(usingStaticDirective.Name)) {
                    throw new NameReservedException(usingStaticDirective.Name, "You cannot add a using-static-directive with same type twice.");
                }

                usingStaticDirectives.Add(usingStaticDirective.Name, usingStaticDirective);
                return true;
            }

            return base.TryAddMember(member);
        }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitModuleBlockDefinition(this);

        internal ModuleBlockDefinition UpdateDefinition(IReadOnlyList<Reference> references)
        {
            if (references is null) {
                throw new ArgumentNullException(nameof(references));
            }

            if (BookkeptReferences.SequenceEqual(references, ReferenceEqualityComparer.Instance)) {
                return this;
            }

            throw new InvalidOperationException("You cannot update the module block");
        }

        private void ResolveImports(ImportResolver importResolver)
        {
            foreach (var importDefinition in BookkeptReferences.OfType<ImportDefinition>()) {
                var importModule = importResolver(importDefinition.FilePath);

                if (importModule is null) {
                    throw SyntaxTreeThrowHelper.ModuleNotFound(importDefinition.FilePath, filePosition: importDefinition.FilePosition);
                }

                importedModules.Add(importModule);
            }
        }

        private void ResolveUsingStaticDirectives()
        {
            foreach (var directiveName in usingStaticDirectives.Keys) {
                var usingStaticDirectory = usingStaticDirectives[directiveName];

                if (usingStaticDirectory.IsResolved) {
                    continue;
                }

                var elementType = usingStaticDirectory.Definition.ElementType.Resolve();

                if (usingStaticDirectiveResolvedElementTypeNames.Contains(elementType.Name)) {
                    throw new BlockEvaluationException("Two using static directives cannot point to the same type.");
                }

                if (!(elementType is INestedTypesProvider nestedTypesProvider)) {
                    throw new BlockEvaluationException("The using static directive can only applied on types with nested types.");
                }

                foreach (var nestedType in nestedTypesProvider.GetNestedTypes()) {
                    importedTypesResolver.AddType(nestedType.Name, nestedType);
                }

                usingStaticDirectives[directiveName] = UsingStaticDirective.Resolved(usingStaticDirectory);
                usingStaticDirectiveResolvedElementTypeNames.Add(elementType.Name);
            }
        }

        public void ResolveOnce(ImportResolver importResolver)
        {
            ResolveImports(importResolver);
            ResolveUsingStaticDirectives();
        }

        private class ModuleBlockContainer : BlockContainer
        {
            public override BlockDefinition? Block {
                get => moduleBlock;
                set => throw new InvalidOperationException("You cannot set the parent block of a module");
            }

            private ModuleBlockDefinition moduleBlock;

            public ModuleBlockContainer(ModuleBlockDefinition moduleBlock) =>
                this.moduleBlock = moduleBlock;
        }

        public readonly struct UsingStaticDirective
        {
            public static UsingStaticDirective Resolved(UsingStaticDirectiveDefinition directive) =>
                new UsingStaticDirective(directive, isResolved: true);

            public UsingStaticDirectiveDefinition Definition { get; }
            public bool IsResolved { get; }

            private UsingStaticDirective(UsingStaticDirectiveDefinition definition, bool isResolved)
            {
                Definition = definition;
                IsResolved = isResolved;
            }

            public UsingStaticDirective(UsingStaticDirectiveDefinition definition)
            {
                Definition = definition;
                IsResolved = false;
            }

            public static implicit operator UsingStaticDirective(UsingStaticDirectiveDefinition directive) =>
                new UsingStaticDirective(directive);

            public static implicit operator UsingStaticDirectiveDefinition(UsingStaticDirective entry) =>
                entry.Definition;
        }
    }
}

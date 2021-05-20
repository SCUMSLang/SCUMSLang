using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.Visitors;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public sealed partial class ModuleDefinition
    {
        internal class ModuleBlockDefinition : TypeBlockDefinition
        {
            public override SyntaxTreeNodeType NodeType =>
                SyntaxTreeNodeType.ModuleBlockDefinition;

            public override BlockScope BlockScope =>
                BlockScope.Module;

            public override ModuleDefinition Module { get; }
            //public override TypeReference DeclaringType => Module;

            //public override string Name =>
            //    string.Empty;

            public IReadOnlyDictionary<string, UsingStaticDirectiveEntry> UsingStaticDirectives =>
                usingStaticDirectives;

            internal IReferenceResolver ModuleExclusiveReferenceResolver { get; }

            internal protected override LinkedBucketList<string, TypeReference> ModuleTypeList { get; }

            protected override BlockDefinition ParentBlock => this;

            private NestedReferenceResolver nestedReferenceResolver;
            private Dictionary<string, UsingStaticDirectiveEntry> usingStaticDirectives;
            private HashSet<string> typeNamesOfResolvedUsingStaticDirective;

            public ModuleBlockDefinition(ModuleDefinition module)
            {
                nestedReferenceResolver = new NestedReferenceResolver();

                var moduleExclusiveReferencePool = new ReferenceResolverPool();
                moduleExclusiveReferencePool.Add(nestedReferenceResolver);
                moduleExclusiveReferencePool.Add(this);
                ModuleExclusiveReferenceResolver = moduleExclusiveReferencePool;

                typeNamesOfResolvedUsingStaticDirective = new HashSet<string>();
                usingStaticDirectives = new Dictionary<string, UsingStaticDirectiveEntry>();
                ModuleTypeList = new LinkedBucketList<string, TypeReference>();
                Module = module;
            }

            internal void ResolveUsingStaticDirectives()
            {
                foreach (var directiveName in usingStaticDirectives.Keys.ToList()) {
                    var directiveEntry = usingStaticDirectives[directiveName];

                    if (directiveEntry.IsResolved) {
                        continue;
                    }

                    var elementType = directiveEntry.Directive.ElementType.Resolve();

                    if (typeNamesOfResolvedUsingStaticDirective.Contains(elementType.Name)) {
                        throw new BlockEvaluatingException("Two using static directives cannot point to the same type.");
                    }

                    if (!(elementType is INestedTypesProvider nestedTypesProvider)) {
                        throw new BlockEvaluatingException("The using static directive can only applied on types with nested types.");
                    }

                    foreach (var nestedType in nestedTypesProvider.GetNestedTypes()) {
                        nestedReferenceResolver.CascadingTypes.Add(nestedType.Name, nestedType);
                    }

                    usingStaticDirectives[directiveName] = UsingStaticDirectiveEntry.Resolved(directiveEntry);
                    typeNamesOfResolvedUsingStaticDirective.Add(elementType.Name);
                }
            }

            protected override bool TryAddMember(Reference member)
            {
                if (member is UsingStaticDirective usingStaticDirective) {
                    if (usingStaticDirectives.ContainsKey(usingStaticDirective.Name)) {
                        throw new NameReservedException(usingStaticDirective.Name, "You cannot add a using-static-directive with same type twice.");
                    }

                    usingStaticDirectives.Add(usingStaticDirective.Name, usingStaticDirective);

                    if (TryGetType(usingStaticDirective.ElementType.Name, isLongName: false, out var type)) {
                        try {
                            _ = type.Resolve();
                            ResolveUsingStaticDirectives();
                        } catch (DefinitionNotFoundException) {
                            // Here we can *try* to resolve using 
                            // static directive so ignore error.
                        }
                    }

                    return true;
                }

                return base.TryAddMember(member);
            }

            public void Import(MemberReference member) =>
                _ = TryAddMember(member);

            protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
                visitor.VisitModuleBlockDefinition(this);

            private class NestedReferenceResolver : ReferenceResolver
            {
                public override LinkedBucketList<string, Reference> BlockMembers { get; }
                public override LinkedBucketList<string, TypeReference> CascadingTypes { get; }

                public NestedReferenceResolver()
                {
                    BlockMembers = new LinkedBucketList<string, Reference>();
                    CascadingTypes = new LinkedBucketList<string, TypeReference>();
                }

            }

            public readonly struct UsingStaticDirectiveEntry
            {
                public static UsingStaticDirectiveEntry Resolved(UsingStaticDirective directive) =>
                    new UsingStaticDirectiveEntry(directive, true);

                public UsingStaticDirective Directive { get; }
                public bool IsResolved { get; }

                private UsingStaticDirectiveEntry(UsingStaticDirective directive, bool isResolved)
                {
                    Directive = directive;
                    IsResolved = isResolved;
                }

                public UsingStaticDirectiveEntry(UsingStaticDirective directive)
                {
                    Directive = directive;
                    IsResolved = false;
                }

                public static implicit operator UsingStaticDirectiveEntry(UsingStaticDirective directive) =>
                    new UsingStaticDirectiveEntry(directive);

                public static implicit operator UsingStaticDirective(UsingStaticDirectiveEntry entry) =>
                    entry.Directive;
            }
        }
    }
}

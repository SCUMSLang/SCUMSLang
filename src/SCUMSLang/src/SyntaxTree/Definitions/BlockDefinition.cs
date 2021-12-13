using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.References;
using Teronis;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public abstract partial class BlockDefinition : Reference, IReferenceResolver
    {
        public static bool TryGetFirstOfReferencesByNameAndProber<T>(IEnumerable<Reference> candidates, Func<Reference, bool> isMemberDelegate, [MaybeNullWhen(false)] out T member)
            where T : Reference
        {
            foreach (var candiate in candidates) {
                if (isMemberDelegate(candiate)) {
                    member = (T)candiate;
                    return true;
                }
            }

            member = null;
            return false;
        }

        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.BlockDefinition;

        public abstract BlockScope BlockScope { get; }
        public abstract ModuleDefinition Module { get; }
        /// <summary>
        /// All references (or definitions) as they appeared
        /// from top to bottom of a block.
        /// </summary>
        public IReadOnlyList<Reference> BookkeptReferences => bookkeptReferences;
        public IReferenceResolver BlockReferenceResolver => blockReferenceResolver;

        private List<Reference> bookkeptReferences;
        private ReferenceResolver blockReferenceResolver;
        private BlockWideReferenceResolverPool blockWideReferenceResolver;

        public BlockDefinition()
        {
            bookkeptReferences = new List<Reference>();
            blockReferenceResolver = new ReferenceResolver();
            blockWideReferenceResolver = new BlockWideReferenceResolverPool(this);
        }

        public bool TryGetBlocksMembersByName(string name, [MaybeNullWhen(false)] out List<Reference> foundNodes)
        {
            foundNodes = new List<Reference>();

            foreach (var block in blockWideReferenceResolver.GetBlocks()) {
                var (success, nodes) = blockReferenceResolver.Members.Buckets.TryGetValue((name, typeof(TypeReference)));

                if (success) {
                    foreach (var node in nodes) {
                        foundNodes.Add(node);
                    }
                }
            }

            return foundNodes.Count != 0;
        }

        /// <summary>
        /// Gets nodes by name casted to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Not all nodes by name <paramref name="name"/> are of type <typeparamref name="T"/>.</exception>
        public List<T>? BlocksMembersByName<T>(string name)
            where T : Reference
        {
            if (TryGetBlocksMembersByName(name, out var nodes)) {
                try {
                    var candidates = nodes.Cast<T>().ToList();
                    return candidates;
                } catch (InvalidCastException) {
                    throw new NameReservedException(name, $"A programming structure of another type with the name {name} exists already.");
                }
            }

            return null;
        }

        public bool TryGetFirstOfMembersByNameAndProber<T>(string name, Func<Reference, bool> isNodeDelegate, [MaybeNullWhen(false)] out T function)
            where T : Reference
        {
            if (TryGetBlocksMembersByName(name, out var candidates)
                && TryGetFirstOfReferencesByNameAndProber(candidates, isNodeDelegate, out function)) {
                return true;
            }

            function = null;
            return false;
        }

        public bool TryGetFirstOfMembers<T>(IEnumerable<T> candidates, T template, [MaybeNullWhen(false)] out T member, IEqualityComparer<T>? comparer = null)
            where T : MemberReference
        {
            comparer ??= EqualityComparer<T>.Default;

            return TryGetFirstOfReferencesByNameAndProber(
                candidates,
                (node) => node is T typedNode && comparer.Equals(typedNode, template),
                out member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns>False indicates skip.</returns>
        protected virtual bool TryAddMember(Reference member)
        {
            if (member is ICollectibleMember collectibleMember) {
                bool hasDuplication = TryGetBlocksMembersByName(collectibleMember.Name, out _);

                // If node has name, then it can handle name duplications.
                if (hasDuplication && member is IOverloadableReference nameDuplicationHandleableNode) {
                    var result = nameDuplicationHandleableNode.SolveConflict(this);

                    if (result == OverloadConflictResult.True) {
                        hasDuplication = false;
                    } else if (result == OverloadConflictResult.Skip) {
                        return false;
                    }
                }

                if (hasDuplication) {
                    throw new NameReservedException(collectibleMember.Name);
                }

                blockReferenceResolver.AddType(collectibleMember.Name, member);
            }

            if (member is INestedTypesProvider namesReservableNode && namesReservableNode.HasNestedTypes) {
                foreach (var namedNode in namesReservableNode.GetNestedTypes()) {
                    _ = TryAddMember(namedNode);
                }
            }

            return true;
        }

        public void AddNode(Reference node)
        {
            if (node is IBlockScopable blockScopable
                && blockScopable.BlockScope != BlockScope) {
                throw new BadBlockScopeException();
            }

            if (TryAddMember(node)) {
                bookkeptReferences.Add(node);

                if (node is IBlockHolder blockHolder && blockHolder.IsBlockOwnable) {
                    blockHolder.SetupBlock(this);
                }
            }
        }

        public ResolveResult<T> Resolve<T>(TypeReference type)
            where T : TypeReference =>
            BlockReferenceResolver.Resolve<T>(type);

        public ResolveResult<TypeReference> Resolve(TypeReference type) =>
           Resolve<TypeReference>(type);

        public ResolveResult<FieldDefinition> Resolve(FieldReference field) =>
            BlockReferenceResolver.Resolve(field);

        public ResolveResult<MethodDefinition> Resolve(MethodReference method) =>
            BlockReferenceResolver.Resolve(method);

        public ResolveResult<EventHandlerDefinition> Resolve(EventHandlerReference eventHandler) =>
            BlockReferenceResolver.Resolve(eventHandler);

        public ResolveResult<TypeDefinition> GetType(string typeName) =>
            BlockReferenceResolver.GetType(typeName);

        public ResolveResult<EventHandlerDefinition> GetEventHandler(string eventHandlerName) =>
            BlockReferenceResolver.GetEventHandler(eventHandlerName);

        public ResolveResult<FieldDefinition> GetField(string fieldName) =>
            BlockReferenceResolver.GetField(fieldName);

        public ResolveResult<MethodDefinition> GetMethod(string methodName) =>
            BlockReferenceResolver.GetMethod(methodName);

        public ResolveResult<MethodDefinition> GetMethod(MethodReference methodReference) =>
            BlockReferenceResolver.GetMethod(methodReference);

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, BookkeptReferences);

        private class BlockWideReferenceResolverPool : ReferenceResolverPoolBase
        {
            private readonly BlockDefinition block;

            public BlockWideReferenceResolverPool(BlockDefinition block) => this.block = block;

            public IEnumerable<BlockDefinition> GetBlocks() {
                BlockDefinition parentBlock = block;
                BlockDefinition? previousParentBlock;

                do {
                    yield return parentBlock;
                    previousParentBlock = parentBlock;
                    parentBlock = parentBlock.ParentBlock;
                } while (!ReferenceEquals(block.Module.Block, previousParentBlock));
            }

            protected override IEnumerable<IReferenceResolver> GetReferenceResolvers()
            {
                BlockDefinition parentBlock = block;
                BlockDefinition? previousParentBlock;

                do {
                    yield return parentBlock;
                    previousParentBlock = parentBlock;
                    parentBlock = parentBlock.ParentBlock;
                } while (!ReferenceEquals(block.Module.Block, previousParentBlock));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.SyntaxTree.References;
using Teronis.Collections.Generic;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public abstract partial class BlockDefinition : Reference
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
        public IReadOnlyList<Reference> ReferenceRecords =>
            referenceRecords;

        /// <summary>
        /// All types
        /// </summary>
        public IReadOnlyLinkedBucketList<string, TypeReference> ModuleTypes =>
            ModuleTypeList;

        private bool isBlockClosed;

        internal protected LinkedBucketList<string, Reference> LocalMemberList { get; }
        internal protected abstract LinkedBucketList<string, TypeReference> ModuleTypeList { get; }

        protected abstract BlockDefinition ParentBlock { get; }

        private List<Reference> referenceRecords;

        public BlockDefinition()
        {
            referenceRecords = new List<Reference>();
            LocalMemberList = new LinkedBucketList<string, Reference>();
        }

        /// <summary>
        /// Returns hisself and base blocks.
        /// </summary>
        /// <returns>Hisself and base blocks.</returns>
        protected IEnumerable<BlockDefinition> AllBlocks()
        {
            BlockDefinition parentBlock = this;
            BlockDefinition? previousParentBlock;

            do {
                yield return parentBlock;
                previousParentBlock = parentBlock;
                parentBlock = parentBlock.ParentBlock;
            } while (!ReferenceEquals(Module.Block, previousParentBlock));
        }

        protected IEnumerable<BlockType> AllBlocksOfType<BlockType>()
            where BlockType : BlockDefinition
        {
            foreach (var block in AllBlocks()) {
                if (block is BlockType typedBlock) {
                    yield return typedBlock;
                }
            }
        }

        public bool TryGetBlocksMembersByName(string name, [MaybeNullWhen(false)] out List<Reference> foundNodes)
        {
            foundNodes = new List<Reference>();

            foreach (var block in AllBlocks()) {
                if (block.LocalMemberList.TryGetBucket(name, out ILinkedBucketList<string, Reference>? nodes)) {
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

        public bool TryGetFirstOfMembers<T>(IEnumerable<T> candidates, [MaybeNullWhen(false)] out T member)
            where T : Reference =>
            TryGetFirstOfReferencesByNameAndProber(candidates, (node) => node is T, out member);

        public bool TryGetMemberFirst<T>(string name, [MaybeNullWhen(false)] out T member)
            where T : Reference =>
            TryGetFirstOfMembersByNameAndProber(name, (node) => node is T, out member);

        public bool TryGetMemberFirst<T>(T template, [MaybeNullWhen(false)] out T member)
            where T : MemberReference =>
            TryGetFirstOfMembersByNameAndProber(template.Name, (node) => node.Equals(template), out member);

        public bool TryGetFirstOfMembers<T>(IEnumerable<T> candidates, T template, [MaybeNullWhen(false)] out T member, IEqualityComparer<T>? comparer = null)
            where T : MemberReference
        {
            comparer ??= EqualityComparer<T>.Default;

            return TryGetFirstOfReferencesByNameAndProber(
                candidates,
                (node) => {
                    return node is T typedNode
                        && comparer.Equals(typedNode, template);
                },
                out member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns>False indicates skip.</returns>
        protected virtual bool TryAddMember(Reference member)
        {
            if (member is IMemberDefinition memberDefinition) {
                bool hasDuplication = TryGetBlocksMembersByName(memberDefinition.Name, out _);

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
                    throw new NameReservedException(memberDefinition.Name);
                }

                LocalMemberList.AddLast(memberDefinition.Name, member);

                if (member is TypeReference type) {
                    ModuleTypeList.Add(type.Name, type);
                }
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
            if (node is IBlockScopedReference blockScopableReference
                && blockScopableReference.BlockScope != BlockScope) {
                throw new BadBlockScopeException();
            }

            // We want to fill missing module references.
            //_ = Module.ModuleFillingVisitor.Visit(node);

            if (TryAddMember(node)) {
                referenceRecords.Add(node);

                if (node is IBlockHolder blockHolder && blockHolder.IsBlockOwnable) {
                    blockHolder.SetupBlock(this);
                }
            }
        }

        private TDefinition GetMemberDefinitionBySelector<TReference, TDefinition>(
            string memberName,
            IReadOnlyLinkedBucketList<string, Reference> candidates,
            Func<IEnumerable<TDefinition>, TDefinition?> singleDefinitionSelector,
            Func<Exception> notFoundErrorProvider)
            where TDefinition : class
        {
            if (!candidates.Buckets.TryGetValue(memberName, out var bucket)) {
                throw notFoundErrorProvider();
            }

            return singleDefinitionSelector(bucket.Cast<TDefinition>())
                ?? throw notFoundErrorProvider();
        }

        public FieldDefinition GetField(string fieldName) =>
            GetMemberDefinitionBySelector<FieldReference, FieldDefinition>(
                fieldName,
                Module.Block.LocalMemberList,
                definitions => definitions.SingleOrDefault(),
                () => SyntaxTreeThrowHelper.FieldNotFound(fieldName));

        public MethodDefinition GetMethod(string methodName) =>
            GetMemberDefinitionBySelector<MethodReference, MethodDefinition>(
                methodName,
                LocalMemberList,
                definitions => definitions.SingleOrDefault(),
                () => SyntaxTreeThrowHelper.MethodNotFound(methodName));

        public MethodDefinition GetMethod(MethodReference methodReference) =>
            GetMemberDefinitionBySelector<MethodReference, MethodDefinition>(
                methodReference.Name,
                LocalMemberList,
                definitions => definitions.SingleOrDefault(x =>
                    new MethodOverloadEqualityComparer(Module).Equals(x, methodReference)),
                () => SyntaxTreeThrowHelper.MethodNotFound(methodReference.Name));

        public EventHandlerDefinition GetEventHandler(string eventHandlerName) =>
            GetMemberDefinitionBySelector<EventHandlerReference, EventHandlerDefinition>(
                eventHandlerName,
                LocalMemberList,
                definitions => definitions.SingleOrDefault(),
                () => SyntaxTreeThrowHelper.MethodNotFound(eventHandlerName));

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, ReferenceRecords);
    }
}

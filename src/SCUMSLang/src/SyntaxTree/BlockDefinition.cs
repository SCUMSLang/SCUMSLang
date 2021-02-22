using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.Visitors;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public abstract partial class BlockDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.BlockDefinition;

        public abstract BlockScope BlockScope { get; }
        public abstract ModuleDefinition Module { get; }
        //public

        //public abstract string Name { get; }

        //public virtual string LongName =>
        //    BlockLongName();

        public BlockDefinition CurrentBlock { get; protected set; }

        /// <summary>
        /// All references as they appeared.
        /// </summary>
        public IReadOnlyList<Reference> ReferenceRecords =>
            referenceRecords;

        /// <summary>
        /// All types
        /// </summary>
        public IReadOnlyLinkedBucketList<string, TypeReference> ReadOnlyCascadingTypes =>
            CascadingTypes;

        internal protected LinkedBucketList<string, Reference> BlockMembers { get; }
        internal protected abstract LinkedBucketList<string, TypeReference> CascadingTypes { get; }

        protected abstract BlockDefinition ParentBlock { get; }

        private List<Reference> referenceRecords;
        private bool isBlockClosed;

        public BlockDefinition()
        {
            referenceRecords = new List<Reference>();
            BlockMembers = new LinkedBucketList<string, Reference>();
            CurrentBlock = this;
        }

        public void BeginBlock(BlockDefinition block) =>
            CurrentBlock = block;

        protected IEnumerable<BlockDefinition> YieldBlocks()
        {
            BlockDefinition parentBlock = this;
            BlockDefinition? previousParentBlock;

            do {
                yield return parentBlock;
                previousParentBlock = parentBlock;
                parentBlock = parentBlock.ParentBlock;
            } while (!ReferenceEquals(Module.Block, previousParentBlock));
        }

        protected IEnumerable<BlockType> YieldBlocks<BlockType>()
            where BlockType : BlockDefinition
        {
            foreach (var block in YieldBlocks()) {
                if (block is BlockType typedBlock) {
                    yield return typedBlock;
                }
            }
        }

        public bool TryGetMembers(string name, [MaybeNullWhen(false)] out List<Reference> foundNodes)
        {
            foundNodes = new List<Reference>();

            foreach (var block in YieldBlocks()) {
                if (block.BlockMembers.TryGetBucket(name, out ILinkedBucketList<string, Reference>? nodes)) {
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
        public List<T>? GetMembersCasted<T>(string name)
            where T : Reference
        {
            if (TryGetMembers(name, out var nodes)) {
                try {
                    var candidates = nodes.Cast<T>().ToList();
                    return candidates;
                } catch (InvalidCastException) {
                    throw new NameReservedException(name, $"A programming structure of another type with the name {name} exists already.");
                }
            }

            return null;
        }

        public bool TryGetMemberFirst<T>(IEnumerable<Reference> candidates, Func<Reference, bool> isMemberDelegate, [MaybeNullWhen(false)] out T member)
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

        public bool TryGetMemberFirst<T>(string name, Func<Reference, bool> isNodeDelegate, [MaybeNullWhen(false)] out T function)
            where T : Reference
        {
            if (TryGetMembers(name, out var candidates)
                && TryGetMemberFirst(candidates, isNodeDelegate, out function)) {
                return true;
            }

            function = null;
            return false;
        }

        public bool TryGetMemberFirst<T>(IEnumerable<T> candidates, [MaybeNullWhen(false)] out T member)
            where T : Reference =>
            TryGetMemberFirst(candidates, (node) => node is T, out member);

        public bool TryGetMemberFirst<T>(string name, [MaybeNullWhen(false)] out T member)
            where T : Reference =>
            TryGetMemberFirst(name, (node) => node is T, out member);

        public bool TryGetMemberFirst<T>(T template, [MaybeNullWhen(false)] out T member)
            where T : MemberReference =>
            TryGetMemberFirst(template.Name, (node) => node.Equals(template), out member);

        public bool TryGetMemberFirst<T>(IEnumerable<T> candidates, T template, [MaybeNullWhen(false)] out T member, IEqualityComparer<T>? comparer = null)
            where T : MemberReference
        {
            comparer ??= EqualityComparer<T>.Default;

            return TryGetMemberFirst(
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
                bool hasDuplication = TryGetMembers(memberDefinition.Name, out _);

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

                BlockMembers.AddLast(memberDefinition.Name, member);

                if (member is TypeReference type) {
                    CascadingTypes.Add(type.LongName, type);
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

            if (TryAddMember(node)) {
                referenceRecords.Add(node);

                if (node is IBlockHolder blockHolder) {
                    var block = new LocalBlockDefinition(this, blockHolder);
                    blockHolder.Block = block;
                    BeginBlock(block);
                }
            }
        }

        public void EndBlock()
        {
            if (isBlockClosed) {
                throw new BlockEvaluatingException("You cannot end the block twice.");
            }

            isBlockClosed = true;

            if (ReferenceEquals(this, Module)) {
                throw new BlockEvaluatingException("You cannot end the block of the root block.");
            }

            CurrentBlock = CurrentBlock.ParentBlock;
        }

        public FieldDefinition GetField(string name)
        {
            if (!BlockMembers.TryGetBucket(name, out var bucket)) {
                throw SyntaxTreeThrowHelper.FieldNotFound(name);
            }

            var types = bucket.Cast<FieldDefinition>();
            var type = types.SingleOrDefault();
            return type ?? throw SyntaxTreeThrowHelper.FieldNotFound(name);
        }

        protected MethodDefinition GetMethodDefinition(MethodReference method)
        {
            if (!BlockMembers.TryGetBucket(method.Name, out var bucket)) {
                throw SyntaxTreeThrowHelper.MethodNotFound(method.Name);
            }

            var types = bucket.OfType<MethodDefinition>();
            var type = types.SingleOrDefault(x => MethodReferenceEqualityComparer.OverloadComparer.Default.Equals(x, method));
            return type ?? throw SyntaxTreeThrowHelper.MethodNotFound(method.Name);
        }

        public MethodDefinition GetMethod(MethodReference method) =>
            GetMethodDefinition(
                new MethodReference(
                    method.Name,
                    method.GenericParameters,
                    method.Parameters,
                    Module));

        //public MethodDefinition GetMethod(MethodCallDefinition methodCall) =>
        //    GetMethodDefinition(
        //        new MethodReference(
        //            methodCall.InferredMethod.Name,
        //            methodCall.InferredMethod.GenericParameters,
        //            methodCall.InferredMethod.Parameters,
        //            Module));

        public MethodDefinition GetMethod(
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters = null,
            IReadOnlyList<ParameterDefinition>? parameters = null) =>
            GetMethodDefinition(
                new MethodReference(
                    name, 
                    genericParameters, 
                    parameters, 
                    Module));

        protected EventHandlerDefinition GetEventHandlerDefinition(EventHandlerReference eventHandler)
        {
            if (!BlockMembers.TryGetBucket(eventHandler.Name, out var bucket)) {
                throw SyntaxTreeThrowHelper.EventHandlerdNotFound(eventHandler.Name);
            }

            var types = bucket.OfType<EventHandlerDefinition>();

            var type = types.SingleOrDefault(x => EventHandlerReferenceEqualityComparer.OverloadComparer.Default.Equals(x, eventHandler))
                ?? throw SyntaxTreeThrowHelper.EventHandlerdNotFound(eventHandler.Name);

            return type;
        }

        public EventHandlerDefinition GetEventHandler(
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters = null,
            IReadOnlyList<ParameterDefinition>? parameters = null,
            IReadOnlyList<MethodCallDefinition>? conditions = null) =>
            GetEventHandlerDefinition(
                new EventHandlerReference(
                    name,
                    genericParameters,
                    parameters,
                    conditions,
                    Module));

        public EventHandlerDefinition GetEventHandler(EventHandlerReference eventHandler) =>
            GetEventHandlerDefinition(
                new EventHandlerReference(
                    eventHandler.Name,
                    eventHandler.GenericParameters,
                    eventHandler.Parameters,
                    eventHandler.Conditions,
                    Module));

        public override bool Equals(object? obj)
        {
            if (!(obj is BlockDefinition block)) {
                return false;
            }

            var equals = isBlockClosed == block.isBlockClosed
                && referenceRecords.SequenceEqual(block.referenceRecords);

            Trace.WriteLineIf(!equals, $"{nameof(BlockDefinition)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, ReferenceRecords);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public abstract partial class BlockDefinition : Reference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.Block;
        public abstract Scope Scope { get; }
        public abstract ModuleDefinition Module { get; }

        public BlockDefinition CurrentBlock { get; protected set; }
        public IReadOnlyList<Reference> Definitions => definitions;

        internal protected abstract NameReservableNodePool NameReservableNodes { get; }
        protected BlockDefinition Parent;

        private List<Reference> definitions;
        private bool blockClosed;

        public BlockDefinition()
        {
            definitions = new List<Reference>();
            CurrentBlock = this;
            Parent = this;
        }

        private BlockDefinition(BlockDefinition parent)
        {
            definitions = new List<Reference>();
            CurrentBlock = this;
            Parent = parent;
        }

        public void BeginBlock(BlockDefinition block) =>
            CurrentBlock = block;

        protected IEnumerable<BlockDefinition> YieldBlocks()
        {
            BlockDefinition parentBlock = this;

            do {
                yield return parentBlock;
                parentBlock = parentBlock.Parent;
            } while (!ReferenceEquals(Module, parentBlock));
        }

        public bool TryGetNodesByName(string name, [MaybeNullWhen(false)] out List<Reference> foundNodes)
        {
            foundNodes = new List<Reference>();

            foreach (var block in YieldBlocks()) {
                if (block.NameReservableNodes.TryGetBucket(name, out ILinkedBucketList<string, Reference>? nodes)) {
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
        public List<T>? GetCastedNodesByName<T>(string name)
            where T : Reference
        {
            if (TryGetNodesByName(name, out var nodes)) {
                try {
                    var candidates = nodes.Cast<T>().ToList();
                    return candidates;
                } catch (InvalidCastException) {
                    throw new ArgumentException($"A programming structure of another type with the name {name} exists already.");
                }
            }

            return null;
        }

        public bool TryGetFirstNode<T>(IEnumerable<Reference> candidates, Func<Reference, bool> isFunctionDelegate, [MaybeNullWhen(false)] out T function)
            where T : Reference
        {
            foreach (var candiate in candidates) {
                if (isFunctionDelegate(candiate)) {
                    function = (T)candiate;
                    return true;
                }
            }

            function = null;
            return false;
        }

        public bool TryGetFirstNode<T>(string name, Func<Reference, bool> isNodeDelegate, [MaybeNullWhen(false)] out T function)
            where T : Reference
        {
            if (TryGetNodesByName(name, out var candidates)
                && TryGetFirstNode(candidates, isNodeDelegate, out function)) {
                return true;
            }

            function = null;
            return false;
        }

        public bool TryGetFirstNode<T>(IEnumerable<T> candidates, [MaybeNullWhen(false)] out T function)
            where T : Reference =>
            TryGetFirstNode(candidates, (node) => node is T, out function);

        public bool TryGetFirstNode<T>(string name, [MaybeNullWhen(false)] out T function)
            where T : Reference =>
            TryGetFirstNode(name, (node) => node is T, out function);

        public bool TryGetFirstNode<T>(T template, [MaybeNullWhen(false)] out T function)
            where T : Reference, INameReservableReference =>
            TryGetFirstNode(template.Name, (node) => node.Equals(template), out function);

        public bool TryGetFirstNode<T>(IEnumerable<T> candidates, T template, [MaybeNullWhen(false)] out T function, IEqualityComparer<T>? comparer = null)
            where T : Reference, INameReservableReference
        {
            comparer ??= EqualityComparer<T>.Default;
            return TryGetFirstNode(candidates, (node) => node is T typedNode && comparer.Equals(typedNode, template), out function);
        }

        public bool TryGetFirstFunctionNode(
            string name,
            IReadOnlyList<ConstantReference>? genericArguments,
            IReadOnlyList<ConstantReference>? arguments,
            [MaybeNullWhen(false)] out FunctionReference function,
            bool required)
        {
            genericArguments ??= new List<ConstantReference>();
            arguments ??= new List<ConstantReference>();

            bool isFunction(Reference node)
            {
                if (!(node is FunctionReference function)) {
                    return false;
                }

                if (function.GenericParameters.Count != genericArguments.Count) {
                    return false;
                }

                var genericParameterIndex = function.GenericParameters.Count;

                while (--genericParameterIndex >= 0) {
                    //if (!function.GenericParameters[genericParameterIndex].DeclaringType.IsSubsetOf(genericArguments[genericParameterIndex].Type)) {
                    //    return false;
                    //}
                }

                var parameterIndex = function.Parameters.Count;

                if (function.Parameters.Count != arguments.Count) {
                    return false;
                }

                while (--parameterIndex >= 0) {
                    //if (!function.Parameters[parameterIndex].DeclaringType.IsSubsetOf(arguments[parameterIndex].Type)) {
                    //    return false;
                    //}
                }

                return true;
            }

            if (TryGetFirstNode(name, isFunction, out function)) {
                return true;
            } else if (required) {
                throw new ArgumentException($"Function with name '{name}' and proper overload does not exist");
            }

            return false;
        }

        public TypeDefinition GetTypeDefinition(SystemType definitionType)
        {
            if (!NameReservableNodes.TryGetBucket(SystemTypeLibrary.Sequences[definitionType], out var bucket)) {
                throw new NotSupportedException("This in-built type does not have a one-to one mapping.");
            }

            return (TypeDefinition)bucket.First!.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathFragments"></param>
        /// <returns>Either <see cref="EnumerationMemberReference"/> or <see cref="EnumerationTypeReference"/>.</returns>
        public TypeDefinition GetTypeDefinition(IReadOnlyList<string> pathFragments, TypeReferenceViewpoint viewpoint)
        {
            if (pathFragments.Count == 0) {
                throw new ArgumentException("Insufficient path fragments.");
            }

            if (viewpoint == TypeReferenceViewpoint.Type && pathFragments.Count > 1) {
                throw new ArgumentException($"You are specifiying a type. You cannot use a member access.");
            }

            if (viewpoint == TypeReferenceViewpoint.Value && pathFragments.Count > 2) {
                throw new ArgumentException($"A member access across two type definitions are not supported.");
            }

            var nodeName = pathFragments[0];
            var candidates = GetCastedNodesByName<TypeDefinition>(nodeName);

            if (candidates is null) {
                throw new ArgumentException($"The type definition {nodeName} does not exist.");
            } else if (candidates.Count > 1) {
                throw new ArgumentException($"There are two or more type definition named by {nodeName}.");
            }

            var candidate = candidates[0].SourceType;

            if (candidate is EnumerationTypeReference enumeration) {
                if (pathFragments.Count == 1) {
                    return enumeration;
                }
                // Enum member is only allowed as value.
                else if (viewpoint == TypeReferenceViewpoint.Value) {
                    return enumeration.GetMemberByName(pathFragments[1]);
                }
            } else if (pathFragments.Count == 1) {
                return candidate;
            }

            throw new ArgumentException($"The type definition '{nodeName}' does not support member access.");
        }

        public void AddNode(Reference node)
        {
            bool handleNameReservation(Reference node)
            {
                if (node is INameReservableReference nameReservableNode) {
                    bool hasDuplication = NameReservableNodes.TryGetBucket(nameReservableNode.Name, out _);

                    // If node has name, then it can handle name duplications.
                    if (hasDuplication && node is INameDuplicationHandleableReference nameDuplicationHandleableNode) {
                        var result = nameDuplicationHandleableNode.CanReserveName(this);

                        if (result == ConditionalNameReservationResult.True) {
                            hasDuplication = false;
                        } else if (result == ConditionalNameReservationResult.Skip) {
                            return false;
                        }
                    }

                    if (hasDuplication) {
                        throw new ArgumentException($"The name '{nameReservableNode.Name}' is already reserved.");
                    }

                    NameReservableNodes.AddLast(nameReservableNode.Name, node);
                }

                if (node is INamesReservableReference namesReservableNode && namesReservableNode.HasReservedNames) {
                    foreach (var namedNode in namesReservableNode.GetReservedNames()) {
                        handleNameReservation(namedNode);
                    }
                }

                return true;
            }

            if (node is IScopableReference scopableNode && scopableNode.Scope != Scope) {
                throw new ArgumentException("Declaration has invalid scope.");
            }

            if (handleNameReservation(node)) {
                definitions.Add(node);
            }
        }

        public void AddAssignment(AssignDefinition assignment) =>
            definitions.Add(assignment);

        public void EndBlock()
        {
            if (blockClosed) {
                throw new InvalidOperationException("You cannot end the block twice.");
            }

            blockClosed = true;

            if (ReferenceEquals(this, Module)) {
                throw new InvalidOperationException("You cannot end the block of the root block.");
            }

            CurrentBlock = CurrentBlock.Parent;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is BlockDefinition block)) {
                return false;
            }

            var equals = blockClosed == block.blockClosed
                && definitions.SequenceEqual(block.definitions);

            Trace.WriteLineIf(!equals, $"{nameof(BlockDefinition)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(ReferenceType, Definitions);
    }
}

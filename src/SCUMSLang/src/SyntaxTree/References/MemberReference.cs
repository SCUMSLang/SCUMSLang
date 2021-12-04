using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree.References
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public abstract class MemberReference : Reference, IOwnableReference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.MemberReference;

        public virtual TypeReference? DeclaringType {
            get => declaringType;
            internal set => declaringType = value;
        }

        public virtual string Name =>
            name;

        [AllowNull]
        public BlockContainer ParentBlockContainer {
            protected get => parentBlockContainer ??= new BlockContainer();
            init => parentBlockContainer = value;
        }

        [AllowNull]
        public virtual BlockDefinition ParentBlock {
            get => ParentBlockContainer.Block ?? throw new InvalidOperationException();
            internal set => ParentBlockContainer.Block = value;
        }

        [MemberNotNullWhen(true, nameof(ParentBlock))]
        public virtual bool HasParentBlock =>
            ParentBlockContainer.HasBlock;

        private TypeReference? declaringType;
        private BlockContainer? parentBlockContainer;
        private string name;
        private MemberReference? resolvedDefinition;

        [AllowNull]
        BlockDefinition IOwnableReference.ParentBlock {
            get => ParentBlock;
            set => ParentBlock = value;
        }

        internal MemberReference() =>
            name = string.Empty;

        internal MemberReference(string? name) =>
            this.name = name ?? string.Empty;

        protected abstract IMemberDefinition ResolveMemberDefinition();

        protected T CacheOrResolve<T>(Func<T> resolveHandler)
            where T : MemberReference
        {
            if (resolvedDefinition is null) {
                resolvedDefinition = resolveHandler();
            }

            return (T)resolvedDefinition;
        }

        public IMemberDefinition Resolve() =>
            ResolveMemberDefinition();

        private string GetDebuggerDisplay() =>
            $"Name = {Name}, Type = {GetType().Name}";
    }
}

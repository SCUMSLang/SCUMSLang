using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree
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
        public virtual ModuleDefinition Module {
            get => module ?? throw new InvalidOperationException();
            internal set => module = value;
        }

        [MemberNotNullWhen(true, nameof(Module))]
        public virtual bool HasModule =>
            module is not null;

        private TypeReference? declaringType;
        private ModuleDefinition? module;
        private string name;
        private MemberReference? resolvedDefinition;

        [AllowNull]
        ModuleDefinition IOwnableReference.Module {
            get => Module;
            set => Module = value;
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

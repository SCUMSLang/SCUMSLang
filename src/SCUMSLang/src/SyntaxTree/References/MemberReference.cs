using System;
using System.Diagnostics;

namespace SCUMSLang.SyntaxTree.References
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public abstract class MemberReference : Reference, IMember
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.MemberReference;
        public virtual TypeReference? DeclaringType { get; internal set; }
        public virtual string Name => name;

        private string name;
        private MemberReference? resolvedDefinition;

        internal MemberReference(string? name) =>
            this.name = name ?? string.Empty;

        internal MemberReference()
            : this(name: null) { }

        protected abstract IMember ResolveMember();

        protected T CacheOrResolve<T>(Func<T> resolveHandler)
            where T : MemberReference
        {
            if (resolvedDefinition is null) {
                resolvedDefinition = resolveHandler();
            }

            return (T)resolvedDefinition;
        }

        public IMember Resolve() =>
            ResolveMember();

        private string GetDebuggerDisplay() =>
            $"Name = {Name}, Type = {GetType().Name}";
    }
}

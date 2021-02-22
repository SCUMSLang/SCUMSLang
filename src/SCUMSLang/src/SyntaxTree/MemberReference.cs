using System;
using Teronis.Text;

namespace SCUMSLang.SyntaxTree
{
    public abstract class MemberReference : Reference, IResolvableDependencies
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.MemberReference;

        public virtual TypeReference DeclaringType {
            get => declaringType;
            internal set => declaringType = value;
        }

        public virtual ModuleDefinition Module {
            get => DeclaringType?.Module ?? throw new InvalidOperationException();
        }

        public virtual string Name { get; }
        public abstract string LongName { get; }

        private TypeReference declaringType = null!;

        internal MemberReference() =>
            Name = string.Empty;

        internal MemberReference(string? name) =>
            Name = name ?? string.Empty;

        internal string MemberFullName()
        {
            if (declaringType is null) {
                return Name;
            }

            var declaringTypeFullName = declaringType.LongName;
            var seperationHelper = new StringSeparationHelper(".");
            seperationHelper.SetStringSeparator(ref declaringTypeFullName);
            return declaringTypeFullName += Name;
        }

        protected abstract IMemberDefinition ResolveDefinition();

        protected virtual void ResolveDependencies() =>
            DeclaringType?.Resolve();

        void IResolvableDependencies.ResolveDependencies() =>
             ResolveDependencies();

        public IMemberDefinition Resolve() =>
            ResolveDefinition();

        public override bool Equals(object? obj)
        {
            return base.Equals(obj) && obj is MemberReference member
                && Equals(member.LongName, LongName)
                && MemberReferenceEqualityComparer.ShallowComparer.Default.Equals(member.DeclaringType, DeclaringType);
        }
    }
}

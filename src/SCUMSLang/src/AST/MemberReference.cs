using Teronis.Text;

namespace SCUMSLang.AST
{
    public abstract class MemberReference : Reference, IResolvableDependencies
    {
        public override TreeTokenType TokenType =>
            TreeTokenType.MemberReference;

        public virtual TypeReference DeclaringType {
            get => declaringType;
            internal set => declaringType = value;
        }

        public virtual ModuleDefinition? Module {
            get => DeclaringType?.Module ?? null;
        }

        public virtual string Name { get; }
        public abstract string FullName { get; }

        private TypeReference declaringType = null!;

        internal MemberReference() =>
            Name = string.Empty;

        internal MemberReference(string name) =>
            Name = name ?? throw new System.ArgumentNullException(nameof(name));

        internal string MemberFullName()
        {
            if (declaringType is null) {
                return Name;
            }

            var declaringTypeFullName = declaringType.FullName;
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
    }
}

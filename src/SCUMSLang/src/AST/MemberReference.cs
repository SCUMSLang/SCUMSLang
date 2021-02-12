namespace SCUMSLang.AST
{
    public abstract class MemberReference : Reference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.MemberReference;

        public virtual TypeReference? DeclaringType => declaringType;

        public virtual ModuleDefinition? Module {
            get => declaringType?.Module ?? null;
        }

        private TypeReference? declaringType;

        protected abstract IMemberDefinition? ResolveDefinition();

        public IMemberDefinition? Resolve() =>
            ResolveDefinition();
    }
}

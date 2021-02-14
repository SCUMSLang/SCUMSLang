using System;

namespace SCUMSLang.AST
{
    public class TypeReference : MemberReference
    {
        public static TypeReference StringReference = new TypeReference(SystemTypeLibrary.Sequences[SystemType.String]);

        public override TreeTokenType TokenType =>
            TreeTokenType.TypeReference;

        public override ModuleDefinition? Module {
            get => module;
        }

        public override string FullName =>
            TypeFullName();

        private ModuleDefinition? module;
        private TypeDefinition? resolvedDefinition;

        protected TypeReference(string name)
            : base(name) { }

        public TypeReference(string name, ModuleDefinition module)
            : base(name) =>
            this.module = module ?? throw new ArgumentNullException(nameof(module));

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();

        public new TypeDefinition Resolve()
        {
            ResolveDependencies();

            return resolvedDefinition = resolvedDefinition
                ?? module?.Resolve(this)
                ?? throw new NotSupportedException();
        }

        protected TypeDefinition ResolveNonAlias(TypeDefinition typeDefinition)
        {
            if (typeDefinition.IsAlias) {
                while (typeDefinition.IsAlias) {
                    if (typeDefinition.BaseType is null) {
                        throw new InvalidOperationException("Type alias has no target type specified.");
                    }

                    typeDefinition = typeDefinition.BaseType.Resolve();
                }


            } else {
                typeDefinition = typeDefinition.Resolve();
            }

            return typeDefinition;
        }

        public virtual TypeDefinition ResolveNonAlias() =>
            ResolveNonAlias(Resolve());

        internal string TypeFullName() =>
            Name;
    }
}

using System;

namespace SCUMSLang.SyntaxTree
{
    public class TypeReference : MemberReference
    {
        public static TypeReference CreateStringReference(ModuleDefinition module) =>
            new TypeReference(module, SystemTypeLibrary.Sequences[SystemType.String]);

        public static TypeReference CreateIntegerReference(ModuleDefinition module) =>
            new TypeReference(module, SystemTypeLibrary.Sequences[SystemType.Integer]);

        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeReference;

        public override ModuleDefinition Module {
            get => module ?? base.Module;
        }

        public override string LongName =>
            TypeLongName();

        public virtual bool IsArray { get; }

        private ModuleDefinition? module;
        private TypeDefinition? resolvedDefinition;

        protected TypeReference(string? name)
            : base(name) { }

        public TypeReference(ModuleDefinition module, string? name)
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

        internal string TypeLongName() =>
            Name;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj) && obj is TypeReference type
                && type.IsArray == IsArray;
        }
    }
}

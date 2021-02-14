using System;

namespace SCUMSLang.AST
{
    public class FieldReference : MemberReference, IMemberDefinition
    {
        public override TreeTokenType TokenType => TreeTokenType.FieldReference;
        public virtual TypeReference FieldType { get; internal set; }
        public bool IsStatic { get; internal set; }

        public override ModuleDefinition? Module =>
            DeclaringType?.Module ?? FieldType.Module;

        private FieldDefinition? resolvedDefinition;

        public override string LongName => FieldType is null
            ? MemberFullName()
            : $"{FieldType.LongName} {MemberFullName()}";

        public FieldReference(string name, TypeReference fieldType)
            : base(name) =>
            FieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));

        public FieldReference(string name, TypeReference fieldType, TypeReference declaringType)
            : this(name, fieldType) =>
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();

        protected override void ResolveDependencies()
        {
            FieldType?.Resolve();
            base.ResolveDependencies();
        }

        public new virtual FieldDefinition Resolve()
        {
            ResolveDependencies();

            return resolvedDefinition = resolvedDefinition
                ?? Module?.Resolve(this)
                ?? throw new NotSupportedException();
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj) && obj is FieldReference reference
                && reference.IsStatic == IsStatic
                && Equals(reference.FieldType, FieldType);
        }
    }
}

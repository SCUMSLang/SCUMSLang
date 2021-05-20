using System;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class FieldDefinition : FieldReference, IMemberDefinition
    {
        public object? Value { get; set; }

        [AllowNull]
        public override TypeReference FieldType {
            get => base.FieldType ?? throw new InvalidOperationException();
            internal set => base.FieldType = value;
        }

        public FieldDefinition(string name, TypeReference fieldType)
            : base(name, fieldType) { }

        public FieldDefinition(string name, TypeReference fieldType, TypeReference declaringType)
            : base(name, fieldType) =>
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

        public new virtual FieldDefinition Resolve() =>
            CacheOrResolve(() => Module.Resolve(this));

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitFieldDefinition(this);

        public FieldDefinition UpdateDefinition(TypeReference fieldType)
        {
            if (ReferenceEquals(FieldType, fieldType)) {
                return this;
            }

            return new FieldDefinition(Name, fieldType) {
                DeclaringType = DeclaringType,
                IsStatic = IsStatic,
                Module = Module,
                Value = Value,
            };
        }
    }
}

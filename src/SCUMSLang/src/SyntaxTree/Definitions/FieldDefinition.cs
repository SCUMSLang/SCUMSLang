using System;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class FieldDefinition : FieldReference
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.FieldDefinition;
        public object? Value { get; set; }

        [AllowNull]
        public override TypeReference FieldType {
            get => base.FieldType ?? throw SyntaxTreeThrowHelper.InvalidOperation(this);
            internal set => base.FieldType = value;
        }

        public FieldDefinition(string name, TypeReference fieldType)
            : base(name, fieldType) { }

        public FieldDefinition(string name, TypeReference fieldType, TypeReference declaringType)
            : base(name, fieldType) =>
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

        public new virtual FieldDefinition Resolve() =>
            CacheOrResolve(() => ParentBlock.Module.Resolve(this).Value);

        protected override IMember ResolveMember() =>
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
                ParentBlockContainer = ParentBlockContainer,
                Value = Value,
            };
        }
    }
}

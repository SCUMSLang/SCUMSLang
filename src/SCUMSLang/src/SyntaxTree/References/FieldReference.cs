using System;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class FieldReference : MemberReference, IMemberDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.FieldReference;

        private FieldDefinition? resolvedDefinition;

        public virtual TypeReference? FieldType { get; internal set; }
        public bool IsStatic { get; internal set; }

        public FieldReference(string name)
            : base(name) { }

        public FieldReference(string name, TypeReference fieldType)
            : base(name) =>
            FieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));

        public FieldReference(string name, TypeReference fieldType, TypeReference declaringType)
            : this(name, fieldType) =>
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

        public new virtual FieldDefinition Resolve()
        {
            return resolvedDefinition = resolvedDefinition
                ?? ParentBlock?.Module.Resolve(this)
                ?? throw new NotSupportedException();
        }

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitFieldReference(this);

        public FieldReference UpdateReference(TypeReference? fieldType)
        {
            if (ReferenceEquals(fieldType, FieldType)) {
                return this;
            }

            return new FieldReference(Name) {
                FieldType = fieldType,
                IsStatic = IsStatic,
                DeclaringType = DeclaringType,
                ParentBlock = ParentBlock
            };
        }
    }
}

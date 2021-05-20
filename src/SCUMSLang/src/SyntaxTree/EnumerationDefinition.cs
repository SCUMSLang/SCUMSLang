using System;
using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class EnumerationDefinition : TypeDefinition, INestedTypesProvider
    {
        public override IReadOnlyList<FieldDefinition> Fields =>
            FieldCollection ?? throw new InvalidOperationException();

        internal IReadOnlyList<FieldDefinition>? FieldCollection;

        public EnumerationDefinition(string name)
            : base(name) { }

        public new EnumerationDefinition Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitEnumerationDefinition(this);

        public EnumerationDefinition Update(TypeReference? baseType, IReadOnlyList<FieldDefinition> fields)
        {
            if (ReferenceEquals(baseType, BaseType) && ReferenceEquals(fields, FieldCollection)) {
                return this;
            }

            return new EnumerationDefinition(Name) {
                AllowOverwriteOnce = AllowOverwriteOnce,
                BaseType = baseType,
                DeclaringType = DeclaringType,
                FieldsAreConstants = FieldsAreConstants,
                IsEnum = IsEnum,
                Module = Module,
                FieldCollection = fields,
            };
        }

        #region INestedTypesProvider

        bool INestedTypesProvider.HasNestedTypes =>
            FieldsAreConstants;

        IEnumerable<TypeReference> INestedTypesProvider.GetNestedTypes()
        {
            foreach (var field in Fields) {
                var alias = CreateAliasDefinition(field.Name, baseType: this);
                yield return alias;
            }
        }

        #endregion
    }

    partial class Reference
    {
        public static EnumerationDefinition CreateEnumDefinition(
            string name,
            IEnumerable<string> fieldNames,
            TypeReference? valueType = null,
            bool fieldsAreConstants = false,
            bool allowOverwriteOnce = false)
        {
            var enumType = new EnumerationDefinition(name) {
                AllowOverwriteOnce = allowOverwriteOnce,
                FieldsAreConstants = fieldsAreConstants,
                IsEnum = true,
            };

            valueType ??= new TypeReference(SystemTypeLibrary.Sequences[SystemType.Integer]);
            enumType.FieldCollection = new EnumerationFieldCollection(valueType, enumType, fieldNames);
            return enumType;
        }
    }
}

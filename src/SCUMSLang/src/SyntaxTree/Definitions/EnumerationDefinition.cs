using System;
using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
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

        public EnumerationDefinition UpdateDefinition(TypeReference? baseType, IReadOnlyList<FieldDefinition> fields)
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
                ParentBlock = ParentBlock,
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
}

namespace SCUMSLang.SyntaxTree.References {
    partial class Reference
    {
        public static EnumerationDefinition CreateEnumDefinition(
            string name,
            IEnumerable<string> fieldNames,
            TypeReference? valueType = null,
            bool fieldsAreConstants = false,
            bool allowOverwriteOnce = false,
            BlockContainer? blockContainer = null)
        {
            blockContainer ??= new BlockContainer();

            var enumType = new EnumerationDefinition(name) {
                AllowOverwriteOnce = allowOverwriteOnce,
                FieldsAreConstants = fieldsAreConstants,
                IsEnum = true,
                ParentBlockContainer = blockContainer
            };

            valueType ??= new TypeReference(SystemTypeLibrary.Sequences[SystemType.Integer]) {
                ParentBlockContainer = blockContainer
            };

            enumType.FieldCollection = new EnumerationFieldCollection(
                valueType,
                enumType,
                fieldNames,
                blockContainer: blockContainer);

            return enumType;
        }
    }
}

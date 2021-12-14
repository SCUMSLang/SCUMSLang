using System;
using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public delegate IReadOnlyList<FieldDefinition> EnumerationFieldsCreator(EnumerationDefinition declaringType);

    public class EnumerationDefinition : TypeDefinition, INestedTypesProvider
    {
        public override IReadOnlyList<FieldDefinition> Fields { get; }

        public EnumerationDefinition(string name, EnumerationFieldsCreator fieldsCreator)
            : base(name)
        {
            if (fieldsCreator == null) {
                throw new ArgumentNullException(nameof(fieldsCreator));
            }

            Fields = fieldsCreator(this) ?? throw new ArgumentException("Fields factory provided null");
        }

        private EnumerationDefinition(string name, IReadOnlyList<FieldDefinition> fields)
            : base(name) =>
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));


        public new EnumerationDefinition Resolve() => this;

        protected override IMember ResolveMember() => Resolve();

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitEnumerationDefinition(this);

        public EnumerationDefinition UpdateDefinition(TypeReference? baseType, IReadOnlyList<FieldDefinition> fields)
        {
            if (ReferenceEquals(baseType, BaseType) && ReferenceEquals(fields, Fields)) {
                return this;
            }

            return new EnumerationDefinition(Name, fields) {
                AllowOverwriteOnce = AllowOverwriteOnce,
                BaseType = baseType,
                DeclaringType = DeclaringType,
                FieldsAreConstants = FieldsAreConstants,
                IsEnum = IsEnum,
                ParentBlockContainer = ParentBlockContainer
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

namespace SCUMSLang.SyntaxTree.References
{
    partial class Reference
    {
        public static EnumerationDefinition CreateEnumDefinition(
            string name,
            EnumerationFieldsCreator fieldsCreator,
            bool fieldsAreConstants = false,
            bool allowOverwriteOnce = false,
            BlockContainer? blockContainer = null,
            IFilePosition? filePosition = null) =>
            new EnumerationDefinition(name, fieldsCreator) {
                AllowOverwriteOnce = allowOverwriteOnce,
                FieldsAreConstants = fieldsAreConstants,
                IsEnum = true,
                ParentBlockContainer = blockContainer ?? new BlockContainer(),
                FilePosition = filePosition
            };

        public static EnumerationDefinition CreateEnumDefinition(
            string name,
            TypeReference fieldType,
            IList<string>? fieldNames,
            bool fieldsAreConstants = false,
            bool allowOverwriteOnce = false,
            BlockContainer? blockContainer = null,
            IFilePosition? enumPosition = null,
            IList<IFilePosition>? fieldNamePositions = null)
        {
            fieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
            blockContainer ??= new BlockContainer();

            FieldDefinition FieldCreator(string name, TypeReference fieldType, TypeReference declaringType, object? value) =>
                    new FieldDefinition(name, fieldType, declaringType) {
                        Value = value,
                        ParentBlockContainer = blockContainer,
                        FilePosition = fieldNamePositions?[fieldNames!.IndexOf(name)]
                    };

            EnumerationFieldCollection CreateFields(EnumerationDefinition declaringType) =>
                EnumerationFieldCollection.Of(
                    fieldType,
                    declaringType,
                    fieldNames,
                    FieldCreator);

            return new EnumerationDefinition(name, CreateFields) {
                AllowOverwriteOnce = allowOverwriteOnce,
                FieldsAreConstants = fieldsAreConstants,
                IsEnum = true,
                ParentBlockContainer = blockContainer,
                FilePosition = enumPosition
            };
        }
    }
}

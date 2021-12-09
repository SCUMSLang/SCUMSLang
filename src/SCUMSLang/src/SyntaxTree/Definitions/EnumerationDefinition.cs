using System;
using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public delegate IReadOnlyList<FieldDefinition> EnumerationFieldsProvider(EnumerationDefinition declaringType);

    public class EnumerationDefinition : TypeDefinition, INestedTypesProvider
    {
        public override IReadOnlyList<FieldDefinition> Fields { get; }

        public EnumerationDefinition(string name, EnumerationFieldsProvider fieldsCreator)
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


        public new EnumerationDefinition Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
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
                ParentBlock = ParentBlock
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
            EnumerationFieldsProvider fieldsCreator,
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
            IEnumerable<string> fieldNames,
            TypeReference? valueType = null,
            bool fieldsAreConstants = false,
            bool allowOverwriteOnce = false,
            BlockContainer? blockContainer = null,
            IFilePosition? filePosition = null)
        {
            valueType ??= CreateTypeReference(SystemType.Integer, blockContainer);
            blockContainer ??= new BlockContainer();

            EnumerationFieldCollection CreateEnumerationFieldCollection(EnumerationDefinition enumType) =>
                EnumerationFieldCollection.Of(
                    valueType,
                    enumType,
                    fieldNames,
                    blockContainer: blockContainer);

            return new EnumerationDefinition(name, CreateEnumerationFieldCollection) {
                AllowOverwriteOnce = allowOverwriteOnce,
                FieldsAreConstants = fieldsAreConstants,
                IsEnum = true,
                ParentBlockContainer = blockContainer,
                FilePosition = filePosition
            };
        }
    }
}

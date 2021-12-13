using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public delegate FieldDefinition FieldCreator(string name, TypeReference fieldType, EnumerationDefinition declaringType, object? value);

    public class EnumerationFieldCollection : Collection<FieldDefinition>
    {
        public static EnumerationFieldCollection Of(TypeReference fieldType, TypeReference declaringType, IEnumerable<string> nameList, BlockContainer? blockContainer)
        {
            var fieldValue = 0;
            var fields = new List<FieldDefinition>();

            foreach (var name in nameList) {
                fields.Add(new FieldDefinition(name, fieldType, declaringType) {
                    Value = fieldValue++,
                    ParentBlockContainer = blockContainer
                });
            }

            return new EnumerationFieldCollection(fieldType, declaringType, fields);
        }

        public static EnumerationFieldCollection Of(TypeReference fieldType, EnumerationDefinition declaringType, IEnumerable<string>? nameList, FieldCreator fieldCreator)
        {
            var fieldValue = 0;
            var fields = nameList?.Select(name => fieldCreator(name, fieldType, declaringType, fieldValue++)) ?? new FieldDefinition[0];
            return new EnumerationFieldCollection(fieldType, declaringType, fields);
        }

        public TypeReference FieldType { get; }
        public TypeReference DeclaringType { get; }

        public EnumerationFieldCollection(TypeReference fieldType, TypeReference declaringType)
        {
            FieldType = fieldType;
            DeclaringType = declaringType;
        }

        public EnumerationFieldCollection(TypeReference fieldType, TypeReference declaringType, IEnumerable<FieldDefinition> fields)
            : this(fieldType, declaringType)
        {
            foreach (var field in fields) {
                Add(field);
            }
        }
    }
}

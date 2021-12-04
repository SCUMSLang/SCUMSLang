using System.Collections.Generic;
using System.Collections.ObjectModel;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class EnumerationFieldCollection : Collection<FieldDefinition>
    {
        public TypeReference FieldType { get; }
        public TypeReference DeclaringType { get; }

        private int fieldValue;

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

        public EnumerationFieldCollection(TypeReference fieldType, TypeReference declaringType, IEnumerable<string> nameList, BlockContainer? blockContainer = null)
            : this(fieldType, declaringType)
        {
            foreach (var name in nameList) {
                Add(name, blockContainer);
            }
        }

        public void Add(string name, BlockContainer? blockContainer = null)
        {
            var fieldInfo = new FieldDefinition(name, FieldType, DeclaringType) {
                Value = fieldValue++,
                ParentBlockContainer = blockContainer
            };

            Add(fieldInfo);
        }
    }
}

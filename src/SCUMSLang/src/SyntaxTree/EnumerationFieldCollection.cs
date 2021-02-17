using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCUMSLang.SyntaxTree
{
    public class EnumerationFieldCollection : Collection<FieldDefinition>
    {
        public TypeReference FieldType { get; }
        public TypeReference DeclaringType { get; }

        private int fieldValue = 0;

        public EnumerationFieldCollection(TypeReference fieldType, TypeReference declaringType)
        {
            FieldType = fieldType;
            DeclaringType = declaringType;
        }

        public EnumerationFieldCollection(TypeReference fieldType, TypeReference declaringType, IEnumerable<string> nameList)
            : this(fieldType, declaringType)
        {
            foreach (var name in nameList) {
                Add(name);
            }
        }

        public void Add(string name)
        {
            var fieldInfo = new FieldDefinition(name, FieldType, DeclaringType) {
                Value = fieldValue++
            };

            Add(fieldInfo);
        }
    }
}

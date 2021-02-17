using System;

namespace SCUMSLang.SyntaxTree
{
    public abstract class TypeSpecification : TypeReference
    {
        public override SyntaxTreeNodeType NodeType => 
            SyntaxTreeNodeType.TypeSpecification;

        public TypeReference ElementType => 
            elementType;

        private TypeReference elementType;

        protected TypeSpecification(TypeReference elementType)
            : base(name: null) =>
            this.elementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
    }
}

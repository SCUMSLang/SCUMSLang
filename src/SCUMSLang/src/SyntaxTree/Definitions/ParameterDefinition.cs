using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class ParameterDefinition : ParameterReference, IConstantProvider
    {
        public bool HasConstant {
            get => !ReferenceEquals(Constant, Constants.Null);

            protected set {
                if (!value) {
                    Constant = Constants.Null;
                }
            }
        }

        public object Constant { get; private set; }
        public string? Name { get; private set; }

        [MemberNotNullWhen(true, nameof(Name))]
        public bool HasName => Name is not null;

        internal ParameterDefinition(TypeReference parameterType, object constant, string? name = null)
            : base(parameterType) =>
            Constant = constant;

        internal ParameterDefinition(TypeReference parameterType, string? name = null)
            : base(parameterType) =>
            Constant = Constants.Null;

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitParameterDefinition(this);

        public ParameterDefinition Update(TypeReference parameterType)
        {
            if (ReferenceEquals(parameterType, ParameterType)) {
                return this;
            }

            return new ParameterDefinition(parameterType) {
                Constant = Constant,
                Name = Name
            };
        }
    }
}

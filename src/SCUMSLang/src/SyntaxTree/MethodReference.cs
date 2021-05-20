using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class MethodReference : MemberReference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.MethodReference;

        public IReadOnlyList<ParameterReference> GenericParameters { get; }
        public IReadOnlyList<ParameterReference> Parameters { get; }
        public override TypeReference? DeclaringType { get; internal set; }

        public MethodReference(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            TypeReference? declaringType)
            : base(name)
        {
            GenericParameters = genericParameters ?? new List<ParameterReference>();
            Parameters = parameters ?? new List<ParameterReference>();
            DeclaringType = declaringType;
        }

        public MethodReference(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters)
            : this(name, genericParameters, parameters, declaringType: null) { }

        public new MethodDefinition Resolve() =>
            CacheOrResolve(() => Module.Resolve(this));

        protected override IMemberDefinition ResolveMemberDefinition() =>
           Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitMethodReference(this);

        public MethodReference UpdateReference(
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters)
        {
            if (ReferenceEquals(genericParameters, GenericParameters)
                && ReferenceEquals(parameters, Parameters)) {
                return this;
            }

            return new MethodReference(Name, genericParameters, parameters) {
                DeclaringType = DeclaringType,
                Module = Module
            };
        }
    }
}

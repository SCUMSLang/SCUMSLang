using System;
using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class MethodReference : MemberReference
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.MethodReference;
        public override string LongName => MemberFullName();
        public IReadOnlyList<ParameterDefinition> GenericParameters { get; }
        public IReadOnlyList<ParameterDefinition> Parameters { get; }
        public override TypeReference? DeclaringType { get; internal set; }

        private MethodDefinition? resolvedDefinition;

        public MethodReference(
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters,
            IReadOnlyList<ParameterDefinition>? parameters,
            TypeReference? declaringType)
            : base(name)
        {
            GenericParameters = genericParameters ?? new List<ParameterDefinition>();
            Parameters = parameters ?? new List<ParameterDefinition>();
            DeclaringType = declaringType;
        }

        public MethodReference(
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters,
            IReadOnlyList<ParameterDefinition>? parameters)
            : this(name, genericParameters, parameters, declaringType: null) { }

        protected override void ResolveDependencies()
        {
            var builder = new ResolveCollection();
            builder.IncludeDependencies(GenericParameters);
            builder.IncludeDependencies(Parameters);
            builder.Resolve();
        }

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();

        public new MethodDefinition Resolve()
        {
            ResolveDependencies();

            return resolvedDefinition = resolvedDefinition
                ?? Module?.Resolve(this)
                ?? throw new NotSupportedException();
        }

        public override bool Equals(object? obj) =>
            base.Equals(obj) && obj is MethodReference method
            && Enumerable.SequenceEqual(method.GenericParameters, GenericParameters)
            && Enumerable.SequenceEqual(method.Parameters, Parameters);

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitMethodReference(this);
    }
}

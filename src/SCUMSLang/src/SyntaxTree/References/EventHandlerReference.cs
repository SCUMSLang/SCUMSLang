using System;
using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class EventHandlerReference : MethodReference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.EventHandlerDefinition;

        public IReadOnlyList<MethodCallDefinition> Conditions { get; }

        private EventHandlerDefinition? resolvedDefinition;

        public EventHandlerReference(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions,
            TypeReference declaringType)
            : base(name, genericParameters, parameters, declaringType) =>
            Conditions = conditions ?? new List<MethodCallDefinition>();

        public EventHandlerReference(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions)
            : base(name, genericParameters, parameters) =>
            Conditions = conditions ?? new List<MethodCallDefinition>();

        public new EventHandlerDefinition Resolve() =>
            CacheOrResolve(() => ParentBlock.Module.Resolve(this).Value);

        protected override IMember ResolveMember() =>
            Resolve();

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitEventHandlerReference(this);

        public EventHandlerReference UpdateReference(
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions)
        {
            if (ReferenceEquals(genericParameters, GenericParameters)
                && ReferenceEquals(parameters, Parameters)
                && ReferenceEquals(conditions, Conditions)) {
                return this;
            }

            return new EventHandlerReference(Name, genericParameters, parameters, conditions) {
                DeclaringType = DeclaringType,
                ParentBlockContainer = ParentBlockContainer,
            };
        }
    }
}

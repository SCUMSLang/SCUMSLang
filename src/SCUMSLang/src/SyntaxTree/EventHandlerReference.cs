using System;
using System.Collections.Generic;
using System.Linq;

namespace SCUMSLang.SyntaxTree
{
    public class EventHandlerReference : MethodReference
    {
        public override SyntaxTreeNodeType NodeType => 
            SyntaxTreeNodeType.EventHandlerDefinition;

        public IReadOnlyList<MethodCallDefinition> Conditions { get; }

        private EventHandlerDefinition? resolvedDefinition;

        public EventHandlerReference(
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters,
            IReadOnlyList<ParameterDefinition>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions,
            TypeReference declaringType)
            : base(name, genericParameters, parameters, declaringType) =>
            Conditions = conditions ?? new List<MethodCallDefinition>();

        public EventHandlerReference(
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters,
            IReadOnlyList<ParameterDefinition>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions)
            : base(name, genericParameters, parameters) =>
            Conditions = conditions ?? new List<MethodCallDefinition>();

        protected override void ResolveDependencies() {
            base.ResolveDependencies();

            foreach (IResolvableDependencies condition in Conditions) {
                condition.ResolveDependencies();
            }
        }

        public new EventHandlerDefinition Resolve()
        {
            ResolveDependencies();

            return resolvedDefinition = resolvedDefinition
                ?? Module?.Resolve(this)
                ?? throw new NotSupportedException();
        }

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();

        public override bool Equals(object? obj) =>
            base.Equals(obj) && obj is EventHandlerReference eventHandler
            && Enumerable.SequenceEqual(eventHandler.Conditions, Conditions);

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), Conditions);
    }
}

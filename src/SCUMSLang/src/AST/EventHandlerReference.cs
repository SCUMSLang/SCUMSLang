using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EventHandlerReference : MethodReference
    {
        public override TreeTokenType TokenType => TreeTokenType.EventHandlerDefinition;
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

        public override bool Equals(object? obj)
        {
            if (!(obj is EventHandlerReference node)) {
                return false;
            }

            var equals = base.Equals(obj)
                && Enumerable.SequenceEqual(Conditions, node.Conditions);

            Trace.WriteLineIf(!equals, $"{nameof(EventHandlerReference)} not equals.");
            return equals;
        }

        public override int GetHashCode() => 
            HashCode.Combine(base.GetHashCode(), Conditions);
    }
}

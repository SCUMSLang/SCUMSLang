using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EventHandlerReference : FunctionReference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.EventHandler;
        public IReadOnlyList<FunctionCallReference> Conditions { get; }

        public EventHandlerReference(
            string name,
            IReadOnlyList<DeclarationReference>? genericParameters, 
            IReadOnlyList<DeclarationReference>? parameters,
            IReadOnlyList<FunctionCallReference>? conditions)
            : base(name, genericParameters, parameters) =>
            Conditions = conditions ?? new List<FunctionCallReference>();

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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.AST
{
    public class EventHandlerReferenceEqualityComparer : EqualityComparer<EventHandlerReference>
    {
        public new static EventHandlerReferenceEqualityComparer Default = new EventHandlerReferenceEqualityComparer();

        public virtual MethodReferenceEqualityComparer MethodComparer { get; }

        public EventHandlerReferenceEqualityComparer(MethodReferenceEqualityComparer? methodComparer = null) =>
            MethodComparer = methodComparer ?? MethodReferenceEqualityComparer.Default;

        public override bool Equals([AllowNull] EventHandlerReference x, [AllowNull] EventHandlerReference y) =>
            MethodComparer.Equals(x, y)
            && Enumerable.SequenceEqual(x.Conditions, y.Conditions);

        public override int GetHashCode([DisallowNull] EventHandlerReference obj) =>
            HashCode.Combine(obj.TokenType, obj.Name, obj.GenericParameters, obj.Parameters);

        public class OverloadComparer : EqualityComparer<EventHandlerReference>
        {
            public new static OverloadComparer Default = new OverloadComparer();

            public virtual MethodReferenceEqualityComparer.OverloadComparer MethodOverloadComparer { get; }

            public MethodCallDefinitionEqualityComparer MethodCallComparer {
                get {
                    if (methodCallComparer is null) {
                        methodCallComparer = MethodCallDefinitionEqualityComparer.Default;
                    }

                    return methodCallComparer;
                }

                set => methodCallComparer = value;
            }

            private MethodCallDefinitionEqualityComparer? methodCallComparer;

            public OverloadComparer(
                MethodReferenceEqualityComparer.OverloadComparer? methodOverloadComparer = null,
                ParameterReferenceEqualityComparer.OverloadComparer? parameterOverloadComparer = null)
            {
                MethodOverloadComparer = methodOverloadComparer ?? new MethodReferenceEqualityComparer.OverloadComparer { 
                    ParameterOverloadComaprer = ParameterReferenceEqualityComparer.OverloadComparer.Default
                };
            }

            public override bool Equals([AllowNull] EventHandlerReference x, [AllowNull] EventHandlerReference y) =>
                MethodOverloadComparer.Equals(x, y)
                && Enumerable.SequenceEqual(x.Conditions, y.Conditions, MethodCallComparer);

            public override int GetHashCode([DisallowNull] EventHandlerReference obj) =>
                HashCode.Combine(obj.TokenType, obj.Name, obj.GenericParameters, obj.Parameters);
        }
    }
}

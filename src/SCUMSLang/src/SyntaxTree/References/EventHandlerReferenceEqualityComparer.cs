using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree.References
{
    public class EventHandlerReferenceEqualityComparer : EqualityComparer<EventHandlerReference>
    {
        public new static EventHandlerReferenceEqualityComparer Default = new EventHandlerReferenceEqualityComparer();

        public virtual MethodReferenceEqualityComparer MethodComparer { get; }

        public EventHandlerReferenceEqualityComparer(MethodReferenceEqualityComparer? methodComparer = null) =>
            MethodComparer = methodComparer ?? MethodReferenceEqualityComparer.Default;

        public override bool Equals([AllowNull] EventHandlerReference x, [AllowNull] EventHandlerReference y) =>
            MethodComparer.Equals(x, y)
            && x.Conditions.SequenceEqual(y.Conditions);

        public override int GetHashCode([DisallowNull] EventHandlerReference obj) =>
            HashCode.Combine(obj.NodeType, obj.Name, obj.GenericParameters, obj.Parameters);

        public class OverloadComparer : EqualityComparer<EventHandlerReference>
        {
            public new static OverloadComparer Default = new OverloadComparer();

            public MethodReferenceEqualityComparer.ViaModuleResolve MethodOverloadComparer {
                get {
                    if (methodComparer is null) {
                        methodComparer = MethodReferenceEqualityComparer.ViaModuleResolve.Default;
                    }

                    return methodComparer;
                }

                set => methodComparer = value;
            }

            public MethodCallDefinitionEqualityComparer.OverloadComparer MethodCallOverloadComparer {
                get {
                    if (methodCallComparer is null) {
                        methodCallComparer = MethodCallDefinitionEqualityComparer.OverloadComparer.Default;
                    }

                    return methodCallComparer;
                }

                set => methodCallComparer = value;
            }

            private MethodReferenceEqualityComparer.ViaModuleResolve? methodComparer;
            private MethodCallDefinitionEqualityComparer.OverloadComparer? methodCallComparer;

            public override bool Equals([AllowNull] EventHandlerReference x, [AllowNull] EventHandlerReference y) =>
                MethodOverloadComparer.Equals(x, y)
                && x.Conditions.SequenceEqual(y.Conditions, MethodCallOverloadComparer);

            public override int GetHashCode([DisallowNull] EventHandlerReference obj) =>
                obj.GetHashCode();
        }
    }
}

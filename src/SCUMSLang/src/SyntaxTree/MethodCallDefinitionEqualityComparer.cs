﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.SyntaxTree
{
    public class MethodCallDefinitionEqualityComparer : EqualityComparer<MethodCallDefinition>
    {
        public new static MethodCallDefinitionEqualityComparer Default = new MethodCallDefinitionEqualityComparer();

        public override bool Equals([AllowNull] MethodCallDefinition x, [AllowNull] MethodCallDefinition y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Equals(y);

        public override int GetHashCode([DisallowNull] MethodCallDefinition obj) =>
            obj.GetHashCode();

        public class OverloadComparer : EqualityComparer<MethodCallDefinition>
        {
            public new static OverloadComparer Default = new OverloadComparer();

            public MethodReferenceEqualityComparer.ViaModuleResolve MethodOveroladComparer {
                get {
                    if (methodReferenceComparer is null) {
                        methodReferenceComparer = MethodReferenceEqualityComparer.ViaModuleResolve.Default;
                    }

                    return methodReferenceComparer;
                }

                set => methodReferenceComparer = value;
            }

            public ConstantDefinitionEqualityComparer.ViaModuleResolve ConstantDefinitionComparer {
                get {
                    if (constantDefinitionComparer is null) {
                        constantDefinitionComparer = ConstantDefinitionEqualityComparer.ViaModuleResolve.Default;
                    }

                    return constantDefinitionComparer;
                }

                set => constantDefinitionComparer = value;
            }

            private MethodReferenceEqualityComparer.ViaModuleResolve? methodReferenceComparer;
            private ConstantDefinitionEqualityComparer.ViaModuleResolve? constantDefinitionComparer;

            public override bool Equals([AllowNull] MethodCallDefinition x, [AllowNull] MethodCallDefinition y) =>
                ReferenceEquals(x, y) || !(x is null) && !(y is null)
                && MethodOveroladComparer.Equals(x.Method, y.Method)
                && Enumerable.SequenceEqual(x.GenericArguments, y.GenericArguments, ConstantDefinitionComparer)
                && Enumerable.SequenceEqual(x.Arguments, y.Arguments, ConstantDefinitionComparer);

            public override int GetHashCode([DisallowNull] MethodCallDefinition obj) =>
                HashCode.Combine(obj.NodeType, obj.Method, obj.GenericArguments, obj.Arguments);
        }
    }
}

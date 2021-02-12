using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class DeclarationReference : MemberReference, INameReservableReference, IScopableReference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.DeclarationReference;

        public Scope Scope { get; }
        public override TypeReference DeclaringType { get; }
        public string Name { get; }

        public DeclarationReference(Scope scope, TypeReference declaringType, string name)
        {
            Scope = scope;
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is DeclarationReference declaration)) {
                return false;
            }

            var equals = Scope == declaration.Scope
                && DeclaringType.Equals(declaration.DeclaringType)
                && Name == declaration.Name;

            Trace.WriteLineIf(!equals, $"{nameof(DeclarationReference)} not equals.");
            return equals;
        }

        protected override IMemberDefinition? ResolveDefinition() =>
            Resolve();

        public new virtual DeclarationDefinition? Resolve() {
            var module = Module ?? throw new NotSupportedException();
            return module.Resolve(this);
        }

        public override int GetHashCode() =>
            HashCode.Combine(ReferenceType, Scope, DeclaringType, Name);
    }
}

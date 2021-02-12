using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public sealed class AssignDefinition : Reference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.Assignment;

        public ConstantReference Constant { get; }
        public DeclarationReference Declaration { get; }

        public AssignDefinition(DeclarationReference declaration, ConstantReference constant)
        {
            Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is AssignDefinition assignment)) {
                return false;
            }

            var equals = Declaration.Equals(assignment.Declaration)
                && Constant.Equals(assignment.Constant);

            Trace.WriteLineIf(!equals, $"{nameof(AssignDefinition)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(ReferenceType, Declaration, Constant);
    }
}

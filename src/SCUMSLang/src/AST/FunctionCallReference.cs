using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class FunctionCallReference : Reference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.FunctionCall;

        public FunctionReference Function { get; }
        public IReadOnlyList<ConstantReference> GenericArguments { get; }
        public IReadOnlyList<ConstantReference> Arguments { get; }

        public FunctionCallReference(FunctionReference function, IReadOnlyList<ConstantReference>? genericArguments, IReadOnlyList<ConstantReference>? arguments)
        {
            Function = function;
            GenericArguments = genericArguments ?? new List<ConstantReference>();
            Arguments = arguments ?? new List<ConstantReference>();
        }
    }
}

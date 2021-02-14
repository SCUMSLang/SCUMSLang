using System.Collections.Generic;
using System.Linq;

namespace SCUMSLang.AST
{
    public class MethodCallDefinition : Reference, IResolvableDependencies
    {
        public override TreeTokenType TokenType => TreeTokenType.MethodCallDefinition;

        public MethodReference InferredMethod { get; }
        public IReadOnlyList<ConstantDefinition> GenericArguments { get; }
        public IReadOnlyList<ConstantDefinition> Arguments { get; }

        public MethodCallDefinition(
            string name,
            IReadOnlyList<ConstantDefinition>? genericArguments,
            IReadOnlyList<ConstantDefinition>? arguments,
            TypeReference methodDeclaringType)
        {
            InferredMethod = new MethodReference(
                name,
                genericArguments?.ToParameterDefinitionList(),
                arguments?.ToParameterDefinitionList(),
                methodDeclaringType);
            
            GenericArguments = genericArguments ?? new List<ConstantDefinition>();
            Arguments = arguments ?? new List<ConstantDefinition>();
        }

        protected void ResolveDependencies()
        {
            var builder = new ResolveCollection();
            builder.IncludeDependencies(GenericArguments);
            builder.IncludeDependencies(Arguments);
            builder.IncludeReference(InferredMethod);
            builder.Resolve();
        }

        void IResolvableDependencies.ResolveDependencies() =>
            ResolveDependencies();

        public override bool Equals(object? obj) =>
            obj is MethodCallDefinition methodCall
            && Equals(methodCall.InferredMethod, InferredMethod)
            && Enumerable.SequenceEqual(methodCall.GenericArguments, GenericArguments)
            && Enumerable.SequenceEqual(methodCall.Arguments, Arguments);
    }
}

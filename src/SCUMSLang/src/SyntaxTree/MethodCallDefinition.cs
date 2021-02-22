using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public sealed class MethodCallDefinition : Reference, IResolvableDependencies
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.MethodCallDefinition;

        //public MethodReference InferredMethod { get; }
        public IReadOnlyList<ConstantDefinition> GenericArguments { get; }
        public IReadOnlyList<ConstantDefinition> Arguments { get; }

        public MethodCallDefinition(
            string name,
            IReadOnlyList<ConstantDefinition>? genericArguments,
            IReadOnlyList<ConstantDefinition>? arguments)
        {
            //InferredMethod = new MethodReference(
            //    name,
            //    genericArguments?.ToParameterDefinitionList(),
            //    arguments?.ToParameterDefinitionList(),
            //    declaringType);
            
            GenericArguments = genericArguments ?? new List<ConstantDefinition>();
            Arguments = arguments ?? new List<ConstantDefinition>();
        }

        protected void ResolveDependencies()
        {
            var builder = new ResolveCollection();
            builder.IncludeDependencies(GenericArguments);
            builder.IncludeDependencies(Arguments);
            //builder.IncludeReference(InferredMethod);
            builder.Resolve();
        }

        void IResolvableDependencies.ResolveDependencies() =>
            ResolveDependencies();

        public override bool Equals(object? obj) =>
            obj is MethodCallDefinition methodCall
            //&& Equals(methodCall.InferredMethod, InferredMethod)
            && Enumerable.SequenceEqual(methodCall.GenericArguments, GenericArguments)
            && Enumerable.SequenceEqual(methodCall.Arguments, Arguments);

        internal protected override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitMethodCallDefinition(this);
    }
}

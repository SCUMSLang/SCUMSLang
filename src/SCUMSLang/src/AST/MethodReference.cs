using System;
using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class MethodReference : MemberReference
    {
        public override TreeTokenType TokenType => TreeTokenType.MethodReference;
        public override string FullName => MemberFullName();
        public IReadOnlyList<ParameterDefinition> GenericParameters { get; }
        public IReadOnlyList<ParameterDefinition> Parameters { get; }
        public override TypeReference DeclaringType { get; internal set; }

        private MethodDefinition? resolvedDefinition;

        public MethodReference(
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters,
            IReadOnlyList<ParameterDefinition>? parameters,
            TypeReference declaringType)
            : base(name)
        {
            GenericParameters = genericParameters ?? new List<ParameterDefinition>();
            Parameters = parameters ?? new List<ParameterDefinition>();
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
        }

        protected override void ResolveDependencies()
        {
            var builder = new ResolveCollection();
            builder.IncludeDependencies(GenericParameters);
            builder.IncludeDependencies(Parameters);
            builder.Resolve();
        }

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();

        public new MethodDefinition Resolve()
        {
            ResolveDependencies();

            return resolvedDefinition = resolvedDefinition
                ?? Module?.Resolve(this)
                ?? throw new NotSupportedException();
        }
    }
}

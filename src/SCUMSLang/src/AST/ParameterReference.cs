namespace SCUMSLang.AST
{
    public abstract class ParameterReference : Reference, IResolvableDependencies
    {
        public override TreeTokenType TokenType => TreeTokenType.ParameterReference;
        
        public string Name { get; set; }
        public TypeReference ParameterType { get; protected set; } = null!;

        internal ParameterReference(TypeReference parameterType)
        {
            Name = string.Empty;
            ParameterType = parameterType ?? throw new System.ArgumentNullException(nameof(parameterType));
        }

        internal ParameterReference(string name, TypeReference parameterType)
        {
            Name = name ?? string.Empty;
            ParameterType = parameterType ?? throw new System.ArgumentNullException(nameof(parameterType));
        }

        protected virtual void ResolveDependencies() =>
            ParameterType.Resolve();

        void IResolvableDependencies.ResolveDependencies() =>
            ResolveDependencies();

        public abstract ParameterDefinition Resolve();

        public override bool Equals(object? obj) =>
            base.Equals(obj) && obj is ParameterReference parameter
            && Equals(parameter.Name, Name)
            && Equals(parameter.ParameterType, ParameterType);

        public override string ToString() =>
            Name;
    }
}

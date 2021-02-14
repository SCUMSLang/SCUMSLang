namespace SCUMSLang.AST
{
    public class ParameterDefinition : ParameterReference, IConstantProvider
    {
        public bool HasConstant {
            get => !ReferenceEquals(constant, ConstantLibrary.Null);

            set {
                if (!value) {
                    constant = ConstantLibrary.Null;
                }
            }
        }

        public object Constant => constant;

        private object constant;

        internal ParameterDefinition(TypeReference parameterType, object constant)
            : base(parameterType) =>
            this.constant = constant;

        internal ParameterDefinition(TypeReference parameterType)
            : base(parameterType) =>
            constant = ConstantLibrary.Null;

        public ParameterDefinition(string name, TypeReference parameterType, object constant)
            : base(name, parameterType) =>
            this.constant = constant;

        public ParameterDefinition(string name, TypeReference parameterType)
            : base(name, parameterType) =>
            constant = ConstantLibrary.Null;

        public override ParameterDefinition Resolve()
        {
            ResolveDependencies();
            return this;
        }
    }
}

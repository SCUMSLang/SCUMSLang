using System.Runtime.Serialization;

namespace SCUMSLang.SyntaxTree
{
    public class ResolutionDefinitionNotFoundException : BlockEvaluatingException
    {
        protected ResolutionDefinitionNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public ResolutionDefinitionNotFoundException(string? message)
            : base(message) { }
    }
}

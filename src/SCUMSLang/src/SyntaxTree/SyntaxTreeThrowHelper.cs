using System;
using Teronis.Extensions;

namespace SCUMSLang.SyntaxTree
{
    internal static class SyntaxTreeThrowHelper
    {
        public static BlockEvaluatingException MemberNotFound(string typeName, string memberName, Func<string, BlockEvaluatingException>? errorFactory = null)
        {
            errorFactory ??= message => new BlockEvaluatingException(message);
            return errorFactory($"{typeName.UpperFirst()} by name '{memberName}' has not been found.");
        }

        public static BlockEvaluatingException TypeNotFound(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            MemberNotFound("type", name, errorFactory);

        public static BlockEvaluatingException FieldNotFound(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            MemberNotFound("field", name, errorFactory);

        public static BlockEvaluatingException MethodNotFound(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            MemberNotFound("method", name, errorFactory);

        public static BlockEvaluatingException EventHandlerdNotFound(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            MemberNotFound("event handler", name, errorFactory);

        public static DefinitionNotFoundException DefinitionNotFoundExceptionDelegate(string message) =>
            new DefinitionNotFoundException(message);
    }
}

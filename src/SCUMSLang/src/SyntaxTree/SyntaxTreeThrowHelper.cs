using System;
using Teronis.Extensions;

namespace SCUMSLang.SyntaxTree
{
    internal static class SyntaxTreeThrowHelper
    {
        public static BlockEvaluatingException CreateMemberNotFoundException(string typeName, string memberName, Func<string, BlockEvaluatingException>? errorFactory = null)
        {
            errorFactory ??= message => new BlockEvaluatingException(message);
            return errorFactory($"{typeName.UpperFirst()} by name '{memberName}' has not been found.");
        }

        public static BlockEvaluatingException CreateTypeNotFoundException(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            CreateMemberNotFoundException("type", name, errorFactory);

        public static BlockEvaluatingException CreateFieldNotFoundException(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            CreateMemberNotFoundException("field", name, errorFactory);

        public static BlockEvaluatingException CreateMethodNotFoundException(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            CreateMemberNotFoundException("method", name, errorFactory);

        public static BlockEvaluatingException CreateEventHandlerdNotFoundException(string name, Func<string, BlockEvaluatingException>? errorFactory = null) =>
            CreateMemberNotFoundException("event handler", name, errorFactory);

        public static ResolutionDefinitionNotFoundException ResolutionDefinitionNotFoundExceptionDelegate(string message) =>
            new ResolutionDefinitionNotFoundException(message);
    }
}

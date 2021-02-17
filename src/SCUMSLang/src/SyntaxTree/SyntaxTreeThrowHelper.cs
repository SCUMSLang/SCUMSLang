using System;
using Teronis.Extensions;

namespace SCUMSLang.SyntaxTree
{
    internal static class SyntaxTreeThrowHelper
    {
        public static ArgumentException CreateMemberNotFoundException(string typeName, string memberName, Func<string, ArgumentException>? errorFactory = null)
        {
            errorFactory ??= message => new ArgumentException(message);
            return errorFactory($"{typeName.UpperFirst()} by name '{memberName}' has not been found.");
        }

        public static ArgumentException CreateTypeNotFoundException(string name, Func<string, ArgumentException>? errorFactory = null) =>
            CreateMemberNotFoundException("type", name, errorFactory);

        public static ArgumentException CreateFieldNotFoundException(string name, Func<string, ArgumentException>? errorFactory = null) =>
            CreateMemberNotFoundException("field", name, errorFactory);

        public static ArgumentException CreateMethodNotFoundException(string name, Func<string, ArgumentException>? errorFactory = null) =>
            CreateMemberNotFoundException("method", name, errorFactory);

        public static ArgumentException CreateEventHandlerdNotFoundException(string name, Func<string, ArgumentException>? errorFactory = null) =>
            CreateMemberNotFoundException("event handler", name, errorFactory);

        public static ResolutionDefinitionNotFoundException ResolutionDefinitionNotFoundExceptionDelegate(string message) =>
            new ResolutionDefinitionNotFoundException(message);
    }
}

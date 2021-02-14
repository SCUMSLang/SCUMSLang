using System;
using Teronis.Extensions;

namespace SCUMSLang.AST
{
    internal static class TreeThrowHelper
    {
        public static ArgumentException CreateMemberNotFoundException(string typeName, string memberName) =>
            new ArgumentException($"{typeName.UpperFirst()} by name '{memberName}' has not been found.");

        public static ArgumentException CreateTypeNotFoundException(string name) =>
            CreateMemberNotFoundException("type", name);

        public static ArgumentException CreateFieldNotFoundException(string name) =>
            CreateMemberNotFoundException("field", name);

        public static ArgumentException CreateMethodNotFoundException(string name) =>
            CreateMemberNotFoundException("method", name);

        public static ArgumentException CreateEventHandlerdNotFoundException(string name) =>
            CreateMemberNotFoundException("event handler", name);

        public static ArgumentException CreateEnumerationFieldNotFoundException(string typeName) =>
            new ArgumentException($"A member could not be retrieved from enumeration {typeName}.");
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Text;
using SCUMSLang.SyntaxTree.References;
using Teronis.Extensions;

namespace SCUMSLang.SyntaxTree
{
    internal static class SyntaxTreeThrowHelper
    {
        public static BlockEvaluationException MemberNotFound(string typeName, string memberName, Func<string, IFilePosition?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null)
        {
            errorFactory ??= (message, filePosition) => new BlockEvaluationException(message) { FilePosition = filePosition };
            return errorFactory($"{typeName.UpperFirst()} by name '{memberName}' has not been found.", filePosition);
        }

        public static BlockEvaluationException TypeNotFound(string name, Func<string, IFilePosition?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null) =>
            MemberNotFound("type", name, errorFactory, filePosition);

        public static BlockEvaluationException FieldNotFound(string name, Func<string, IFilePosition?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null) =>
            MemberNotFound("field", name, errorFactory, filePosition);

        public static BlockEvaluationException MethodNotFound(string name, Func<string, IFilePosition?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null) =>
            MemberNotFound("method", name, errorFactory, filePosition);

        public static BlockEvaluationException EventHandlerdNotFound(string name, Func<string, IFilePosition?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null) =>
            MemberNotFound("event handler", name, errorFactory, filePosition);

        public static DefinitionNotFoundException DefinitionNotFoundExceptionDelegate(string message, IFilePosition? filePosition) =>
            new DefinitionNotFoundException(message) { FilePosition = filePosition };

        public static BlockEvaluationException AttributeMisposition(string? causedName = null) => causedName is null
            ? new BlockEvaluationException($"The attribute cannot be positioned before {causedName}.")
            : new BlockEvaluationException($"The attribute could not be attached to any node.");

        public static BlockEvaluationException NonNullBlockSetup() =>
            new BlockEvaluationException("The block cannot be set up twice.");

        public static InvalidOperationException InvalidOperation(Reference owner, string? message = null, [CallerMemberName] string? caller = null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"Improperly access of '{caller}'");

            if (owner is MemberReference member) {
                stringBuilder.Append($" in member '{member.Name}'");
            }

            stringBuilder.Append($" of class type {owner.GetType().Name}");

            if (message != null) {
                stringBuilder.Append($". {message}");
            }

            return new InvalidOperationException(stringBuilder.ToString());
        }
    }
}

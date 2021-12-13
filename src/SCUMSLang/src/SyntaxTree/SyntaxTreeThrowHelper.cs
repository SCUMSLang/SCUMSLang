using System;
using System.Runtime.CompilerServices;
using System.Text;
using SCUMSLang.SyntaxTree.References;
using Teronis.Extensions;

namespace SCUMSLang.SyntaxTree
{
    internal static class SyntaxTreeThrowHelper
    {
        public static BlockResolutionException BlockResolutionExceptionDelegate(string message, IFilePosition? filePosition, string? stackTrace)
        {
            var error = new BlockResolutionException(message) { FilePosition = filePosition };
            error.SetStackTrace(stackTrace);
            return error;
        }

        public static BlockEvaluationException BlockEvaluatingExceptionDelegate(string message, IFilePosition? filePosition, string? stackTrace)
        {
            var error = new BlockEvaluationException(message) { FilePosition = filePosition };
            error.SetStackTrace(stackTrace);
            return error;
        }

        public static BlockEvaluationException MemberNotFound(string typeName, string memberName, Func<string, IFilePosition?, string?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null, string? stackTrace = null)
        {
            if (errorFactory is null) {
                errorFactory = BlockEvaluatingExceptionDelegate;
            }

            return errorFactory($"{typeName.UpperFirst()} by name '{memberName}' has not been found.", filePosition, stackTrace);
        }

        public static BlockEvaluationException TypeNotFound(string name, Func<string, IFilePosition?, string?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null, string? stackTrace = null) =>
            MemberNotFound("type", name, errorFactory, filePosition, stackTrace);

        public static BlockEvaluationException FieldNotFound(string name, Func<string, IFilePosition?, string?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null, string? stackTrace = null) =>
            MemberNotFound("field", name, errorFactory, filePosition, stackTrace);

        public static BlockEvaluationException MethodNotFound(string name, Func<string, IFilePosition?, string?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null, string? stackTrace = null) =>
            MemberNotFound("method", name, errorFactory, filePosition, stackTrace);

        public static BlockEvaluationException EventHandlerdNotFound(string name, Func<string, IFilePosition?, string?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null, string? stackTrace = null) =>
            MemberNotFound("event handler", name, errorFactory, filePosition, stackTrace);

        public static BlockEvaluationException ModuleNotFound(string name, Func<string, IFilePosition?, string?, BlockEvaluationException>? errorFactory = null, IFilePosition? filePosition = null, string? stackTrace = null) =>
            MemberNotFound("module", name, errorFactory, filePosition, stackTrace);

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

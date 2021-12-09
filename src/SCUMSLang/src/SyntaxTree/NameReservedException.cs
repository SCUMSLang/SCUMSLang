using System;

namespace SCUMSLang.SyntaxTree
{
    public class NameReservedException : BlockEvaluationException
    {
        public string Name { get; }

        public NameReservedException(string name)
            : base($"The name '{name}' is already reserved.") =>
            Name = name;

        public NameReservedException(string name, string? message) 
            : base(message) =>
            Name = name;

        public NameReservedException(string name, string? message, Exception? innerException)
            : base(message, innerException) =>
            Name = name;
    }
}

using System;

namespace SCUMSLang.Tokenization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class SequenceAttribute : Attribute
    {
        public string Sequence { get; }

        public SequenceAttribute(string sequence) =>
            Sequence = sequence;
    }
}

using System;

namespace SCUMSLang.Tokenization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class SequenceExampleAttribute : Attribute
    {
        public string Sequence { get; }

        public SequenceExampleAttribute(string sequence) =>
            Sequence = sequence;
    }
}

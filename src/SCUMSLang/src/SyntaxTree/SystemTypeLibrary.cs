using System.Collections.Generic;
using SCUMSLang.Tokenization;
using static SCUMSLang.SCUMSLangTools;

namespace SCUMSLang.SyntaxTree
{
    public static class SystemTypeLibrary {
        public static IReadOnlyDictionary<SystemType, string> Sequences => sequences;

        private static Dictionary<SystemType, string> sequences;

        static SystemTypeLibrary() {
            sequences = new Dictionary<SystemType, string>();

            ForEachEnum<SystemType>(enumValue => {
                var memberInfo = GetEnumField(enumValue);

                if (TryGetAttribute<SequenceAttribute>(memberInfo, out var sequenceExample)) {
                    sequences[enumValue] = sequenceExample.Sequence;
                }
            });
        }
    }
}

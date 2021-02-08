using System.Collections.Generic;
using SCUMSLang.Tokenization;
using static SCUMSLang.SCUMSLangTools;

namespace SCUMSLang.AST
{
    public static class InBuiltTypeLibrary {
        public static IReadOnlyDictionary<InBuiltType, string> SequenceExamples => sequenceExamples;

        private static Dictionary<InBuiltType, string> sequenceExamples;

        static InBuiltTypeLibrary() {
            sequenceExamples = new Dictionary<InBuiltType, string>();

            ForEachEnum<InBuiltType>(enumValue => {
                var memberInfo = GetEnumField(enumValue);

                if (TryGetAttribute<SequenceExampleAttribute>(memberInfo, out var sequenceExample)) {
                    sequenceExamples[enumValue] = sequenceExample.Sequence;
                }
            });
        }
    }
}

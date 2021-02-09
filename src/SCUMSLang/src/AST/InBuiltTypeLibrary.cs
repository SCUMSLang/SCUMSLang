using System.Collections.Generic;
using SCUMSLang.Tokenization;
using static SCUMSLang.SCUMSLangTools;

namespace SCUMSLang.AST
{
    public static class InBuiltTypeLibrary {
        public static IReadOnlyDictionary<DefinitionType, string> Sequences => sequences;

        private static Dictionary<DefinitionType, string> sequences;

        static InBuiltTypeLibrary() {
            sequences = new Dictionary<DefinitionType, string>();

            ForEachEnum<DefinitionType>(enumValue => {
                var memberInfo = GetEnumField(enumValue);

                if (TryGetAttribute<SequenceAttribute>(memberInfo, out var sequenceExample)) {
                    sequences[enumValue] = sequenceExample.Sequence;
                }
            });
        }
    }
}

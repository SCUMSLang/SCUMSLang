using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public static class SystemTypes
    {
        public static TypeReference UInt8 = Reference.CreateTypeReference(SystemType.Byte.Sequence(), blockContainer: null);
        public static TypeReference UInt32 = Reference.CreateTypeReference(SystemType.Integer.Sequence(), blockContainer: null);
        public static TypeReference Boolean = Reference.CreateTypeReference(SystemType.Boolean.Sequence(), blockContainer: null);
        public static TypeReference String = Reference.CreateTypeReference(SystemType.String.Sequence(), blockContainer: null);
    }

    public class SystemTypesResolver
    {
        public TypeReference UInt8 => (uint8 ??= module.GetType(SystemType.Byte.Sequence()).ValueOrDefault()) ?? SystemTypes.UInt8;
        public TypeReference UInt32 => (uint32 ??= module.GetType(SystemType.Integer.Sequence()).ValueOrDefault()) ?? SystemTypes.UInt32;
        public TypeReference Boolean => (boolean ??= module.GetType(SystemType.Boolean.Sequence()).ValueOrDefault()) ?? SystemTypes.Boolean;
        public TypeReference String => (@string ??= module.GetType(SystemType.String.Sequence()).ValueOrDefault()) ?? SystemTypes.String;

        private TypeReference? uint8;
        private TypeReference? uint32;
        private TypeReference? boolean;
        private TypeReference? @string;
        private readonly ModuleDefinition module;

        public SystemTypesResolver(ModuleDefinition module) =>
            this.module = module;
    }
}

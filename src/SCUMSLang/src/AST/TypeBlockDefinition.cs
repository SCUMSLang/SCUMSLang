using System.Linq;
using Teronis.Collections.Specialized;
using Teronis;

namespace SCUMSLang.AST
{
    public abstract class TypeBlockDefinition : BlockDefinition
    {
        public override TreeTokenType TokenType => TreeTokenType.ModuleBlockDefinition;

        public TypeDefinition GetType(string shortName, bool isLongName = false)
        {
            IReadOnlyLinkedBucketList<string, Reference> typesByName;

            if (isLongName) {
                typesByName = ModuleTypes;
            } else {
                typesByName = BlockMembers;
            }

            var (success, bucket) = typesByName.Buckets.TryGetValue(shortName);

            if (!success) {
                throw TreeThrowHelper.CreateTypeNotFoundException(shortName);
            }

            var types = bucket.Cast<TypeDefinition>();
            var type = types.Single();
            return type;
        }
    }
}

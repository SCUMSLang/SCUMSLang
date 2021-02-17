using System.Linq;
using Teronis.Collections.Specialized;
using Teronis;

namespace SCUMSLang.SyntaxTree
{
    public abstract class TypeBlockDefinition : BlockDefinition, IReferenceResolver
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ModuleBlockDefinition;

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
                throw SyntaxTreeThrowHelper.CreateTypeNotFoundException(shortName);
            }

            var types = bucket.Cast<TypeDefinition>();
            var type = types.Single();
            return type;
        }

        protected abstract TypeDefinition Resolve(TypeReference type);
        protected abstract FieldDefinition Resolve(FieldReference field);
        protected abstract MethodDefinition Resolve(MethodReference method);
        protected abstract EventHandlerDefinition Resolve(EventHandlerReference eventHandler);

        TypeDefinition IReferenceResolver.Resolve(TypeReference type) => 
            Resolve(type);

        FieldDefinition IReferenceResolver.Resolve(FieldReference field) => 
            Resolve(field);

        MethodDefinition IReferenceResolver.Resolve(MethodReference method) => 
            Resolve(method);

        EventHandlerDefinition IReferenceResolver.Resolve(EventHandlerReference eventHandler) => 
            Resolve(eventHandler);
    }
}

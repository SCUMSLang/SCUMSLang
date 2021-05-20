using System.Linq;
using Teronis.Collections.Specialized;
using Teronis;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree
{
    public abstract class TypeBlockDefinition : BlockDefinition, IReferenceResolver
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeBlockDefinition;

        private TypeBlockOwnedReferenceResolver referenceResolver;

        public TypeBlockDefinition() =>
            referenceResolver = new TypeBlockOwnedReferenceResolver(this);

        public bool TryGetType(string shortName, bool isLongName, [MaybeNullWhen(false)] out TypeDefinition type) {
            IReadOnlyLinkedBucketList<string, Reference> typesByName;

            if (isLongName) {
                typesByName = ModuleTypeList;
            } else {
                typesByName = LocalMemberList;
            }

            var (success, bucket) = typesByName.Buckets.TryGetValue(shortName);

            if (!success) {
                type = null;
                return false;
            }

            var types = bucket.Cast<TypeDefinition>();
            type = types.Single();
            return true;
        }

        public TypeDefinition GetType(string shortName, bool isLongName = false)
        {
            if (!TryGetType(shortName, isLongName: isLongName, out var type)) {
                throw SyntaxTreeThrowHelper.TypeNotFound(shortName);
            }

            return type;
        }

        TypeDefinition IReferenceResolver.Resolve(TypeReference type) =>
            referenceResolver.Resolve(type);

        FieldDefinition IReferenceResolver.Resolve(FieldReference field) =>
            referenceResolver.Resolve(field);

        MethodDefinition IReferenceResolver.Resolve(MethodReference method) =>
            referenceResolver.Resolve(method);

        EventHandlerDefinition IReferenceResolver.Resolve(EventHandlerReference eventHandler) =>
            referenceResolver.Resolve(eventHandler);

        private class TypeBlockOwnedReferenceResolver : ReferenceResolver
        {
            public override LinkedBucketList<string, Reference> BlockMembers =>
                block.LocalMemberList;

            public override LinkedBucketList<string, TypeReference> CascadingTypes =>
                block.ModuleTypeList;

            private readonly BlockDefinition block;

            public TypeBlockOwnedReferenceResolver(BlockDefinition block) =>
                this.block = block ?? throw new ArgumentNullException(nameof(block));
        }
    }
}

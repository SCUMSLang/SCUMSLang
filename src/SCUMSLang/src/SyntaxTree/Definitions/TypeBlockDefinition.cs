using System.Linq;
using Teronis.Collections.Specialized;
using Teronis;
using System;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public abstract class TypeBlockDefinition : BlockDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeBlockDefinition;

        /// <summary>
        /// Resolves the references that are exclusively part of this block.
        /// </summary>
        public IReferenceResolver BlockReferenceResolver;

        public TypeBlockDefinition() =>
            BlockReferenceResolver = new TypeBlockReferenceResolver(this);

        public bool TryGetType(string shortName, bool isLongName, [MaybeNullWhen(false)] out TypeDefinition type)
        {
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

        private class TypeBlockReferenceResolver : ReferenceResolver
        {
            public override LinkedBucketList<string, Reference> BlockMembers =>
                block.LocalMemberList;

            public override LinkedBucketList<string, TypeReference> CascadingTypes =>
                block.ModuleTypeList;

            private readonly BlockDefinition block;

            public TypeBlockReferenceResolver(BlockDefinition block) =>
                this.block = block ?? throw new ArgumentNullException(nameof(block));
        }
    }
}

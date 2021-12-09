using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using Teronis;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public abstract class ReferenceResolver : IReferenceResolver
    {
        public abstract LinkedBucketList<string, Reference> BlockMembers { get; }
        public abstract LinkedBucketList<string, TypeReference> CascadingTypes { get; }

        private T? FindByMember<T>(IReadOnlyLinkedBucketList<string, Reference> references, MemberReference member)
            where T : MemberReference
        {
            var (success, bucket) = references.Buckets.TryGetValue(member.Name);

            if (!success) {
                return null;
            }

            var typedReferences = bucket.OfType<T>();

            return typedReferences.SingleOrDefault(typedReference =>
                TypeReferenceEqualityComparer.ViaResolveComparer.Default.Equals(typedReference.DeclaringType, member.DeclaringType));
        }

        public TypeDefinition Resolve(TypeReference type)
        {
            return FindByMember<TypeDefinition>(CascadingTypes, type)
                ?? throw SyntaxTreeThrowHelper.TypeNotFound(type.Name, SyntaxTreeThrowHelper.DefinitionNotFoundExceptionDelegate, type.FilePosition);
        }

        public FieldDefinition Resolve(FieldReference field)
        {
            FieldDefinition? fieldDefinition;

            if (field.DeclaringType is null) {
                // This allows to resolve field members in own block.
                // TODO: What about resolving a field member of static block in local block?
                fieldDefinition = FindByMember<FieldDefinition>(BlockMembers, field);
            } else {
                var declaringType = Resolve(field.DeclaringType);
                fieldDefinition = declaringType.Fields?.SingleOrDefault(x => x.Name == field.Name);
            }

            if (fieldDefinition is null) {
                throw SyntaxTreeThrowHelper.FieldNotFound(field.Name, SyntaxTreeThrowHelper.DefinitionNotFoundExceptionDelegate, field.FilePosition);
            }

            return fieldDefinition;
        }

        public MethodDefinition Resolve(MethodReference method)
        {
            MethodDefinition? methodDefinition = null;

            if (BlockMembers.TryGetBucket(method.Name, out var bucket)) {
                var typeDefinitions = bucket
                    .OfType<MethodDefinition>()
                    .Where(x => x.DeclaringType == method.DeclaringType);

                methodDefinition = typeDefinitions.SingleOrDefault(x =>
                    MethodOverloadEqualityComparer.Default.Equals(x, method));
            }

            if (methodDefinition is null) {
                throw SyntaxTreeThrowHelper.MethodNotFound(method.Name, SyntaxTreeThrowHelper.DefinitionNotFoundExceptionDelegate);
            }

            return methodDefinition;
        }

        public EventHandlerDefinition Resolve(EventHandlerReference eventHandler)
        {
            EventHandlerDefinition? eventHandlerDefinition = null;

            if (BlockMembers.TryGetBucket(eventHandler.Name, out var bucket)) {
                var typeDefinitions = bucket
                    .OfType<EventHandlerDefinition>()
                    .Where(x => x.DeclaringType == eventHandler.DeclaringType);

                eventHandlerDefinition = typeDefinitions.SingleOrDefault(x =>
                    EventHandlerReferenceEqualityComparer.OverloadComparer.Default.Equals(x, eventHandler));
            }

            if (eventHandlerDefinition is null) {
                throw SyntaxTreeThrowHelper.EventHandlerdNotFound(eventHandler.Name, SyntaxTreeThrowHelper.DefinitionNotFoundExceptionDelegate, eventHandler.FilePosition);
            }

            return eventHandlerDefinition;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using Teronis;
using Teronis.Collections.Generic;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public class ReferenceResolver : IReferenceResolver
    {
        private readonly static Type ReferenceType = typeof(Reference);
        private readonly static Type TypeReferenceType = typeof(TypeReference);
        private readonly static Type MethodDefinitionType = typeof(MethodDefinition);
        private readonly static Type EventHandlerDefinitionType = typeof(EventHandlerDefinition);

        public virtual IReadOnlyLinkedBucketList<(string, Type), Reference> Members => members;

        private LinkedBucketList<(string, Type), Reference> members = new LinkedBucketList<(string, Type), Reference>();

        private void AddBaseTypes(string name, Reference type, Type currentDotNetType, Type stoppingBaseType)
        {
            addBaseType:
            members.Add((name, currentDotNetType), type);

            var dotNetBaseType = currentDotNetType.BaseType;

            if (dotNetBaseType == null || dotNetBaseType == stoppingBaseType) {
                return;
            }

            currentDotNetType = dotNetBaseType;
            goto addBaseType;
        }

        public void AddType(string name, Reference type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            var dotNetType = type.GetType();

            if (dotNetType == TypeReferenceType) {
                throw new ArgumentException("Type cannot be a reference");
            }

            AddBaseTypes(name, type, dotNetType, ReferenceType);
        }

        private T? FindByMember<T>(IReadOnlyLinkedBucketList<(string, Type), Reference> references, MemberReference member, Type? exceptBaseType = null)
            where T : MemberReference
        {
            var (success, bucket) = references.Buckets.TryGetValue((member.Name, typeof(T)));

            if (!success) {
                return null;
            }

            var members = bucket.Cast<T>();

            if (exceptBaseType is not null) {
                members = members.Where(member => member.GetType() != exceptBaseType);
            }

            return members.SingleOrDefault(typedReference =>
                TypeReferenceEqualityComparer.AfterResolveComparer.Default.Equals(typedReference.DeclaringType, member.DeclaringType));
        }

        public ResolveResult<T> Resolve<T>(TypeReference type)
            where T : TypeReference
        {
            var typeDefinition = FindByMember<T>(Members, type, typeof(TypeReference));

            if (typeDefinition is null) {
                return ResolveResult.Failed<T>(SyntaxTreeThrowHelper.TypeNotFound(
                    type.Name,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    type.FilePosition,
                    Environment.StackTrace));
            }

            return ResolveResult.Success(typeDefinition);
        }

        public ResolveResult<TypeReference> Resolve(TypeReference type) =>
           Resolve<TypeReference>(type);

        public ResolveResult<FieldDefinition> Resolve(FieldReference field)
        {
            FieldDefinition? fieldDefinition;

            if (field.DeclaringType is null) {
                // This allows to resolve field members in own block.
                // TODO: What about resolving a field member of static block in local block?
                fieldDefinition = FindByMember<FieldDefinition>(Members, field);
            } else {
                var declaringType = Resolve<TypeDefinition>(field.DeclaringType).ValueOrDefault();
                fieldDefinition = declaringType?.Fields?.SingleOrDefault(x => x.Name == field.Name);
            }

            if (fieldDefinition is null) {
                return ResolveResult.Failed<FieldDefinition>(SyntaxTreeThrowHelper.FieldNotFound(
                    field.Name,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    field.FilePosition,
                    Environment.StackTrace));
            }

            return ResolveResult.Success(fieldDefinition);
        }

        public ResolveResult<MethodDefinition> Resolve(MethodReference method)
        {
            MethodDefinition? methodDefinition = null;

            if (Members.Buckets.TryGetValue((method.Name, MethodDefinitionType), out var bucket)) {
                var typeDefinitions = bucket
                    .Cast<MethodDefinition>()
                    .Where(x => x.DeclaringType == method.DeclaringType);

                methodDefinition = typeDefinitions.SingleOrDefault(x =>
                    MethodOverloadEqualityComparer.Default.Equals(x, method));
            }

            if (methodDefinition is null) {
                return ResolveResult.Failed<MethodDefinition>(SyntaxTreeThrowHelper.MethodNotFound(
                    method.Name,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    method.FilePosition,
                    Environment.StackTrace));
            }

            return ResolveResult.Success(methodDefinition);
        }

        public ResolveResult<EventHandlerDefinition> Resolve(EventHandlerReference eventHandler)
        {
            EventHandlerDefinition? eventHandlerDefinition = null;

            if (Members.Buckets.TryGetValue((eventHandler.Name, EventHandlerDefinitionType), out var bucket)) {
                var typeDefinitions = bucket
                    .Cast<EventHandlerDefinition>()
                    .Where(x => x.DeclaringType == eventHandler.DeclaringType);

                eventHandlerDefinition = typeDefinitions.SingleOrDefault(x =>
                    EventHandlerReferenceEqualityComparer.OverloadComparer.Default.Equals(x, eventHandler));
            }

            if (eventHandlerDefinition is null) {
                return ResolveResult.Failed<EventHandlerDefinition>(SyntaxTreeThrowHelper.EventHandlerdNotFound(
                    eventHandler.Name,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    eventHandler.FilePosition,
                    Environment.StackTrace));
            }

            return ResolveResult.Success(eventHandlerDefinition);
        }

        private bool TryGetMemberDefinitionBySelector<TDefinition>(
            string memberName,
            IReadOnlyLinkedBucketList<(string, Type), Reference> candidates,
            Func<IEnumerable<TDefinition>, TDefinition?> definitionSelector,
            out TDefinition definition)
            where TDefinition : class
        {
            if (!candidates.Buckets.TryGetValue((memberName, typeof(TDefinition)), out var bucket)) {
                definition = null!;
                return false;
            }

            definition = definitionSelector(bucket.Cast<TDefinition>())!;
            return true;
        }

        private ResolveResult<TDefinition> GetMemberDefinitionBySelector<TDefinition>(
            string memberName,
            IReadOnlyLinkedBucketList<(string, Type), Reference> candidates,
            Func<IEnumerable<TDefinition>, TDefinition?> definitionSelector,
            Func<string, Exception> notFoundErrorProvider)
            where TDefinition : class
        {
            if (TryGetMemberDefinitionBySelector(memberName, candidates, definitionSelector, out var definition) && definition is not null) {
                return ResolveResult.Success(definition);
            }

            return ResolveResult.Failed<TDefinition>(notFoundErrorProvider(Environment.StackTrace));
        }

        public ResolveResult<TypeDefinition> GetType(string typeName) =>
            GetMemberDefinitionBySelector<TypeDefinition>(
                typeName,
                Members,
                definitions => definitions.SingleOrDefault(),
                (stackTrace) => SyntaxTreeThrowHelper.TypeNotFound(
                    typeName,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    stackTrace: stackTrace));

        public ResolveResult<FieldDefinition> GetField(string fieldName) =>
            GetMemberDefinitionBySelector<FieldDefinition>(
                fieldName,
                Members,
                definitions => definitions.SingleOrDefault(),
                (stackTrace) => SyntaxTreeThrowHelper.FieldNotFound(
                    fieldName,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    stackTrace: stackTrace));

        public ResolveResult<MethodDefinition> GetMethod(string methodName) =>
            GetMemberDefinitionBySelector<MethodDefinition>(
                methodName,
                Members,
                definitions => definitions.SingleOrDefault(),
                (stackTrace) => SyntaxTreeThrowHelper.MethodNotFound(
                    methodName,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    stackTrace: stackTrace));

        public ResolveResult<MethodDefinition> GetMethod(MethodReference methodReference) =>
            GetMemberDefinitionBySelector<MethodDefinition>(
                methodReference.Name,
                Members,
                definitions => definitions.SingleOrDefault(definition =>
                    new MethodOverloadEqualityComparer(definition.ParentBlock.Module.ModuleReferenceResolver).Equals(definition, methodReference)),
                (stackTrace) => SyntaxTreeThrowHelper.MethodNotFound(
                    methodReference.Name,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    methodReference.FilePosition,
                    stackTrace));

        public ResolveResult<EventHandlerDefinition> GetEventHandler(string eventHandlerName) =>
            GetMemberDefinitionBySelector<EventHandlerDefinition>(
                eventHandlerName,
                Members,
                definitions => definitions.SingleOrDefault(),
                (stackTrace) => SyntaxTreeThrowHelper.MethodNotFound(
                    eventHandlerName,
                    SyntaxTreeThrowHelper.BlockResolutionExceptionDelegate,
                    stackTrace: stackTrace));
    }
}

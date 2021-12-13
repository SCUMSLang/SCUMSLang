using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree
{
    public abstract class ReferenceResolverPoolBase : IReferenceResolver, IEnumerable<IReferenceResolver>
    {
        protected abstract IEnumerable<IReferenceResolver> GetReferenceResolvers();

        private ResolveResult<T> getFirstOrThrowFirst<T>(Func<IReferenceResolver, ResolveResult<T>> resolveDelegate)
        {
            var errors = new List<Exception>();

            foreach (var referenceResolver in GetReferenceResolvers()) {
                var resolveResult = resolveDelegate(referenceResolver);

                if (!resolveResult.HasError) {
                    return resolveResult;
                }

                if (resolveResult.Error is BlockResolutionAggregateException aggregateError) {
                    errors.AddRange(aggregateError.InnerExceptions);
                } else if (resolveResult.Error is BlockResolutionException error) {
                    errors.Add(error);
                } else {
                    return ResolveResult.Failed<T>(resolveResult.Error);
                }
            }

            {
                // We only want show unique error messages.
                var aggregateError = new BlockResolutionAggregateException("One or more exceptions occured while resolving a definition.", errors.GroupBy(x => x.Message).Select(x => x.First()));
                aggregateError.SetStackTrace(Environment.StackTrace);
                return ResolveResult.Failed<T>(aggregateError);
            }
        }

        public ResolveResult<T> Resolve<T>(TypeReference type)
            where T : TypeReference =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.Resolve<T>(type));

        public ResolveResult<TypeReference> Resolve(TypeReference type) =>
           Resolve<TypeReference>(type);

        public ResolveResult<FieldDefinition> Resolve(FieldReference field) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.Resolve(field));

        public ResolveResult<MethodDefinition> Resolve(MethodReference method) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.Resolve(method));

        public ResolveResult<EventHandlerDefinition> Resolve(EventHandlerReference eventHandler) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.Resolve(eventHandler));

        public ResolveResult<TypeDefinition> GetType(string typeName) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.GetType(typeName));

        public ResolveResult<EventHandlerDefinition> GetEventHandler(string eventHandlerName) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.GetEventHandler(eventHandlerName));

        public ResolveResult<FieldDefinition> GetField(string fieldName) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.GetField(fieldName));

        public ResolveResult<MethodDefinition> GetMethod(string methodName) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.GetMethod(methodName));

        public ResolveResult<MethodDefinition> GetMethod(MethodReference methodReference) =>
            getFirstOrThrowFirst(referenceResolver => referenceResolver.GetMethod(methodReference));

        public IEnumerator<IReferenceResolver> GetEnumerator() =>
            GetReferenceResolvers().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree
{
    public partial class ReferenceResolverPool : IReferenceResolver, IEnumerable<IReferenceResolver>
    {
        private List<IReferenceResolver> modules;

        public ReferenceResolverPool() =>
            modules = new List<IReferenceResolver>();

        public void Add(IReferenceResolver module)
        {
            if (module is null) {
                throw new ArgumentNullException(nameof(module));
            }

            modules.Add(module);
        }

        protected virtual IEnumerable<IReferenceResolver> YieldReferenceResolvers()
        {
            foreach (var module in modules) {
                yield return module;
            }
        }

        private T getFirstOrThrowFirst<T>(Func<IReferenceResolver, T> resolveDelegate)
        {
            var errors = new List<Exception>();

            foreach (var module in YieldReferenceResolvers()) {
                try {
                    return resolveDelegate(module);
                } catch (DefinitionNotFoundException error) {
                    errors.Add(error);
                } catch (DefinitionNotFoundAggregateException error) {
                    errors.AddRange(error.InnerExceptions);
                } catch {
                    throw;
                }
            }

            // We only want show unique error messages.
            throw new DefinitionNotFoundAggregateException("One or more exceptions occured while trying to resolve definition.", errors.GroupBy(x => x.Message).Select(x => x.First()));
        }

        public TypeDefinition Resolve(TypeReference type) =>
            getFirstOrThrowFirst(module => module.Resolve(type));

        public FieldDefinition Resolve(FieldReference field) =>
            getFirstOrThrowFirst(module => module.Resolve(field));

        public MethodDefinition Resolve(MethodReference method) =>
            getFirstOrThrowFirst(module => module.Resolve(method));

        public EventHandlerDefinition Resolve(EventHandlerReference eventHandler) =>
            getFirstOrThrowFirst(module => module.Resolve(eventHandler));

        public IEnumerator<IReferenceResolver> GetEnumerator() =>
            YieldReferenceResolvers().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}

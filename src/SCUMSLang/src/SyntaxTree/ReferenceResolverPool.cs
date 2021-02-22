using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace SCUMSLang.SyntaxTree
{
    public class ReferenceResolverPool : IReferenceResolver, IEnumerable<IReferenceResolver>
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

        protected virtual IEnumerable<IReferenceResolver> YieldModules()
        {
            foreach (var module in modules) {
                yield return module;
            }
        }

        private T getFirstOrThrowFirst<T>(Func<IReferenceResolver, T> resolveDelegate)
        {
            Exception firstError = null!;

            foreach (var module in YieldModules()) {
                try {
                    return resolveDelegate(module);
                } catch (ResolutionDefinitionNotFoundException error) {
                    if (firstError is null) {
                        firstError = error;
                    }
                } catch {
                    throw;
                }
            }

            ExceptionDispatchInfo.Capture(firstError).Throw();
            throw new InvalidOperationException();
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
            YieldModules().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}

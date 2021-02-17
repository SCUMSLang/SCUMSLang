using System;
using System.Linq;
using Teronis;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public sealed partial class ModuleDefinition : TypeReference, IReferenceResolver
    {
        public TypeBlockDefinition Block => block;
        public override ModuleDefinition Module => this;
        public string FilePath { get; }

        private ModuleBlockDefinition block;
        private IReferenceResolver referenceResolver;

        public ModuleDefinition(ModuleParameters? parameters)
            : base(name: string.Empty)
        {
            block = new ModuleBlockDefinition(this);
            FilePath = parameters?.FilePath ?? string.Empty;

            if (parameters?.ReferenceResolver is null) {
                referenceResolver = block;
            } else {
                var referenceResolverPool = new ReferenceResolverPool();
                referenceResolverPool.AddModuleResolver(block);
                referenceResolverPool.AddModuleResolver(parameters.ReferenceResolver);
                referenceResolver = referenceResolverPool;
            }
        }

        public ModuleDefinition()
            : this(parameters: null) { }

        public TypeDefinition Resolve(TypeReference type) =>
            referenceResolver.Resolve(type);

        public FieldDefinition Resolve(FieldReference field) =>
            referenceResolver.Resolve(field);

        public MethodDefinition Resolve(MethodReference method) =>
            referenceResolver.Resolve(method);

        public EventHandlerDefinition Resolve(EventHandlerReference method) =>
            referenceResolver.Resolve(method);

        public void Import(MemberReference member) =>
            block.Import(member);

        internal class ModuleBlockDefinition : TypeBlockDefinition
        {
            public override SyntaxTreeNodeType NodeType =>
                SyntaxTreeNodeType.ModuleBlockDefinition;

            public override Scope Scope =>
                Scope.Static;

            public override ModuleDefinition Module { get; }
            public override TypeReference DeclaringType => Module;

            public override string Name =>
                string.Empty;

            protected override BlockDefinition ParentBlock => this;
            protected override LinkedBucketList<string, TypeReference> ModuleTypes { get; }

            public ModuleBlockDefinition(ModuleDefinition module)
            {
                ModuleTypes = new LinkedBucketList<string, TypeReference>();
                Module = module;
            }

            private T? findSingleReference<T>(IReadOnlyLinkedBucketList<string, Reference> references, MemberReference type)
                where T : class
            {
                var (success, bucket) = references.Buckets.TryGetValue(type.Name);

                if (!success) {
                    return null;
                }

                var typeDefinitions = bucket.OfType<T>();

                return typeDefinitions.SingleOrDefault(typeDefinition => {
                    return type.DeclaringType == type.DeclaringType;
                });
            }

            protected override TypeDefinition Resolve(TypeReference type)
            {
                var typeDefinition = findSingleReference<TypeDefinition>(ModuleTypes, type)
                    ?? throw SyntaxTreeThrowHelper.CreateTypeNotFoundException(type.LongName);

                return typeDefinition.Resolve();
            }

            protected override FieldDefinition Resolve(FieldReference field)
            {
                FieldDefinition? fieldDefinition;

                if (field.DeclaringType is null) {
                    fieldDefinition = findSingleReference<FieldDefinition>(BlockMembers, field);
                } else {
                    var declaringType = Resolve(field.DeclaringType);
                    fieldDefinition = declaringType.Fields.SingleOrDefault(x => x.Name == field.Name);
                }

                if (fieldDefinition is null) {
                    throw SyntaxTreeThrowHelper.CreateFieldNotFoundException(field.Name, SyntaxTreeThrowHelper.ResolutionDefinitionNotFoundExceptionDelegate);
                }

                return fieldDefinition.Resolve();
            }

            protected override MethodDefinition Resolve(MethodReference method)
            {
                MethodDefinition? methodDefinition = null;

                if (BlockMembers.TryGetBucket(method.Name, out var bucket)) {
                    var typeDefinitions = bucket
                        .OfType<MethodDefinition>()
                        .Where(x => x.DeclaringType == method.DeclaringType);

                    methodDefinition = typeDefinitions.SingleOrDefault(x => {
                        return MethodReferenceEqualityComparer.OverloadComparer.Default.Equals(x, method);
                    });
                }

                if (methodDefinition is null) {
                    throw SyntaxTreeThrowHelper.CreateMethodNotFoundException(method.Name, SyntaxTreeThrowHelper.ResolutionDefinitionNotFoundExceptionDelegate);
                }

                return methodDefinition.Resolve();
            }

            protected override EventHandlerDefinition Resolve(EventHandlerReference eventHandler)
            {
                EventHandlerDefinition? eventHandlerDefinition = null;

                if (BlockMembers.TryGetBucket(eventHandler.Name, out var bucket)) {
                    var typeDefinitions = bucket
                        .OfType<EventHandlerDefinition>()
                        .Where(x => x.DeclaringType == eventHandler.DeclaringType);

                    eventHandlerDefinition = typeDefinitions.SingleOrDefault(x => {
                        return EventHandlerReferenceEqualityComparer.OverloadComparer.Default.Equals(x, eventHandler);
                    });
                }

                if (eventHandlerDefinition is null) {
                    throw SyntaxTreeThrowHelper.CreateEventHandlerdNotFoundException(eventHandler.Name, SyntaxTreeThrowHelper.ResolutionDefinitionNotFoundExceptionDelegate);
                }

                return eventHandlerDefinition.Resolve();
            }

            public void Import(MemberReference member) =>
                _ = TryAddMember(member);
        }
    }
}

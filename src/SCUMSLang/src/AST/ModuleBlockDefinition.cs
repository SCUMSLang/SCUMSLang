using System;
using System.IO;
using System.Linq;
using Teronis;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public sealed partial class ModuleDefinition : TypeReference
    {
        public TypeBlockDefinition Block => block;
        public override ModuleDefinition? Module => this;
        public string FilePath { get; private set; } = null!;

        public string? DirectoryName =>
            Path.GetDirectoryName(FilePath);

        private ModuleBlockDefinition block;

        public ModuleDefinition(ModuleParameters _)
            : base(name: string.Empty) =>
            block = new ModuleBlockDefinition(this);
        //FilePath = parameters.FilePath ?? string.Empty

        public ModuleDefinition()
            : base(name: string.Empty) =>
            block = new ModuleBlockDefinition(this);

        public TypeDefinition Resolve(TypeReference type) =>
            block.Resolve(type);

        public FieldDefinition Resolve(FieldReference field) =>
            block.Resolve(field);

        public MethodDefinition Resolve(MethodReference method) =>
            block.Resolve(method);

        public EventHandlerDefinition Resolve(EventHandlerReference method) =>
            block.Resolve(method);

        public void Import(MemberReference member) =>
            block.Import(member);

        internal class ModuleBlockDefinition : TypeBlockDefinition
        {
            public override TreeTokenType TokenType =>
                TreeTokenType.ModuleBlockDefinition;

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

            private T? findReference<T>(IReadOnlyLinkedBucketList<string, Reference> references, MemberReference type)
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

            public TypeDefinition Resolve(TypeReference type)
            {
                var typeDefinition = findReference<TypeDefinition>(ModuleTypes, type)
                    ?? throw TreeThrowHelper.CreateTypeNotFoundException(type.FullName);

                return typeDefinition.Resolve();
            }

            public FieldDefinition Resolve(FieldReference field)
            {
                FieldDefinition? fieldDefinition;

                if (field.DeclaringType is null) {
                    fieldDefinition = findReference<FieldDefinition>(BlockMembers, field);
                } else {
                    var declaringType = Resolve(field.DeclaringType);
                    fieldDefinition = declaringType.Fields.SingleOrDefault(x => x.Name == field.Name);
                }

                if (fieldDefinition is null) {
                    throw TreeThrowHelper.CreateFieldNotFoundException(field.Name);
                }

                return fieldDefinition.Resolve();
            }

            public MethodDefinition Resolve(MethodReference method)
            {
                if (!BlockMembers.TryGetBucket(method.Name, out var bucket)) {
                    throw TreeThrowHelper.CreateMethodNotFoundException(method.Name);
                }

                var typeDefinitions = bucket
                    .OfType<MethodDefinition>()
                    .Where(x => x.DeclaringType == method.DeclaringType);

                var methodDefinition = typeDefinitions.SingleOrDefault(x => {
                    return MethodReferenceEqualityComparer.OverloadComparer.Default.Equals(x, method);
                }) ?? throw TreeThrowHelper.CreateMethodNotFoundException(method.Name);

                return methodDefinition.Resolve();
            }

            public EventHandlerDefinition Resolve(EventHandlerReference eventHandler)
            {
                if (!BlockMembers.TryGetBucket(eventHandler.Name, out var bucket)) {
                    throw new ArgumentException($"Condition by name '{eventHandler.Name}' does not exist.");
                }

                var typeDefinitions = bucket
                    .OfType<EventHandlerDefinition>()
                    .Where(x => x.DeclaringType == eventHandler.DeclaringType);

                var eventHandlerDefinition = typeDefinitions.SingleOrDefault(x => {
                    return EventHandlerReferenceEqualityComparer.OverloadComparer.Default.Equals(x, eventHandler);
                }) ?? throw TreeThrowHelper.CreateEventHandlerdNotFoundException(eventHandler.Name);

                return eventHandlerDefinition.Resolve();
            }

            public void Import(MemberReference member) =>
                _ = TryAddMember(member);
        }
    }
}

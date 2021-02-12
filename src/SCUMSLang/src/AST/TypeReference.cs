using System;

namespace SCUMSLang.AST
{
    public class TypeReference : MemberReference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.TypeReference;

        public string Name { get; }
        public virtual SystemType SystemType { get; }

        public override ModuleDefinition? Module => 
            module;

        private ModuleDefinition? module;

        protected TypeReference(string name) =>
            Name = name;

        protected TypeReference(string name, SystemType systemType) {
            Name = name;
            SystemType = systemType;
        }

        public TypeReference(string name, SystemType systemType, ModuleDefinition module) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SystemType = systemType;
            this.module = module ?? throw new ArgumentNullException(nameof(module));
        }

        protected override IMemberDefinition ResolveDefinition() => throw new NotImplementedException();

        public new TypeDefinition? Resolve() {
            var module = Module ?? throw new NotSupportedException();
            return module.Resolve(this);
        }
    }
}

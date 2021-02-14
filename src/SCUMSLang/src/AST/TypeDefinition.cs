using System;
using System.Collections.Generic;
using System.Linq;

namespace SCUMSLang.AST
{
    public sealed class TypeDefinition : TypeReference, INameReservableReference, IOverloadableReference, IMemberDefinition
    {
        public static TypeDefinition CreateEnumDefinition(
            ModuleDefinition module,
            string name,
            IEnumerable<string> names,
            TypeReference? valueType = null,
            bool usableAsConstants = false,
            bool allowOverwriteOnce = false)
        {
            var enumType = new TypeDefinition(module, name) {
                IsEnum = true,
                FieldsAreConstants = usableAsConstants,
                AllowOverwriteOnce = allowOverwriteOnce,
            };

            valueType ??= new TypeReference(SystemTypeLibrary.Sequences[SystemType.Integer], module);
            enumType.Fields = new EnumerationFieldCollection(valueType, enumType, names);
            return enumType;
        }

        public static TypeDefinition CreateAliasDefinition(
            ModuleDefinition module,
            string name,
            TypeReference sourceType)
        {
            var alias = new TypeDefinition(module, name) {
                IsAlias = true,
                BaseType = sourceType,
            };

            return alias;
        }

        public override TreeTokenType TokenType => TreeTokenType.TypeDefinition;

        public TypeReference? BaseType { get; internal set; }
        public bool IsEnum { get; internal set; }
        public bool IsAlias { get; internal set; }

        public IReadOnlyList<FieldDefinition> Fields {
            get {
                if (fields is null) {
                    fields = new List<FieldDefinition>();
                }

                return fields;
            }

            internal set => fields = value;
        }

        public bool FieldsAreConstants { get; internal set; }

        internal bool AllowOverwriteOnce { get; set; }

        private IReadOnlyList<FieldDefinition>? fields;

        public TypeDefinition(ModuleDefinition module, string name)
            : base(name, module) { }

        public string? GetEnumerationName(object value)
        {
            if (value is string valueString) {
                return valueString;
            }

            return fields?.SingleOrDefault(x => Equals(x.Value, value)).Name;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj) && obj is TypeDefinition type
                && type.FieldsAreConstants == FieldsAreConstants
                && type.IsEnum == IsEnum
                && type.IsAlias == type.IsAlias
                && Equals(type.BaseType, BaseType)
                && MemberReferenceEqualityComparer.ShallowComparer.Default.Equals(type.DeclaringType, DeclaringType)
                && Enumerable.SequenceEqual(type.Fields, Fields);
        }

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), TokenType, Name);

        public new TypeDefinition Resolve()
        {
            ResolveDependencies();
            return this;
        }

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();

        public override TypeDefinition ResolveNonAlias() =>
            ResolveNonAlias(this);

        #region IConditionalNameReservableNode

        OverloadConflictResult IOverloadableReference.SolveConflict(BlockDefinition blockNode)
        {
            var candidates = blockNode.GetMembersCasted<TypeDefinition>(Name);

            if (candidates is null) {
                return OverloadConflictResult.True;
            } else if (candidates.Count == 1) {
                var candidate = candidates[0];

                if (candidate.AllowOverwriteOnce && candidate.Equals(this)) {
                    AllowOverwriteOnce = false;
                    return OverloadConflictResult.Skip;
                } else {
                    return OverloadConflictResult.False;
                }
            }

            throw new NotImplementedException("More than two type definition with the name have been found that got name reserved without checking.");
        }

        #endregion
    }
}

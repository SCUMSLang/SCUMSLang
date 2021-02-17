using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCUMSLang.SyntaxTree
{
    public sealed class TypeDefinition : TypeReference, INameReservableReference, IOverloadableReference, IMemberDefinition, INestedTypesProvider
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

            valueType ??= new TypeReference(module, SystemTypeLibrary.Sequences[SystemType.Integer]);
            enumType.Fields = new EnumerationFieldCollection(valueType, enumType, names);
            return enumType;
        }

        public static TypeDefinition CreateAliasDefinition(
            ModuleDefinition module,
            string name,
            TypeReference baseType)
        {
            var alias = new TypeDefinition(module, name) {
                IsAlias = true,
                BaseType = baseType,
            };

            return alias;
        }

        public static TypeDefinition CreateArrayDefinition(
            ModuleDefinition module,
            TypeReference nestedType)
        { 
         var array = new TypeDefinition(module,   
        }

        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.TypeDefinition;

        public TypeReference? BaseType { get; internal set; }
        public IReadOnlyList<TypeReference>? NestedTypes { get; private set; }
        public bool IsEnum { get; internal set; }
        public bool IsAlias { get; internal set; }
        public bool IsArray { get; internal set; }

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
        private readonly Collection<TypeReference> nestedTypes;

        public TypeDefinition(ModuleDefinition module, string name)
            : base(module, name) { }

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
                && type.IsAlias == IsAlias
                && type.IsArray == IsArray
                && Equals(type.BaseType, BaseType)
                && MemberReferenceEqualityComparer.ShallowComparer.Default.Equals(type.DeclaringType, DeclaringType)
                && Enumerable.SequenceEqual(type.Fields, Fields);
        }

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), NodeType, Name);

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

        #region IAliasesHavingReference

        bool INestedTypesProvider.HasNestedTypes =>
            FieldsAreConstants;

        IEnumerable<TypeReference> INestedTypesProvider.GetNestedTypes()
        {
            foreach (var field in Fields) {
                var alias = CreateAliasDefinition(field.Module, field.Name, baseType: field.DeclaringType);
                yield return alias;
            }
        }

        #endregion
    }
}

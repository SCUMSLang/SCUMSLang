using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class TypeDefinition : TypeReference, INameReservableReference, IOverloadableReference, ICollectibleMember, ITypeDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeDefinition;

        public override TypeReference? BaseType { get; internal init; }
        public bool IsEnum { get; internal init; }
        public override bool IsAlias { get; internal init; }
        public virtual IReadOnlyList<FieldDefinition>? Fields { get; }

        [MemberNotNullWhen(true, nameof(Fields))]
        public bool HasFields =>
            !(Fields is null) && Fields.Count > 0;

        public bool FieldsAreConstants { get; internal set; }

        internal bool AllowOverwriteOnce { get; set; }

        bool ITypeDefinition.AllowOverwriteOnce =>
            AllowOverwriteOnce;

        public TypeDefinition(string name)
            : base(name) { }

        public bool TryGetFieldByName(string fieldName, [MaybeNullWhen(false)] out FieldDefinition field)
        {
            if (!HasFields) {
                field = null;
                return false;
            }

            field = Fields.SingleOrDefault(x => Equals(x.Name, fieldName));
            return field is not null;
        }

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), NodeType, Name);


        public new TypeDefinition Resolve() =>
            Resolve<TypeDefinition>();

        protected override IMember ResolveMember() => Resolve();

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitTypeDefinition(this);

        public TypeDefinition UpdateDefinition(TypeReference? baseType)
        {
            if (ReferenceEquals(baseType, BaseType)) {
                return this;
            }

            return new TypeDefinition(Name) {
                AllowOverwriteOnce = AllowOverwriteOnce,
                BaseType = baseType,
                DeclaringType = DeclaringType,
                IsAlias = IsAlias,
                IsArray = IsArray,
                ParentBlockContainer = ParentBlockContainer
            };
        }

        OverloadConflictResult IOverloadableReference.SolveConflict(BlockDefinition blockNode)
        {
            var candidates = blockNode.BlocksMembersByName<TypeDefinition>(Name);

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
    }
}

namespace SCUMSLang.SyntaxTree.References
{
    partial class Reference
    {
        public static TypeDefinition CreateTypeDefinition(string name, bool allowOverwriteOnce, BlockContainer? blockContainer) =>
            new TypeDefinition(name) { ParentBlockContainer = blockContainer, AllowOverwriteOnce = allowOverwriteOnce };

        public static TypeDefinition CreateTypeDefinition(SystemType systemType, bool allowOverwriteOnce, BlockContainer? blockContainer) =>
            CreateTypeDefinition(Sequences.SystemTypes[systemType], allowOverwriteOnce, blockContainer);

        public static TypeDefinition CreateAliasDefinition(
            string name,
            TypeReference baseType,
            BlockContainer? blockContainer = null)
        {
            return new TypeDefinition(name) {
                IsAlias = true,
                BaseType = baseType,
                ParentBlockContainer = blockContainer
            };
        }
    }
}

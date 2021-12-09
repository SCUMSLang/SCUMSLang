using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class TypeDefinition : TypeReference, INameReservableReference, IOverloadableReference, IMemberDefinition, ITypeDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeDefinition;

        public TypeReference? BaseType { get; internal set; }
        public bool IsEnum { get; internal set; }
        public bool IsAlias { get; internal set; }
        public virtual IReadOnlyList<FieldDefinition>? Fields { get; }

        [AllowNull]
        public override BlockDefinition ParentBlock {
            get => ParentBlockContainer.Block ?? BaseType?.ParentBlock ?? throw new InvalidOperationException();
            internal set => ParentBlockContainer.Block = value;
        }

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
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
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
                ParentBlock = ParentBlock
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
        public static TypeReference CreateTypeDefinition(string name, BlockContainer? blockContainer, bool allowOverwriteOnce) =>
            new TypeDefinition(name) { ParentBlockContainer = blockContainer, AllowOverwriteOnce = allowOverwriteOnce };

        public static TypeReference CreateTypeDefinition(SystemType systemType, BlockContainer? blockContainer, bool allowOverwriteOnce) =>
            CreateTypeDefinition(SystemTypeLibrary.Sequences[systemType], blockContainer, allowOverwriteOnce);

        public static TypeDefinition CreateAliasDefinition(
            string name,
            TypeReference baseType,
            BlockContainer? blockContainer = null)
        {
            var alias = new TypeDefinition(name) {
                IsAlias = true,
                BaseType = baseType,
                ParentBlockContainer = blockContainer
            };

            return alias;
        }
    }
}

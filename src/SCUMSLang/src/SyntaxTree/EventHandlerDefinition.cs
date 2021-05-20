using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class EventHandlerDefinition : EventHandlerReference, IMemberDefinition, IBlockHolder
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.EventHandlerDefinition;
        public BlockDefinition? Block { get; internal set; }
        bool IBlockHolder.IsExpandable => true;

        BlockDefinition? IBlockHolder.Block {
            get => Block;
            set => Block = value;
        }

        public EventHandlerDefinition(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions,
            TypeReference declaringType)
            : base(name, genericParameters, parameters, conditions, declaringType) { }

        public EventHandlerDefinition(
            string name,
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions)
            : base(name, genericParameters, parameters, conditions) { }

        public new EventHandlerDefinition Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
             visitor.VisitEventHandlerDefinition(this);

        public EventHandlerDefinition UpdateDefinition(
            IReadOnlyList<ParameterReference>? genericParameters,
            IReadOnlyList<ParameterReference>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions)
        {
            if (ReferenceEquals(genericParameters, GenericParameters)
                && ReferenceEquals(parameters, Parameters)
                && ReferenceEquals(conditions, Conditions)) {
                return this;
            }

            return new EventHandlerDefinition(Name, genericParameters, parameters, conditions) {
                Block = Block,
                DeclaringType = DeclaringType,
                Module = Module,
            };
        }
    }
}

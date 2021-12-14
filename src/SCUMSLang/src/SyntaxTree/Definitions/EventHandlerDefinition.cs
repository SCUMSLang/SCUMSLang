using System.Collections.Generic;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class EventHandlerDefinition : EventHandlerReference, ICollectibleMember, IBlockHolder
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.EventHandlerDefinition;

        public BlockDefinition? Body =>
            body;

        private BlockDefinition? body;

        bool IBlockHolder.IsBlockOwnable =>
            true;

        BlockDefinition? IBlockHolder.Block =>
            Body;

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

        void IBlockHolder.SetupBlock(BlockDefinition parentBlock) =>
            BlockHolders.SetupBlock(ref body, parentBlock, BlockScope.Local);

        public new EventHandlerDefinition Resolve() =>
            this;

        protected override IMember ResolveMember() =>
            Resolve();

        protected internal override Reference Accept(NodeVisitor visitor) =>
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
                DeclaringType = DeclaringType,
                ParentBlockContainer = ParentBlockContainer,
                body = body,
            };
        }
    }
}

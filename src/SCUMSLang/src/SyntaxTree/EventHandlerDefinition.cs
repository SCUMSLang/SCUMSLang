using System.Collections.Generic;

namespace SCUMSLang.SyntaxTree
{
    public class EventHandlerDefinition : EventHandlerReference, IMemberDefinition, IBlockHolder
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.EventHandlerDefinition;
        public BlockDefinition? Block { get; internal set; }
        bool IBlockHolder.IsExandable => true;

        BlockDefinition? IBlockHolder.Block {
            get => Block;
            set => Block = value;
        }

        public EventHandlerDefinition(            
            string name,
            IReadOnlyList<ParameterDefinition>? genericParameters,
            IReadOnlyList<ParameterDefinition>? parameters,
            IReadOnlyList<MethodCallDefinition>? conditions,
            TypeReference declaringType)
            : base(name, genericParameters, parameters, conditions, declaringType) { }

        public new EventHandlerDefinition Resolve() {
            ResolveDependencies();
            return this;
        }

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();
    }
}

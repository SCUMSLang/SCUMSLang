﻿using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public sealed class MethodCallDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType => 
            SyntaxTreeNodeType.MethodCallDefinition;

        public MethodReference Method { get; }
        public IReadOnlyList<ConstantDefinition> GenericArguments { get; }
        public IReadOnlyList<ConstantDefinition> Arguments { get; }

        private MethodCallDefinition(
            MethodReference method,
            IReadOnlyList<ConstantDefinition>? genericArguments,
            IReadOnlyList<ConstantDefinition>? arguments)
        {
            Method = method ?? throw new System.ArgumentNullException(nameof(method));
            GenericArguments = genericArguments ?? new List<ConstantDefinition>();
            Arguments = arguments ?? new List<ConstantDefinition>();
        }

        public MethodCallDefinition(
            string name,
            IReadOnlyList<ConstantDefinition>? genericArguments,
            IReadOnlyList<ConstantDefinition>? arguments)
        {
            Method = new MethodReference(
                name,
                genericArguments?.ToParameterDefinitionList(),
                arguments?.ToParameterDefinitionList());

            GenericArguments = genericArguments ?? new List<ConstantDefinition>();
            Arguments = arguments ?? new List<ConstantDefinition>();
        }

        internal protected override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitMethodCallDefinition(this);

        public MethodCallDefinition Update(MethodReference method, IReadOnlyList<ConstantDefinition> genericArguments, IReadOnlyList<ConstantDefinition> arguments)
        {
            if (ReferenceEquals(method, Method) && ReferenceEquals(genericArguments, GenericArguments) && ReferenceEquals(arguments, Arguments)) {
                return this;
            }

            return new MethodCallDefinition(method, genericArguments, arguments);
        }
    }
}

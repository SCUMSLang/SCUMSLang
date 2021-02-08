﻿using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class TypeDefinitionNode : Node, INameReservedNode
    {
        public override NodeType NodeType => NodeType.TypeDefinition;

        public string Name { get; }
        public virtual InBuiltType Type { get; }

        protected TypeDefinitionNode(string name) =>
            Name = name ?? throw new ArgumentNullException(nameof(name));

        public TypeDefinitionNode(string name, InBuiltType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is TypeDefinitionNode node)) {
                return false;
            }

            var equals = NodeType == node.NodeType
                && Name == node.Name
                && Type == node.Type;

            Debug.WriteLineIf(!equals, $"{nameof(TypeDefinitionNode)} not equals.");
            return equals;
        }

        public virtual bool IsSubsetOf(object? obj) =>
            Equals(obj);

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), NodeType, Name, Type);
    }
}
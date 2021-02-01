using System;

namespace SCUMSLang.AST
{
    public class LinkedAssignment : Node
    {
        public override NodeType NodeType => NodeType.LinkedAssignment;

        public DeclarationNode Declaration { get; }
        public AssignNode Assignment { get; }

        public LinkedAssignment(DeclarationNode declaration, AssignNode assignment)
        {
            Declaration = declaration;
            Assignment = assignment;
        }

        public override bool Equals(object? obj) =>
            obj is LinkedAssignment linkedAssignment
            && Declaration.Equals(linkedAssignment.Declaration)
            && Assignment.Equals(linkedAssignment.Assignment);

        public override int GetHashCode() => 
            HashCode.Combine(NodeType, Declaration, Assignment);
    }
}

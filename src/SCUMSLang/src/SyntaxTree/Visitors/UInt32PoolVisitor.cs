using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Visitors
{
    /// <summary>
    /// Walks the tree and searches for reservable intergers declared by the 'add_uint32'-method.
    /// </summary>
    public class UInt32PoolVisitor : SyntaxNodeVisitor
    {
        protected internal override Reference VisitMethodCallDefinition(MethodCallDefinition methodCall) {
            //if (methodCall.Method.Name == "add_uint32") { 
            //    methodCall.Arguments
            //}

            return base.VisitMethodCallDefinition(methodCall);
        }
    }
}

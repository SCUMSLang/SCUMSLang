using System;

namespace SCUMSLang.SyntaxTree.Visitors
{
    public static class VisitorThrowHelper
    {
        public static InvalidOperationException MustRewriteToSameNode(Type nodeType, string callerName) =>
            new InvalidOperationException($"Node must be rewritten to {nodeType.FullName}. (caller: {callerName})");
    }
}

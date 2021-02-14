using System;

namespace SCUMSLang.AST
{
    public static class ReferenceTools
    {
        public static ResolveCollection ResolveBuilder(params MemberReference?[] memberReferences) {
            var resolveBuilder = new ResolveCollection();
            resolveBuilder.IncludeDependencies(memberReferences);
            return resolveBuilder;
        }
    }
}

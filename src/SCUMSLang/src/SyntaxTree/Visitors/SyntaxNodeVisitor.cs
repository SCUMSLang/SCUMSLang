using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SCUMSLang.SyntaxTree.Visitors
{
    public class SyntaxNodeVisitor
    {
        //protected virtual void VisitReference(Reference reference) { }

        //protected virtual void VisitMemberReference(MemberReference member) =>
        //    VisitReference(member);

        //protected virtual void VisitTypeReferenceBase(TypeReference typeBase) =>
        //    VisitMemberReference(typeBase);

        //protected virtual void VisitTypeSpecification(TypeSpecification typeSpecification) =>
        //    VisitTypeReferenceBase(typeSpecification);

        //protected virtual void VisitBlockDefinition(BlockDefinition block) =>
        //    VisitReference(block);

        //protected virtual void VisitFieldReferenceBase(FieldReference field) =>
        //    VisitMemberReference(field);

        //protected virtual void VisitTypeBlockDefinition(TypeBlockDefinition typeBlock) =>
        //    VisitBlockDefinition(typeBlock);

        //protected virtual void VisitParameterReference(ParameterReference parameter) =>
        //    VisitReference(parameter);

        [return: NotNullIfNotNull("reference")]
        public virtual Reference? Visit(Reference? reference) =>
            reference?.Accept(this) ?? null!;

        [return: NotNullIfNotNull("reference")]
        public T? VisitAndConvert<T>(T? reference, [CallerMemberName] string? callerName = null)
            where T : Reference
        {
            if (reference is null) {
                return reference;
            }

            var visitedReference = Visit(reference) as T;

            if (visitedReference is null) {
                throw VisitorThrowHelper.MustRewriteToSameNode(typeof(T), callerName);
            }

            return visitedReference;
        }

        public IReadOnlyList<T> VisitAndConvert<T>(IReadOnlyList<T> nodes, [CallerMemberName] string? callerName = null)
            where T : Reference
        {
            T[]? newNodes = null;
            var nodesCount = nodes.Count;

            for (int i = 0; i < nodesCount; i++) {
                T? node = Visit(nodes[i]) as T;

                if (node == null) {
                    throw VisitorThrowHelper.MustRewriteToSameNode(typeof(T), callerName);
                }

                if (newNodes != null) {
                    newNodes[i] = node;
                } else if (!ReferenceEquals(node, nodes[i])) {
                    newNodes = new T[nodesCount];

                    for (int j = 0; j < i; j++) {
                        newNodes[j] = nodes[j];
                    }

                    newNodes[i] = node;
                }
            }

            if (newNodes == null) {
                return nodes;
            }

            return newNodes;
        }

        protected internal virtual Reference VisitMemberAssignmentDefinition(MemberAssignmenDefinition assign) =>
            assign;

        protected internal virtual Reference VisitMethodReference(MethodReference method) =>
            method.UpdateReference(VisitAndConvert(method.GenericParameters), VisitAndConvert(method.Parameters));

        protected internal virtual Reference VisitMethodDefinition(MethodDefinition method) =>
            method.UpdateDefinition(VisitAndConvert(method.GenericParameters), VisitAndConvert(method.Parameters));

        protected internal virtual Reference VisitMethodCallDefinition(MethodCallDefinition methodCall) =>
            methodCall.Update(
                VisitAndConvert(methodCall.Method),
                VisitAndConvert(methodCall.GenericArguments),
                VisitAndConvert(methodCall.Arguments));

        protected internal virtual Reference VisitAttributeDefinition(AttributeDefinition attribute) =>
            attribute;

        protected internal virtual Reference VisitArrayType(ArrayType arrayType) =>
            arrayType;

        protected internal virtual Reference VisitScopedBlockDefinition(BlockDefinition.ScopedBlockDefinition localBlock) =>
            localBlock;

        protected internal virtual Reference VisitConstantDefinition(ConstantDefinition constant) =>
            constant.Update(VisitAndConvert(constant.ValueType));

        protected internal virtual Reference VisitFieldReference(FieldReference field) =>
            field.UpdateReference(VisitAndConvert(field.FieldType));

        protected internal virtual Reference VisitForInDefinition(ForInDefinition forIn) =>
            forIn.Update(VisitAndConvert(forIn.Parameter), VisitAndConvert(forIn.Arguments));

        protected internal virtual Reference VisitImportDefinition(ImportDefinition import) =>
            import;

        protected internal virtual Reference VisitFieldDefinition(FieldDefinition field) =>
            field.UpdateDefinition(VisitAndConvert(field.FieldType));

        protected internal virtual Reference VisitModuleDefinition(ModuleDefinition module) =>
            module;

        protected internal virtual Reference VisitParameterReference(ParameterReference parameter) =>
            parameter;

        protected internal virtual Reference VisitParameterDefinition(ParameterDefinition parameter) =>
            parameter.Update(VisitAndConvert(parameter.ParameterType));

        protected internal virtual Reference VisitTemplateForInDefinition(TemplateForInDefinition templateForIn) =>
            templateForIn;

        protected internal virtual Reference VisitTypeReference(TypeReference type) =>
            type; // Fine

        protected internal virtual Reference VisitEnumerationDefinition(EnumerationDefinition type) =>
            type.Update(VisitAndConvert(type.BaseType), VisitAndConvert(type.Fields));

        protected internal virtual Reference VisitTypeDefinition(TypeDefinition type) =>
            type.Update(VisitAndConvert(type.BaseType));

        protected internal virtual Reference VisitUsingStaticDirective(UsingStaticDirective usingStaticDirective) =>
            usingStaticDirective.Update(VisitAndConvert(usingStaticDirective.ElementType));

        protected internal virtual Reference VisitEventHandlerReference(EventHandlerReference eventHandler) => eventHandler.UpdateReference(
            VisitAndConvert(eventHandler.GenericParameters),
            VisitAndConvert(eventHandler.Parameters),
            VisitAndConvert(eventHandler.Conditions));

        protected internal virtual Reference VisitEventHandlerDefinition(EventHandlerDefinition eventHandler) => eventHandler.UpdateDefinition(
            VisitAndConvert(eventHandler.GenericParameters),
            VisitAndConvert(eventHandler.Parameters),
            VisitAndConvert(eventHandler.Conditions));

        internal virtual Reference VisitModuleBlockDefinition(ModuleDefinition.ModuleBlockDefinition moduleBlock) =>
            moduleBlock;
    }
}

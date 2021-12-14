using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Visitors
{
    public class NodeVisitor
    {
        public ILogger? Logger { get; set; }

        [return: NotNullIfNotNull("reference")]
        public virtual Reference? Visit(Reference? reference)
        {
#if DEBUG
            if (reference is not null) {
                var stringBuilder = new StringBuilder();

                if (reference is MemberReference member) {
                    stringBuilder.Append($"Name = {member.Name}, ");
                }

                stringBuilder.Append($"NodeType = {Enum.GetName(reference.NodeType)}, ClassType = {reference.GetType().Name}{Environment.NewLine}");

                if (reference is ModuleDefinition module) {
                    stringBuilder.Append($"--> FilePath = {module.FilePath}");
                }

                Logger?.LogInformation(stringBuilder.ToString());
            }
#endif

            return reference?.Accept(this) ?? null!;
        }

        [return: NotNullIfNotNull("reference")]
        protected T? VisitAndConvert<T>(T? reference, [CallerMemberName] string? callerName = null)
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

        protected IReadOnlyList<T> VisitAndConvert<T>(IReadOnlyList<T> nodes, [CallerMemberName] string? callerName = null)
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
            methodCall.UpdateDefinition(
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
            constant.UpdateDefinition(VisitAndConvert(constant.ValueType));

        protected internal virtual Reference VisitFieldReference(FieldReference field) =>
            field.UpdateReference(VisitAndConvert(field.FieldType));

        protected internal virtual Reference VisitForInDefinition(ForInDefinition forIn) =>
            forIn.Update(VisitAndConvert(forIn.Parameter), VisitAndConvert(forIn.Arguments));

        protected internal virtual Reference VisitImportDefinition(ImportDefinition import) =>
            import;

        protected internal virtual Reference VisitFieldDefinition(FieldDefinition field) =>
            field.UpdateDefinition(VisitAndConvert(field.FieldType));

        protected internal virtual Reference VisitParameterReference(ParameterReference parameter) =>
            parameter;

        protected internal virtual Reference VisitParameterDefinition(ParameterDefinition parameter) =>
            parameter.Update(VisitAndConvert(parameter.ParameterType));

        protected internal virtual Reference VisitTemplateForInDefinition(TemplateForInDefinition templateForIn) =>
            templateForIn.UpdateDefinition(VisitAndConvert(templateForIn.ForInCollection));

        protected internal virtual Reference VisitTypeReference(TypeReference type) =>
            type; // Fine

        protected internal virtual Reference VisitEnumerationDefinition(EnumerationDefinition type) =>
            type.UpdateDefinition(VisitAndConvert(type.BaseType), VisitAndConvert(type.Fields));

        protected internal virtual Reference VisitTypeDefinition(TypeDefinition type) =>
            type.UpdateDefinition(VisitAndConvert(type.BaseType));

        protected internal virtual Reference VisitUsingStaticDirectiveDefinition(UsingStaticDirectiveDefinition usingStaticDirective) =>
            usingStaticDirective.UpdateDefinition(VisitAndConvert(usingStaticDirective.ElementType));

        protected internal virtual Reference VisitEventHandlerReference(EventHandlerReference eventHandler) => eventHandler.UpdateReference(
            VisitAndConvert(eventHandler.GenericParameters),
            VisitAndConvert(eventHandler.Parameters),
            VisitAndConvert(eventHandler.Conditions));

        protected internal virtual Reference VisitEventHandlerDefinition(EventHandlerDefinition eventHandler) => eventHandler.UpdateDefinition(
            VisitAndConvert(eventHandler.GenericParameters),
            VisitAndConvert(eventHandler.Parameters),
            VisitAndConvert(eventHandler.Conditions));

        protected internal virtual Reference VisitModuleBlockDefinition(ModuleBlockDefinition moduleBlock) =>
            moduleBlock.UpdateDefinition(VisitAndConvert(moduleBlock.BookkeptReferences));

        protected internal virtual Reference VisitModuleDefinition(ModuleDefinition module) =>
            module.UpdateDefinition(VisitAndConvert(module.ModuleBlock));
    }
}

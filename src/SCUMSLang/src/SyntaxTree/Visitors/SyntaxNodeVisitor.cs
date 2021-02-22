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

        public virtual Reference Visit(Reference reference) =>
            reference;

        public T VisitAndConvert<T>(T reference, string callerName)
            where T : Reference
        {
            var visitedReference = Visit(reference) as T;

            if (visitedReference is null) {
                throw VisitorThrowHelper.MustRewriteToSameNode(typeof(T), callerName);
            }

            return visitedReference;
        }

        protected internal virtual Reference VisitAssignDefinition(AssignDefinition assign) =>
            assign;

        protected internal virtual Reference VisitMethodReference(MethodReference method) =>
            method;

        protected internal virtual Reference VisitMethodCallDefinition(MethodCallDefinition methodCall) =>
            methodCall;

        protected internal virtual Reference VisitAttributeDefinition(AttributeDefinition attribute) =>
            attribute;

        protected internal virtual Reference VisitArrayType(ArrayType arrayType) =>
            arrayType;

        protected internal virtual Reference VisitLocalBlockDefinition(BlockDefinition.LocalBlockDefinition localBlock) =>
            localBlock;

        protected internal virtual Reference VisitConstantDefinition(ConstantDefinition constant) =>
            constant.Rewrite(VisitAndConvert(constant.ValueType, nameof(VisitConstantDefinition)));

        protected internal virtual Reference VisitFieldReference(FieldReference field) =>
            field;

        protected internal virtual Reference VisitForInDefinition(ForInDefinition forIn) =>
            forIn;

        protected internal virtual Reference VisitImportDefinition(ImportDefinition import) =>
            import;

        protected internal virtual Reference VisitFieldDefinition(FieldDefinition field) =>
            field;

        protected internal virtual Reference VisitModuleDefinition(ModuleDefinition module) =>
            module;

        protected internal virtual Reference VisitParameterDefinition(ParameterDefinition parameter) =>
            parameter;

        protected internal virtual Reference VisitTemplateForInDefinition(TemplateForInDefinition templateForIn) =>
            templateForIn;

        protected internal virtual Reference VisitTypeReference(TypeReference type) =>
            type;

        protected internal virtual Reference VisitTypeDefinition(TypeDefinition type) =>
            type;

        protected internal virtual Reference VisitUsingStaticDirective(UsingStaticDirective usingStaticDirective) =>
            usingStaticDirective;

        internal virtual Reference VisitModuleBlockDefinition(ModuleDefinition.ModuleBlockDefinition moduleBlock) =>
            moduleBlock;
    }
}

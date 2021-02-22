//namespace SCUMSLang.SyntaxTree.Visitors
//{
//    public class SyntaxNodeModuleFillingVisitor : SyntaxNodeVisitor
//    {
//        private readonly ModuleDefinition module;

//        public SyntaxNodeModuleFillingVisitor(ModuleDefinition module) => 
//            this.module = module ?? throw new System.ArgumentNullException(nameof(module));

//        protected override void Visit(MemberReference member)
//        {
//            member.Module ??= module;
//            base.VisitMemberReference(member);
//        }
//    }
//}

namespace SCUMSLang.AST
{
    public class ModuleParameters
    {
        public IMemberResolver? MemberResolver { get; set; }
        public NameReservableNodePool? NameReservableDefinitions { get; set; }
        public string? FilePath { get; set; }
    }
}

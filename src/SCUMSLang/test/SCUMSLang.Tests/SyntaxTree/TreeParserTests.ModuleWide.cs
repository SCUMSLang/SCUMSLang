using SCUMSLang.SyntaxTree;
using Xunit;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class ModuleWide : TreeParserTests
        {
            [Fact]
            public void Should_resolve_nested_type_without_specifying_type_name()
            {
                var content = @"
enum Boolean {
    false,
    true
}

using static Boolean;";

                var parser = new SyntaxTreeParser();
                var result = parser.Parse(content);
                var module = result.Module;

                _ = module.Resolve(new TypeReference("false"));
            }

            [Fact]
            public void Should_throw_because_using_static_directives_point_to_same_type()
            {
                var content = @"
enum Boolean {
    false,
    true
}

typedef Boolean bool;

using static Boolean;
using static bool;";

                Assert.Throws<BlockEvaluatingException>(() => _ = new SyntaxTreeParser().Parse(content));
            }
        }
    }
}

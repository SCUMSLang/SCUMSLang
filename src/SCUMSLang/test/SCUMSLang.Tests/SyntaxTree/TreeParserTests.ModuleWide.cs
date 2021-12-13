using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.SyntaxTree.References;
using Xunit;
using Xunit.Abstractions;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class ModuleWide : TreeParserTests
        {
            public ModuleWide(ITestOutputHelper outputHelper)
                : base(outputHelper) { }

            [Fact]
            public void Should_resolve_nested_type_without_specifying_type_name()
            {
                var content = @"
enum Boolean {
    false,
    true
}

using static Boolean;";

                var parser = new SyntaxTreeParser(options => {
                    options.AutoResolve = true;
                    options.Module.AddSystemTypes();
                });

                var result = parser.Parse(content);
                var module = result.Module;

                module.Resolve(new TypeReference("false")).ThrowIfError();
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

                Assert.Throws<BlockEvaluationException>(() => SyntaxTreeParser.AutoResolvable.Parse(content));
            }
        }
    }
}

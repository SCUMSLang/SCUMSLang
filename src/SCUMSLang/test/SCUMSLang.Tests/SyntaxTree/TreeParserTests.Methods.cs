using SCUMSLang.SyntaxTree.Parser;
using Xunit;
using Xunit.Abstractions;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class Methods : TreeParserTests
        {
            public Methods(ITestOutputHelper outputHelper)
                : base(outputHelper) { }

            [Fact]
            public void Should_parse_method_without_body()
            {
                // Assign
                var content = "function daisy();";

                // Act
                var module = DefaultParser.Parse(content).Module;
                var method = module.Block.GetMethod("daisy").Value;

                // Assert
                Assert.True(method.IsAbstract);
                Assert.Null(method.Body);
            }
        }
    }
}

using Xunit;
using Xunit.Abstractions;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class BlockScope : TreeParserTests
        {
            public BlockScope(ITestOutputHelper outputHelper)
                : base(outputHelper) { }

            [Fact]
            public void Should_throw ()
            {
                
            }
        }
    }
}

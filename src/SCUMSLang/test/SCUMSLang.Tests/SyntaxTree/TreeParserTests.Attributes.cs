using SCUMSLang.SyntaxTree.Parser;
using Xunit;
using Xunit.Abstractions;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class Attribtues : TreeParserTests
        {
            public Attribtues(ITestOutputHelper outputHelper)
                : base(outputHelper) { }

            [Fact]
            public void Should_parse_attribute_without_brackets()
            {
                var content = @"
function TriggerCondition();
[TriggerCondition]
function daisy();";

                var module = DefaultParser.Parse(content).Module;
                module.Block.GetMethod("TriggerCondition").Resolve();
            }

            [Fact]
            public void Should_parse_attribute_with_brackets()
            {
                var content = @"
function TriggerCondition();
[TriggerCondition()]
function daisy();";

                var module = DefaultParser.Parse(content).Module;
                module.Block.GetMethod("TriggerCondition").Resolve();
            }

            [Fact]
            public void Should_parse_method_decorated_with_attribute()
            {
                var content = @"
function TriggerCondition();
[TriggerCondition]
function daisy();";

                var module = DefaultParser.Parse(content).Module;
                var method = module.Block.GetMethod("daisy").Value;
                Assert.Single(method.Attributes);
            }

            [Fact]
            public void Should_throw_because_non_attached_attribute()
            {
                var content = @"
function TriggerCondition();
[TriggerCondition()]";

                var error = Assert.Throws<BlockEvaluationException>(() => DefaultParser.Parse(content));
                Assert.Contains("attribute", error.Message);
            }
        }
    }
}

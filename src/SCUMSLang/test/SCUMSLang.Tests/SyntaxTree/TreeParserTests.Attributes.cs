using SCUMSLang.SyntaxTree.Parser;
using Xunit;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class Attribtues : TreeParserTests
        {
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
                var method = module.Block.GetMethod("daisy");
                Assert.Single(method.Attributes);
            }

            [Fact]
            public void Should_throw_because_non_attached_attribute()
            {
                var content = @"
function TriggerCondition();
[TriggerCondition()]";

                var error = Assert.Throws<BlockEvaluatingException>(() => DefaultParser.Parse(content));
                Assert.Contains("attribute", error.Message);
            }
        }
    }
}

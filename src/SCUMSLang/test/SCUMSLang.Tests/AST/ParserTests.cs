using SCUMSLang.AST;
using Xunit;

namespace SCUMSLANG.AST
{
    public class ParserTests
    {
        [Fact]
        public void Should_parse_static_declaration_and_integer_assignment()
        {
            var content = "static int goofy = 4;";
            var block = Parser.Parse(content);

            var declaration = new DeclarationNode(Scope.Static, NodeValueType.Integer, "goofy");
            var expectedNodes = new Node[] { declaration, new LinkedAssignment(declaration, new AssignNode("goofy", new ConstantNode(NodeValueType.Integer, 4))) };

            Assert.Equal(expectedNodes, block.Nodes);
        }

        [Fact]
        public void Should_parse_function()
        {
            var content = "function daisy() {}";
            var block = Parser.Parse(content);

            var expectedBlock = new StaticBlockNode();
            expectedBlock.BeginBlock(new FunctionNode("daisy", null, new DeclarationNode[] { }));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_functions()
        {
            var content = @"function daisy(int hello) {}
                            function daisy() {}";

            var block = Parser.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var declarationFirst = new DeclarationNode(Scope.Local, NodeValueType.Integer, "hello");

            expectedBlock.BeginBlock(new FunctionNode("daisy", null, new[] { declarationFirst }));
            expectedBlock.CurrentBlock.EndBlock();

            expectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_function_with_generic_parameters()
        {
            var content = @"function daisy<unit PlayerId>() {}";

            var block = Parser.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var genericDeclaration = new DeclarationNode(Scope.Local, NodeValueType.Unit, "PlayerId");

            expectedBlock.BeginBlock(new FunctionNode("daisy", new DeclarationNode[] { genericDeclaration }, null));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_event_handler_with_generic_parameters()
        {
            var content = @"function daisy<unit PlayerId>() when cond_one<Player1>(0xf) {}";

            var block = Parser.Parse(content);

            var expectedBlock = new StaticBlockNode();

            expectedBlock.BeginBlock(
                new EventHandlerNode(
                    "daisy",
                    new[] { new DeclarationNode(Scope.Local, NodeValueType.Unit, "PlayerId") },
                    null,
                    new[] { new FunctionCallNode("cond_one", new[] { new ConstantNode(NodeValueType.Player, Player.Player1) }, new[] { new ConstantNode(NodeValueType.Integer, 16) }) }));

            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }
    }
}

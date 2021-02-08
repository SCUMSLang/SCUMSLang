using System.Diagnostics;
using SCUMSLang;
using SCUMSLang.AST;
using SCUMSLang.Tokenization;
using Xunit;

namespace SCUMSLANG.AST
{
    public class ParserTests
    {
        public ParserOptions ParserChannelParserOptions { get; }
        public TypeDefinitionNode IntegerDefinition { get; }
        public TypeDefinitionNode StringDefinition { get; }
        public EnumerationDefinitionNode PlayerDefinition { get; }
        public EnumerationDefinitionNode UnitDefinition { get; }
        public EnumerationDefinitionNode BoolDefinition { get; }

        public ParserTests()
        {
            Trace.Listeners.Add(new DefaultTraceListener());

            ParserChannelParserOptions = new ParserOptions() {
                TokenReaderBehaviour = new SpanReaderBehaviour<Token>() {
                    SkipCondition = (ref ReaderPosition<Token> tokenPosition) => {
                        return tokenPosition.Value.Channel != Channel.Parser;
                    }
                }
            };

            IntegerDefinition = new TypeDefinitionNode("int", InBuiltType.Integer);
            StringDefinition = new TypeDefinitionNode("string", InBuiltType.Integer);
            PlayerDefinition = new EnumerationDefinitionNode("Player", hasReservedNames: false, new[] { "Player2", "Player1" });
            UnitDefinition = new EnumerationDefinitionNode("Unit", hasReservedNames: false, new string[] { });
            BoolDefinition = new EnumerationDefinitionNode("bool", hasReservedNames: true, new[] { "false", "true" });
        }

        [Fact]
        public void Should_parse_static_int_declaration_and_assignment()
        {
            var content = @"
typedef int int;
static int goofy = 4;";

            var block = Parser.Default.Parse(content);

            var staticBlock = new StaticBlockNode();
            staticBlock.AddTypeDefintion(IntegerDefinition);
            var goofyDeclaration = new DeclarationNode(Scope.Static, IntegerDefinition, "goofy");
            staticBlock.AddDeclaration(goofyDeclaration);
            staticBlock.AddAssignment(new AssignNode(goofyDeclaration, new ConstantNode(IntegerDefinition, 4)));

            // Assert
            Assert.Equal(staticBlock, block);
        }

        [Fact]
        public void Should_parse_function()
        {
            var content = "function daisy() {}";
            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            expectedBlock.BeginBlock(new FunctionNode("daisy", null, new DeclarationNode[] { }));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_parameterized_function_and_function()
        {
            var content = @"
typedef int int;
function daisy(int hello) {}
function daisy() {}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddTypeDefintion(IntegerDefinition);
            var helloDeclaration = new DeclarationNode(Scope.Local, IntegerDefinition, "hello");

            expectedBlock.BeginBlock(new FunctionNode("daisy", null, new[] { helloDeclaration }));
            expectedBlock.CurrentBlock.EndBlock();

            expectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_generic_parameterized_function()
        {
            var content = @"
enum Player { Player2, Player1 }
function daisy<Player PlayerId>() {}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var genericDeclaration = new DeclarationNode(Scope.Local, PlayerDefinition, "PlayerId");
            expectedBlock.AddTypeDefintion(PlayerDefinition);

            expectedBlock.BeginBlock(new FunctionNode("daisy", new DeclarationNode[] { genericDeclaration }, null));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_function_with_declared_assignment()
        {
            var content = @"
typedef int int;

function daisy() {
    int local_var = 2;
}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var localVarDeclration = new DeclarationNode(Scope.Local, IntegerDefinition, "local_var");

            expectedBlock.AddTypeDefintion(IntegerDefinition);
            expectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            expectedBlock.CurrentBlock.AddDeclaration(localVarDeclration);
            expectedBlock.CurrentBlock.AddAssignment(new AssignNode(localVarDeclration, new ConstantNode(IntegerDefinition, 2)));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_function_with_static_declaration_and_local_assignment()
        {
            var content = @"
typedef int int;
static int global_var;

function daisy() {
    global_var = 2;
}";

            var block = Parser.Default.Parse(content);
            var globalVarDeclaration = new DeclarationNode(Scope.Static, IntegerDefinition, "global_var");

            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddTypeDefintion(IntegerDefinition);
            expectedBlock.AddDeclaration(globalVarDeclaration);
            expectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            expectedBlock.CurrentBlock.AddAssignment(new AssignNode(globalVarDeclaration, new ConstantNode(IntegerDefinition, 2)));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_generic_parameterized_event_handler()
        {
            var content = @"
typedef int int;
enum Player { Player2, Player1 }
enum Unit {}
function cond_one<Player PlayerId>(int some_val);
function daisy<Unit UnitId>() when cond_one<Player.Player1>(0xf) {}";

            var block = Parser.Default.Parse(content);

            var playerIdDeclaration = new DeclarationNode(Scope.Local, PlayerDefinition, "PlayerId");
            var someValDeclaration = new DeclarationNode(Scope.Local, IntegerDefinition, "some_val");
            var condOneEventHandler = new FunctionNode("cond_one", new[] { playerIdDeclaration }, new[] { someValDeclaration }, true);

            var playerPlayer1EnumerationMember = new EnumerationMemberNode(PlayerDefinition, "Player.Player1", 0);
            var condOneFunctionCall = new FunctionCallNode(condOneEventHandler, new[] { new ConstantNode(playerPlayer1EnumerationMember, null) }, new[] { new ConstantNode(IntegerDefinition, 15) });

            var unitIdDeclaration = new DeclarationNode(Scope.Local, UnitDefinition, "UnitId");
            var daisyEventHandler = new EventHandlerNode("daisy", new[] { unitIdDeclaration }, null, new[] { condOneFunctionCall });

            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddTypeDefintion(IntegerDefinition);
            expectedBlock.AddTypeDefintion(PlayerDefinition);
            expectedBlock.AddTypeDefintion(UnitDefinition);
            expectedBlock.AddFunction(condOneEventHandler);
            expectedBlock.BeginBlock(daisyEventHandler);
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_static_int_declaration_with_comment()
        {
            var content = @"
typedef int int;
// IGNORE ME
static int goofy;";

            var block = Parser.Default.Parse(content, ParserChannelParserOptions);

            var goofyDeclaration = new DeclarationNode(Scope.Static, IntegerDefinition, "goofy");
            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddTypeDefintion(IntegerDefinition);
            expectedBlock.AddDeclaration(goofyDeclaration);

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_short_cut_attribute()
        {
            var content = @"
function TriggerCondition();

[TriggerCondition]";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var triggerConditionFunction = new FunctionNode("TriggerCondition", null, null, isAbstract: true);
            var triggerConditionAttribute = new AttributeNode(new FunctionCallNode(triggerConditionFunction, null, null));
            expectedBlock.AddFunction(triggerConditionFunction);
            expectedBlock.AddNode(triggerConditionAttribute);

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_enumeration()
        {
            var content = @"
enum Unit {
    ProtossProbe,
    ProtossZealot
}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var unitTypeDefinition = new EnumerationDefinitionNode("Unit", false, new[] { "ProtossProbe", "ProtossZealot" });
            expectedBlock.AddTypeDefintion(unitTypeDefinition);

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_enumeration_with_reserved_names()
        {
            var content = @"
typedef enum {
    false,
    true
} bool;

function daisy(bool test_bool);
function goofy() when daisy(false) {}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var daisyFunction = new FunctionNode("daisy", null, new[] { new DeclarationNode(Scope.Local, BoolDefinition, "test_bool") }, isAbstract: true);

            var daisyFunctionCall = new FunctionCallNode(daisyFunction, null, new[] { new ConstantNode(BoolDefinition, "false") });
            var goofyFunction = new EventHandlerNode("goofy", null, null, new[] { daisyFunctionCall });

            expectedBlock.AddTypeDefintion(BoolDefinition);
            expectedBlock.AddFunction(daisyFunction);

            expectedBlock.BeginBlock(goofyFunction);
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }
    }
}

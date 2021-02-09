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
        public TypeDefinitionNode UInt32Type { get; }
        public TypeAliasNode IntTypeAlias { get; }
        public TypeDefinitionNode StringType { get; }
        public TypeAliasNode StringTypeAlias { get; }
        public EnumerationDefinitionNode PlayerType { get; }
        public EnumerationDefinitionNode UnitType { get; }
        public EnumerationDefinitionNode BooleanType { get; }
        public TypeAliasNode BoolTypeAlias { get; }

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

            UInt32Type = new TypeDefinitionNode("UInt32", DefinitionType.Integer);
            IntTypeAlias = new TypeAliasNode("int", UInt32Type);
            StringType = new TypeDefinitionNode("String", DefinitionType.Integer);
            StringTypeAlias = new TypeAliasNode("string", StringType);
            PlayerType = new EnumerationDefinitionNode("Player", hasReservedNames: false, new[] { "Player2", "Player1" });
            UnitType = new EnumerationDefinitionNode("Unit", hasReservedNames: false, new string[] { });
            BooleanType = new EnumerationDefinitionNode("Boolean", hasReservedNames: true, new[] { "false", "true" });
            BoolTypeAlias = new TypeAliasNode("bool", BooleanType);
        }

        [Fact]
        public void Should_parse_static_int_declaration_and_assignment()
        {
            var content = @"
typedef Int32 int;
static int goofy = 4;";

            var block = Parser.Default.Parse(content);

            var staticBlock = new StaticBlockNode();
            staticBlock.AddNode(IntTypeAlias);
            var goofyDeclaration = new DeclarationNode(Scope.Static, UInt32Type, "goofy");
            staticBlock.AddNode(goofyDeclaration);
            staticBlock.AddAssignment(new AssignNode(goofyDeclaration, new ConstantNode(UInt32Type, 4)));

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
typedef Int32 int;
function daisy(int hello) {}
function daisy() {}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddNode(IntTypeAlias);
            var helloDeclaration = new DeclarationNode(Scope.Local, UInt32Type, "hello");

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
            var genericDeclaration = new DeclarationNode(Scope.Local, PlayerType, "PlayerId");
            expectedBlock.AddNode(PlayerType);

            expectedBlock.BeginBlock(new FunctionNode("daisy", new DeclarationNode[] { genericDeclaration }, null));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_function_with_declared_assignment()
        {
            var content = @"
typedef Int32 int;

function daisy() {
    int local_var = 2;
}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var localVarDeclration = new DeclarationNode(Scope.Local, UInt32Type, "local_var");

            expectedBlock.AddNode(IntTypeAlias);
            expectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            expectedBlock.CurrentBlock.AddNode(localVarDeclration);
            expectedBlock.CurrentBlock.AddAssignment(new AssignNode(localVarDeclration, new ConstantNode(UInt32Type, 2)));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_function_with_static_declaration_and_local_assignment()
        {
            var content = @"
typedef Int32 int;
static int global_var;

function daisy() {
    global_var = 2;
}";

            var block = Parser.Default.Parse(content);
            var globalVarDeclaration = new DeclarationNode(Scope.Static, UInt32Type, "global_var");

            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddNode(IntTypeAlias);
            expectedBlock.AddNode(globalVarDeclaration);
            expectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            expectedBlock.CurrentBlock.AddAssignment(new AssignNode(globalVarDeclaration, new ConstantNode(UInt32Type, 2)));
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_generic_parameterized_event_handler()
        {
            var content = @"
typedef Int32 int;
enum Player { Player2, Player1 }
enum Unit {}
function cond_one<Player PlayerId>(int some_val);
function daisy<Unit UnitId>() when cond_one<Player.Player1>(0xf) {}";

            var block = Parser.Default.Parse(content);

            var playerIdDeclaration = new DeclarationNode(Scope.Local, PlayerType, "PlayerId");
            var someValDeclaration = new DeclarationNode(Scope.Local, UInt32Type, "some_val");
            var condOneEventHandler = new FunctionNode("cond_one", new[] { playerIdDeclaration }, new[] { someValDeclaration }, true);

            var playerPlayer1EnumerationMember = new EnumerationMemberNode(PlayerType, "Player.Player1", 0);
            var condOneFunctionCall = new FunctionCallNode(condOneEventHandler, new[] { new ConstantNode(playerPlayer1EnumerationMember, null) }, new[] { new ConstantNode(UInt32Type, 15) });

            var unitIdDeclaration = new DeclarationNode(Scope.Local, UnitType, "UnitId");
            var daisyEventHandler = new EventHandlerNode("daisy", new[] { unitIdDeclaration }, null, new[] { condOneFunctionCall });

            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddNode(IntTypeAlias);
            expectedBlock.AddNode(PlayerType);
            expectedBlock.AddNode(UnitType);
            expectedBlock.AddNode(condOneEventHandler);
            expectedBlock.BeginBlock(daisyEventHandler);
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_static_int_declaration_with_comment()
        {
            var content = @"
typedef Int32 int;
// IGNORE ME
static int goofy;";

            var block = Parser.Default.Parse(content, ParserChannelParserOptions);

            var goofyDeclaration = new DeclarationNode(Scope.Static, UInt32Type, "goofy");
            var expectedBlock = new StaticBlockNode();
            expectedBlock.AddNode(IntTypeAlias);
            expectedBlock.AddNode(goofyDeclaration);

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
            expectedBlock.AddNode(triggerConditionFunction);
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
            expectedBlock.AddNode(unitTypeDefinition);

            Assert.Equal(expectedBlock, block);
        }

        [Fact]
        public void Should_parse_enumeration_with_reserved_names()
        {
            var content = @"
typedef enum {
    false,
    true
} Boolean;

typedef Boolean bool;

function daisy(bool test_bool);
function goofy() when daisy(false) {}";

            var block = Parser.Default.Parse(content);

            var expectedBlock = new StaticBlockNode();
            var daisyFunction = new FunctionNode("daisy", null, new[] { new DeclarationNode(Scope.Local, BooleanType, "test_bool") }, isAbstract: true);

            var daisyFunctionCall = new FunctionCallNode(daisyFunction, null, new[] { new ConstantNode(BooleanType, "false") });
            var goofyFunction = new EventHandlerNode("goofy", null, null, new[] { daisyFunctionCall });

            expectedBlock.AddNode(BoolTypeAlias);
            expectedBlock.AddNode(daisyFunction);

            expectedBlock.BeginBlock(goofyFunction);
            expectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(expectedBlock, block);
        }
    }
}

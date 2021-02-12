using System;
using System.Diagnostics;
using SCUMSLang;
using SCUMSLang.AST;
using SCUMSLang.Tokenization;
using Xunit;

namespace SCUMSLANG.AST
{
    public class ParserTests
    {
        public Action<ParserOptions> ParserChannelParserOptionsCallback { get; }
        public TypeDefinitionNode UInt32Type { get; }
        public TypeAliasNode IntTypeAlias { get; }
        public TypeDefinitionNode StringType { get; }
        public TypeAliasNode StringTypeAlias { get; }
        public EnumerationDefinitionNode PlayerType { get; }
        public EnumerationDefinitionNode UnitType { get; }
        public EnumerationDefinitionNode BooleanType { get; }
        public TypeAliasNode BoolTypeAlias { get; }
        public Parser DefaultParser { get; }
        public StaticBlockNode ExpectedBlock { get; }

        public ParserTests()
        {
            Trace.Listeners.Add(new DefaultTraceListener());

            ParserChannelParserOptionsCallback = options => {
                options.TokenReaderBehaviour
                    .SetNonParserChannelTokenSkipCondition();
            };

            UInt32Type = new TypeDefinitionNode("UInt32", DefinitionType.Integer);
            IntTypeAlias = new TypeAliasNode("int", UInt32Type);
            StringType = new TypeDefinitionNode("String", DefinitionType.Integer);
            StringTypeAlias = new TypeAliasNode("string", StringType);
            PlayerType = new EnumerationDefinitionNode("Player", hasReservedNames: false, new[] { "Player2", "Player1" });
            UnitType = new EnumerationDefinitionNode("Unit", hasReservedNames: false, new string[] { });
            BooleanType = new EnumerationDefinitionNode("Boolean", hasReservedNames: true, new[] { "false", "true" });
            BoolTypeAlias = new TypeAliasNode("bool", BooleanType);

            DefaultParser = new Parser(options => {
                options.StaticBlock.AddSystemTypes();
            });

            ExpectedBlock = new StaticBlockNode();
            ExpectedBlock.AddSystemTypes();
        }

        [Fact]
        public void Should_parse_static_int_declaration_and_assignment()
        {
            var content = @"
typedef UInt32 int;
static int goofy = 4;";

            var block = DefaultParser.Parse(content).StaticBlock;

            ExpectedBlock.AddNode(IntTypeAlias);
            var goofyDeclaration = new DeclarationNode(Scope.Static, UInt32Type, "goofy");
            ExpectedBlock.AddNode(goofyDeclaration);
            ExpectedBlock.AddAssignment(new AssignNode(goofyDeclaration, new ConstantNode(UInt32Type, 4)));

            // Assert
            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_function()
        {
            var content = "function daisy() {}";
            var block = DefaultParser.Parse(content).StaticBlock;

            ExpectedBlock.BeginBlock(new FunctionNode("daisy", null, new DeclarationNode[] { }));
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_parameterized_function_and_function()
        {
            var content = @"
typedef UInt32 int;
function daisy(int hello) {}
function daisy() {}";

            var block = DefaultParser.Parse(content).StaticBlock;

            ExpectedBlock.AddNode(IntTypeAlias);
            var helloDeclaration = new DeclarationNode(Scope.Local, UInt32Type, "hello");

            ExpectedBlock.BeginBlock(new FunctionNode("daisy", null, new[] { helloDeclaration }));
            ExpectedBlock.CurrentBlock.EndBlock();

            ExpectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_generic_parameterized_function()
        {
            var content = @"
enum Player { Player2, Player1 }
function daisy<Player PlayerId>() {}";

            var block = DefaultParser.Parse(content).StaticBlock;

            var genericDeclaration = new DeclarationNode(Scope.Local, PlayerType, "PlayerId");
            ExpectedBlock.AddNode(PlayerType);

            ExpectedBlock.BeginBlock(new FunctionNode("daisy", new DeclarationNode[] { genericDeclaration }, null));
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_function_with_declared_assignment()
        {
            var content = @"
typedef UInt32 int;

function daisy() {
    int local_var = 2;
}";

            var block = DefaultParser.Parse(content).StaticBlock;

            var localVarDeclration = new DeclarationNode(Scope.Local, UInt32Type, "local_var");

            ExpectedBlock.AddNode(IntTypeAlias);
            ExpectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            ExpectedBlock.CurrentBlock.AddNode(localVarDeclration);
            ExpectedBlock.CurrentBlock.AddAssignment(new AssignNode(localVarDeclration, new ConstantNode(UInt32Type, 2)));
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_function_with_static_declaration_and_local_assignment()
        {
            var content = @"
typedef UInt32 int;
static int global_var;

function daisy() {
    global_var = 2;
}";

            var block = DefaultParser.Parse(content).StaticBlock;
            var globalVarDeclaration = new DeclarationNode(Scope.Static, UInt32Type, "global_var");

            ExpectedBlock.AddNode(IntTypeAlias);
            ExpectedBlock.AddNode(globalVarDeclaration);
            ExpectedBlock.BeginBlock(new FunctionNode("daisy", null, null));
            ExpectedBlock.CurrentBlock.AddAssignment(new AssignNode(globalVarDeclaration, new ConstantNode(UInt32Type, 2)));
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_generic_parameterized_event_handler()
        {
            var content = @"
typedef UInt32 int;
enum Player { Player2, Player1 }
enum Unit {}
function cond_one<Player PlayerId>(int some_val);
function daisy<Unit UnitId>() when cond_one<Player.Player1>(0xf) {}";

            var block = DefaultParser.Parse(content).StaticBlock;

            var playerIdDeclaration = new DeclarationNode(Scope.Local, PlayerType, "PlayerId");
            var someValDeclaration = new DeclarationNode(Scope.Local, UInt32Type, "some_val");
            var condOneEventHandler = new FunctionNode("cond_one", new[] { playerIdDeclaration }, new[] { someValDeclaration }, true);

            var playerPlayer1EnumerationMember = new EnumerationMemberNode(PlayerType, "Player.Player1", 0);
            var condOneFunctionCall = new FunctionCallNode(condOneEventHandler, new[] { new ConstantNode(playerPlayer1EnumerationMember, null) }, new[] { new ConstantNode(UInt32Type, 15) });

            var unitIdDeclaration = new DeclarationNode(Scope.Local, UnitType, "UnitId");
            var daisyEventHandler = new EventHandlerNode("daisy", new[] { unitIdDeclaration }, null, new[] { condOneFunctionCall });

            ExpectedBlock.AddNode(IntTypeAlias);
            ExpectedBlock.AddNode(PlayerType);
            ExpectedBlock.AddNode(UnitType);
            ExpectedBlock.AddNode(condOneEventHandler);
            ExpectedBlock.BeginBlock(daisyEventHandler);
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_static_int_declaration_with_comment()
        {
            var content = @"
typedef UInt32 int;
// IGNORE ME
static int goofy;";

            var parser = new Parser(ParserChannelParserOptionsCallback);
            var block = parser.Parse(content).StaticBlock;

            var goofyDeclaration = new DeclarationNode(Scope.Static, UInt32Type, "goofy");
            ExpectedBlock.AddNode(IntTypeAlias);
            ExpectedBlock.AddNode(goofyDeclaration);

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_short_cut_attribute()
        {
            var content = @"
function TriggerCondition();

[TriggerCondition]";

            var block = DefaultParser.Parse(content).StaticBlock;

            var triggerConditionFunction = new FunctionNode("TriggerCondition", null, null, isAbstract: true);
            var triggerConditionAttribute = new AttributeNode(new FunctionCallNode(triggerConditionFunction, null, null));
            ExpectedBlock.AddNode(triggerConditionFunction);
            ExpectedBlock.AddNode(triggerConditionAttribute);

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_enumeration()
        {
            var content = @"
enum Unit {
    ProtossProbe,
    ProtossZealot
}";

            var block = DefaultParser.Parse(content).StaticBlock;

            var unitTypeDefinition = new EnumerationDefinitionNode("Unit", false, new[] { "ProtossProbe", "ProtossZealot" });
            ExpectedBlock.AddNode(unitTypeDefinition);

            Assert.Equal(ExpectedBlock, block);
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

            var block = DefaultParser.Parse(content).StaticBlock;

            var daisyFunction = new FunctionNode("daisy", null, new[] { new DeclarationNode(Scope.Local, BooleanType, "test_bool") }, isAbstract: true);

            var daisyFunctionCall = new FunctionCallNode(daisyFunction, null, new[] { new ConstantNode(BooleanType, "false") });
            var goofyFunction = new EventHandlerNode("goofy", null, null, new[] { daisyFunctionCall });

            ExpectedBlock.AddNode(BoolTypeAlias);
            ExpectedBlock.AddNode(daisyFunction);

            ExpectedBlock.BeginBlock(goofyFunction);
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }
    }
}

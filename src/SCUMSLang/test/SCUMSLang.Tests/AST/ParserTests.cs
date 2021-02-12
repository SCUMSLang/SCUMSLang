using System;
using System.Diagnostics;
using SCUMSLang.AST;
using SCUMSLang.Tokenization;
using Xunit;

namespace SCUMSLANG.AST
{
    public class ParserTests
    {
        public Action<ReferenceParserOptions> ParserChannelParserOptionsCallback { get; }
        public TypeDefinition UInt32Type { get; }
        public TypeAliasReference IntTypeAlias { get; }
        public TypeDefinition StringType { get; }
        public TypeAliasReference StringTypeAlias { get; }
        public EnumerationTypeReference PlayerType { get; }
        public EnumerationTypeReference UnitType { get; }
        public EnumerationTypeReference BooleanType { get; }
        public TypeAliasReference BoolTypeAlias { get; }
        public ReferenceParser DefaultParser { get; }
        public ModuleDefinition ExpectedBlock { get; }

        public ParserTests()
        {
            Trace.Listeners.Add(new DefaultTraceListener());

            ParserChannelParserOptionsCallback = options => {
                options.TokenReaderBehaviour
                    .SetNonParserChannelTokenSkipCondition();
            };

            UInt32Type = new TypeDefinition("UInt32", SystemType.Integer);
            IntTypeAlias = new TypeAliasReference("int", UInt32Type);
            StringType = new TypeDefinition("String", SystemType.Integer);
            StringTypeAlias = new TypeAliasReference("string", StringType);
            PlayerType = new EnumerationTypeReference("Player", hasReservedNames: false, new[] { "Player2", "Player1" });
            UnitType = new EnumerationTypeReference("Unit", hasReservedNames: false, new string[] { });
            BooleanType = new EnumerationTypeReference("Boolean", hasReservedNames: true, new[] { "false", "true" });
            BoolTypeAlias = new TypeAliasReference("bool", BooleanType);

            DefaultParser = new ReferenceParser(options => {
                options.Module.AddSystemTypes();
            });

            ExpectedBlock = ModuleDefinition.Create()
                .AddSystemTypes();
        }

        [Fact]
        public void Should_parse_static_int_declaration_and_assignment()
        {
            var content = @"
typedef UInt32 int;
static int goofy = 4;";

            var block = DefaultParser.Parse(content).Module;

            ExpectedBlock.AddNode(IntTypeAlias);
            var goofyDeclaration = new DeclarationReference(Scope.Static, UInt32Type, "goofy");
            ExpectedBlock.AddNode(goofyDeclaration);
            ExpectedBlock.AddAssignment(new AssignDefinition(goofyDeclaration, new ConstantReference(UInt32Type, 4)));

            // Assert
            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_function()
        {
            var content = "function daisy() {}";
            var block = DefaultParser.Parse(content).Module;

            ExpectedBlock.BeginBlock(new FunctionReference("daisy", null, new DeclarationReference[] { }));
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

            var block = DefaultParser.Parse(content).Module;

            ExpectedBlock.AddNode(IntTypeAlias);
            var helloDeclaration = new DeclarationReference(Scope.Local, UInt32Type, "hello");

            ExpectedBlock.BeginBlock(new FunctionReference("daisy", null, new[] { helloDeclaration }));
            ExpectedBlock.CurrentBlock.EndBlock();

            ExpectedBlock.BeginBlock(new FunctionReference("daisy", null, null));
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }

        [Fact]
        public void Should_parse_generic_parameterized_function()
        {
            var content = @"
enum Player { Player2, Player1 }
function daisy<Player PlayerId>() {}";

            var block = DefaultParser.Parse(content).Module;

            var genericDeclaration = new DeclarationReference(Scope.Local, PlayerType, "PlayerId");
            ExpectedBlock.AddNode(PlayerType);

            ExpectedBlock.BeginBlock(new FunctionReference("daisy", new DeclarationReference[] { genericDeclaration }, null));
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

            var block = DefaultParser.Parse(content).Module;

            var localVarDeclration = new DeclarationReference(Scope.Local, UInt32Type, "local_var");

            ExpectedBlock.AddNode(IntTypeAlias);
            ExpectedBlock.BeginBlock(new FunctionReference("daisy", null, null));
            ExpectedBlock.CurrentBlock.AddNode(localVarDeclration);
            ExpectedBlock.CurrentBlock.AddAssignment(new AssignDefinition(localVarDeclration, new ConstantReference(UInt32Type, 2)));
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

            var block = DefaultParser.Parse(content).Module;
            var globalVarDeclaration = new DeclarationReference(Scope.Static, UInt32Type, "global_var");

            ExpectedBlock.AddNode(IntTypeAlias);
            ExpectedBlock.AddNode(globalVarDeclaration);
            ExpectedBlock.BeginBlock(new FunctionReference("daisy", null, null));
            ExpectedBlock.CurrentBlock.AddAssignment(new AssignDefinition(globalVarDeclaration, new ConstantReference(UInt32Type, 2)));
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

            var block = DefaultParser.Parse(content).Module;

            var playerIdDeclaration = new DeclarationReference(Scope.Local, PlayerType, "PlayerId");
            var someValDeclaration = new DeclarationReference(Scope.Local, UInt32Type, "some_val");
            var condOneEventHandler = new FunctionReference("cond_one", new[] { playerIdDeclaration }, new[] { someValDeclaration }, true);

            var playerPlayer1EnumerationMember = new EnumerationMemberReference(PlayerType, "Player.Player1", 0);
            var condOneFunctionCall = new FunctionCallReference(condOneEventHandler, new[] { new ConstantReference(playerPlayer1EnumerationMember, null) }, new[] { new ConstantReference(UInt32Type, 15) });

            var unitIdDeclaration = new DeclarationReference(Scope.Local, UnitType, "UnitId");
            var daisyEventHandler = new EventHandlerReference("daisy", new[] { unitIdDeclaration }, null, new[] { condOneFunctionCall });

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

            var parser = new ReferenceParser(ParserChannelParserOptionsCallback);
            var block = parser.Parse(content).Module;

            var goofyDeclaration = new DeclarationReference(Scope.Static, UInt32Type, "goofy");
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

            var block = DefaultParser.Parse(content).Module;

            var triggerConditionFunction = new FunctionReference("TriggerCondition", null, null, isAbstract: true);
            var triggerConditionAttribute = new AttributeDefinition(new FunctionCallReference(triggerConditionFunction, null, null));
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

            var block = DefaultParser.Parse(content).Module;

            var unitTypeDefinition = new EnumerationTypeReference("Unit", false, new[] { "ProtossProbe", "ProtossZealot" });
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

            var block = DefaultParser.Parse(content).Module;

            var daisyFunction = new FunctionReference("daisy", null, new[] { new DeclarationReference(Scope.Local, BooleanType, "test_bool") }, isAbstract: true);

            var daisyFunctionCall = new FunctionCallReference(daisyFunction, null, new[] { new ConstantReference(BooleanType, "false") });
            var goofyFunction = new EventHandlerReference("goofy", null, null, new[] { daisyFunctionCall });

            ExpectedBlock.AddNode(BoolTypeAlias);
            ExpectedBlock.AddNode(daisyFunction);

            ExpectedBlock.BeginBlock(goofyFunction);
            ExpectedBlock.CurrentBlock.EndBlock();

            Assert.Equal(ExpectedBlock, block);
        }
    }
}

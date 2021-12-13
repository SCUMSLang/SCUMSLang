using System;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.SyntaxTree.References;
using Xunit;
using Xunit.Abstractions;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class ModuleScoped : TreeParserTests
        {
            public ModuleScoped(ITestOutputHelper outputHelper)
                : base(outputHelper) { }

            [Fact]
            public void Should_parse_static_int_declaration_and_assignment()
            {
                var content = @"
static int goofy = 4;";

                var module = DefaultParser.Parse(content).Module;

                module.Block.GetType("UInt32").Resolve();
                module.Block.GetType("int").Resolve();
                module.Block.GetField("goofy").Resolve();
            }

            [Fact]
            public void Should_parse_function()
            {
                var content = "function daisy() {}";
                var module = DefaultParser.Parse(content).Module;
                module.Block.GetMethod("daisy").Resolve();
            }

            [Fact]
            public void Should_parse_parameterized_function_and_function()
            {
                var content = @"
function daisy(int hello) {}
function daisy() {}";

                var module = DefaultParser.Parse(content).Module;

                var daisyHelloFunction = module.Block.GetMethod(new MethodReference("daisy", genericParameters: null, parameters: new[] { new ParameterReference(IntType) })).Value;
                Assert.Equal(1, daisyHelloFunction.Parameters.Count);

                var daisyFunction = module.Block.GetMethod(new MethodReference("daisy", genericParameters: null, parameters: null)).Value;
                Assert.Equal(0, daisyFunction.Parameters.Count);
            }

            [Fact]
            public void Should_parse_generic_parameterized_function()
            {
                var content = @"
enum Player { Player2, Player1 }
function daisy<Player PlayerId>() {}";

                var module = DefaultParser.Parse(content).Module;

                var playerEnum = module.Block.GetType("Player").Value;
                Assert.Equal(2, playerEnum.Fields.Count);
                Assert.Equal("Player2", playerEnum.Fields[0].Name);
                Assert.Equal("Player1", playerEnum.Fields[1].Name);

                var uint32Type = module.Block.GetType("UInt32").Value;
                var player2Field = module.Resolve(new FieldReference("Player2", uint32Type, playerEnum)).Value;
                Assert.Equal(0, player2Field.Value);

                var daisyFunction = module.Block.GetMethod("daisy").Value;
                Assert.Equal(1, daisyFunction.GenericParameters.Count);
            }

            [Fact]
            public void Should_throw_because_non_static_field_declaration_in_function()
            {
                var content = @"function beka() {
int local_var = 2;
}";

                Assert.Throws<NotSupportedException>(() => DefaultParser.Parse(content));
            }

            [Fact]
            public void Should_parse_function_with_static_declaration_and_local_assignment()
            {
                var content = @"
static int global_var;

function daisy() {
    global_var = 2;
}";

                var module = DefaultParser.Parse(content).Module;
                var daisyMethod = module.Block.GetMethod("daisy").Value;
                Assert.Equal(1, daisyMethod.Body.BookkeptReferences.Count);
                Assert.IsType<MemberAssignmenDefinition>(daisyMethod.Body.BookkeptReferences[0]);
            }

            [Fact]
            public void Should_parse_generic_parameterized_event_handler()
            {
                var content = @"
enum Player { Player2, Player1 }
enum Unit {}
function cond_one<Player PlayerId>(int some_val);
function daisy<Unit UnitId>() when cond_one<Player.Player1>(0xf) {}";

                var module = DefaultParser.Parse(content).Module;
                var playerType = module.Block.GetType("Player").Value;
                var unitType = module.Block.GetType("Unit").Value;

                var eventHandler = module.Block.GetEventHandler("daisy").Value;

                Assert.Equal("daisy", eventHandler.Name);
                Assert.Equal(1, eventHandler.Conditions.Count);
            }

            [Fact]
            public void Should_parse_static_int_declaration_with_comment()
            {
                var content = @"
// IGNORE ME
static int goofy;";

                var parser = new SyntaxTreeParser(ParserChannelParserOptionsCallback);
                var module = parser.Parse(content).Module;
                module.Block.GetField("goofy").Resolve();
            }

            [Fact]
            public void Should_parse_enumeration()
            {
                var content = @"
enum Unit {
    ProtossProbe,
    ProtossZealot
}";

                var module = DefaultParser.Parse(content).Module;

                var unit = module.Block.GetType("Unit").Value.Resolve();

                var probeFieldInfo = unit.GetField("ProtossProbe");
                Assert.Equal(0, probeFieldInfo.GetValue<int>());

                var zealotFieldInfo = unit.GetField("ProtossZealot");
                Assert.Equal(1, zealotFieldInfo.GetValue<int>());
            }

            [Fact]
            public void Should_parse_enumeration_with_reserved_names()
            {
                var content = @"
function daisy(bool test_bool);
function goofy() when daisy(false) {}";

                var module = DefaultParser.Parse(content).Module;

                var booleanEnum = module.Block.GetType("Boolean").Value;
                Assert.True(booleanEnum.IsEnum);

                var boolAlias = module.Block.GetType("bool").Value;
                Assert.True(boolAlias.IsAlias);

                var boolAliasBooleanEnum = boolAlias.BaseType.Resolve();
                Assert.True(ReferenceEquals(booleanEnum, boolAliasBooleanEnum));

                var goofyEventHandler = module.Block.GetEventHandler("goofy").Value.Resolve();
                Assert.Equal(booleanEnum, goofyEventHandler.Conditions[0].Arguments[0].ValueType, TypeReferenceEqualityComparer.AfterResolveComparer.Default);
            }

            [Fact]
            public void Should_throw_because_unresolvable_function_parameter()
            {
                var content = "function daisy(inter32 test_int);";
                var aggregatedError = Assert.Throws<BlockResolutionAggregateException>(() => _ = DefaultParser.Parse(content));
                Assert.Contains(aggregatedError.InnerExceptions, error => error.Message.Contains("inter32"));
            }

            [Fact]
            public void Should_throw_because_unresolvable_function_generic_parameter()
            {
                var content = "function daisy<Player PlayerId>();";
                var aggregatedError = Assert.Throws<BlockResolutionAggregateException>(() => _ = DefaultParser.Parse(content));
                Assert.Contains(aggregatedError.InnerExceptions, error => error.Message.Contains("Player"));
            }

            [Fact]
            public void Should_throw_because_unresolvable_event_handler_condition_parameter()
            {
                var content = @"
function cond_one(inter32 int_val);
function daisy() when cond_one<>(2) {}";

                var aggregatedError = Assert.Throws<BlockResolutionAggregateException>(() => _ = DefaultParser.Parse(content));
                Assert.Contains(aggregatedError.InnerExceptions, error => error.Message.Contains("inter32"));
            }

            [Fact]
            public void Should_parse_event_handler_with_enumeration_in_condition()
            {
                var content = @"
enum Player {
    Player2
}

typedef int inter32;

function cond_one<Player PlayerId>(inter32 int_val);
function daisy<Player PlayerId>(inter32 test_int) when cond_one<Player.Player2>(2) {}";

                DefaultParser.Parse(content);
            }

            [Fact]
            public void Should_throw_because_non_static_variable_declaration()
            {
                var content = @"
static int global_var;

function daisy() {
    int global_var;
}";

                Assert.Throws<NotSupportedException>(() => _ = DefaultParser.Parse(content));
            }
        }
    }
}

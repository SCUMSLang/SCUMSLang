using System;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.SyntaxTree.References;
using Xunit;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public class ModuleScoped : TreeParserTests
        {
            [Fact]
            public void Should_parse_static_int_declaration_and_assignment()
            {
                var content = @"
typedef UInt32 int;
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
typedef UInt32 int;
function daisy(int hello) {}
function daisy() {}";

                var module = DefaultParser.Parse(content).Module;

                var daisyHelloFunction = module.Block.GetMethod(new MethodReference("daisy", genericParameters: null, parameters: new[] { new ParameterReference(IntType) }));
                Assert.Equal(1, daisyHelloFunction.Parameters.Count);

                var daisyFunction = module.Block.GetMethod(new MethodReference("daisy", genericParameters: null, parameters: null));
                Assert.Equal(0, daisyFunction.Parameters.Count);
            }

            [Fact]
            public void Should_parse_generic_parameterized_function()
            {
                var content = @"
enum Player { Player2, Player1 }
function daisy<Player PlayerId>() {}";

                var module = DefaultParser.Parse(content).Module;

                var playerEnum = module.Block.GetType("Player").Resolve();
                Assert.Equal(2, playerEnum.Fields.Count);
                Assert.Equal("Player2", playerEnum.Fields[0].Name);
                Assert.Equal("Player1", playerEnum.Fields[1].Name);

                var uint32Type = module.Block.GetType("UInt32").Resolve();
                var player2Field = module.Resolve(new FieldReference("Player2", uint32Type, playerEnum));
                Assert.Equal(0, player2Field.Value);

                var daisyFunction = module.Block.GetMethod("daisy").Resolve();
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
typedef UInt32 int;
static int global_var;

function daisy() {
    global_var = 2;
}";

                var module = DefaultParser.Parse(content).Module;
                var daisyMethod = module.Block.GetMethod("daisy").Resolve();
                Assert.Equal(1, daisyMethod.Body.ReferenceRecords.Count);
                Assert.IsType<MemberAssignmenDefinition>(daisyMethod.Body.ReferenceRecords[0]);
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

                var module = DefaultParser.Parse(content).Module;
                var playerType = module.Block.GetType("Player").Resolve();
                var unitType = module.Block.GetType("Unit").Resolve();

                var eventHandler = module.Block.GetEventHandler("daisy");

                Assert.Equal("daisy", eventHandler.Name);
                Assert.Equal(1, eventHandler.Conditions.Count);
            }

            [Fact]
            public void Should_parse_static_int_declaration_with_comment()
            {
                var content = @"
typedef UInt32 int;
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

                var unit = module.Block.GetType("Unit").Resolve();

                var probeFieldInfo = unit.GetField("ProtossProbe");
                Assert.Equal(0, probeFieldInfo.GetValue<int>());

                var zealotFieldInfo = unit.GetField("ProtossZealot");
                Assert.Equal(1, zealotFieldInfo.GetValue<int>());
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

                var module = DefaultParser.Parse(content).Module;

                var booleanEnum = module.Block.GetType("Boolean");
                Assert.True(booleanEnum.IsEnum);

                var boolAlias = module.Block.GetType("bool");
                Assert.True(boolAlias.IsAlias);

                var boolAliasBooleanEnum = boolAlias.BaseType.Resolve();
                Assert.True(ReferenceEquals(booleanEnum, boolAliasBooleanEnum));

                module.Block.TryGetMemberFirst("goofy", out EventHandlerDefinition goofyEventHandler);
                Assert.Equal(booleanEnum, goofyEventHandler.Conditions[0].Arguments[0].ValueType, TypeReferenceEqualityComparer.ViaResolveComparer.Default);
            }

            [Fact]
            public void Should_throw_because_unresolvable_function_parameter()
            {
                var module = DefaultParser.Parse(@"function daisy(inter32 test_int);").Module;

                var aggregatedError = Assert.Throws<AggregateException>(() => {
                    if (!module.Block.TryGetMemberFirst<MethodDefinition>("daisy", out var method)) {
                        throw new InvalidOperationException();
                    }

                    _ = module.Resolve(method);
                });

                Assert.Contains(aggregatedError.InnerExceptions, error => error.Message.Contains("inter32"));
            }

            [Fact]
            public void Should_throw_because_unresolvable_function_generic_parameter()
            {
                var module = DefaultParser.Parse(@"function daisy<Player PlayerId>();").Module;

                var aggregatedError = Assert.Throws<AggregateException>(() => {
                    if (!module.Block.TryGetMemberFirst<MethodDefinition>("daisy", out var method)) {
                        throw new InvalidOperationException();
                    }

                    _ = module.Resolve(method);
                });

                Assert.Contains(aggregatedError.InnerExceptions, error => error.Message.Contains("Player"));
            }

            [Fact]
            public void Should_throw_because_unresolvable_event_handler_condition_parameter()
            {
                var content = @"
function cond_one(inter32 int_val);
function daisy() when cond_one<>(2) {}";

                var module = DefaultParser.Parse(content).Module;

                var aggregatedError = Assert.Throws<AggregateException>(() => {
                    if (!module.Block.TryGetMemberFirst<EventHandlerDefinition>("daisy", out var eventHandler)) {
                        throw new InvalidOperationException();
                    }

                    eventHandler.Conditions[0].Method.Resolve();
                });

                Assert.Contains(aggregatedError.InnerExceptions, error => error.Message.Contains("inter32"));
            }

            [Fact]
            public void Should_parse_event_handler_without_existing_types() =>
                DefaultParser.Parse(@"function daisy<Player PlayerId>(inter32 test_int) when cond_one<Player.Player2>(""test"") {}");

            [Fact]
            public void Should_throw_because_non_static_variable_declaration()
            {
                var content = @"
static int global_var;

function daisy() {
    int global_var;
}";

                Assert.Throws<NotSupportedException>(() => DefaultParser.Parse(content).Module);
            }
        }
    }
}

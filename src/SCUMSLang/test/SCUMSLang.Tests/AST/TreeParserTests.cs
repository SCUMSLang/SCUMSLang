﻿using System;
using System.Diagnostics;
using SCUMSLang.AST;
using SCUMSLang.Tokenization;
using Xunit;

namespace SCUMSLANG.AST
{
    public class TreeParserTests
    {
        public Action<TreeParserOptions> ParserChannelParserOptionsCallback { get; }
        public TypeDefinition UInt32Type { get; }
        public TypeDefinition IntType { get; }
        public TypeDefinition StringType { get; }
        public TypeDefinition PlayerType { get; }
        public TypeDefinition UnitType { get; }
        public TreeParser DefaultParser { get; }
        public ModuleDefinition ExpectedModule { get; }

        public TreeParserTests()
        {
            Trace.Listeners.Add(new DefaultTraceListener());

            ExpectedModule = new ModuleDefinition()
                .AddSystemTypes();

            ParserChannelParserOptionsCallback = options => {
                options.TokenReaderBehaviour
                    .SetSkipConditionForNonParserChannelTokens();
            };

            UInt32Type = new TypeDefinition(ExpectedModule, "UInt32");
            StringType = new TypeDefinition(ExpectedModule, "String");
            IntType = new TypeDefinition(ExpectedModule, "int") { DeclaringType = new TypeReference("UInt32", ExpectedModule) };

            DefaultParser = new TreeParser(options => {
                options.Module.AddSystemTypes();
            });
        }

        [Fact]
        public void Should_parse_static_int_declaration_and_assignment()
        {
            var content = @"
typedef UInt32 int;
static int goofy = 4;";

            var module = DefaultParser.Parse(content).Module;

            module.Block.GetType(UInt32Type.Name).Resolve();
            module.Block.GetType(IntType.Name).Resolve();
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

            var uint32Type = module.Block.GetType("UInt32").Resolve();
            var intType = module.Block.GetType("int").Resolve();
            var intTypeRootType = intType.ResolveNonAlias();
            Assert.True(ReferenceEquals(uint32Type, intTypeRootType));

            var daisyHelloFunction = module.Block.GetMethod("daisy", parameters: new[] { new ParameterDefinition(uint32Type) });
            Assert.Equal(1, daisyHelloFunction.Parameters.Count);
            Assert.Equal("hello", daisyHelloFunction.Parameters[0].Name);

            var daisyFunction = module.Block.GetMethod("daisy");
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

            var daisyFunction = module.Block.GetMethod("daisy", genericParameters: new[] { new ParameterDefinition(playerEnum) }).Resolve();
            Assert.Equal(1, daisyFunction.GenericParameters.Count);
            Assert.Equal("PlayerId", daisyFunction.GenericParameters[0].Name);
        }

        [Fact]
        public void Should_parse_function_with_declared_assignment()
        {
            var content = @"
typedef UInt32 int;

function daisy() {
    int local_var = 2;
}";

            var module = DefaultParser.Parse(content).Module;

            Assert.Throws<ArgumentException>(() => {
                module.Block.GetField("local_var");
            });

            var daisyFunction = module.Block.GetMethod("daisy").Resolve();
            daisyFunction.Block.GetField("local_var").Resolve();
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
            Assert.Equal(1, daisyMethod.Block.ReferenceRecords.Count);
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

            var eventHandler = module.Block.GetEventHandler(
                "daisy",
                genericParameters: new[] { new ParameterDefinition(unitType) },
                conditions: new[] {
                    new MethodCallDefinition(
                        "cond_one",
                        new []{new ConstantDefinition(playerType, 1)},
                        new []{ new ConstantDefinition(UInt32Type, 15) },
                        module)
                }).Resolve();

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

            var parser = new TreeParser(ParserChannelParserOptionsCallback);
            var module = parser.Parse(content).Module;
            module.Block.GetField("goofy").Resolve();
        }

        [Fact]
        public void Should_parse_short_cut_attribute()
        {
            var content = @"
function TriggerCondition();

[TriggerCondition]";

            var module = DefaultParser.Parse(content).Module;
            module.Block.GetMethod("TriggerCondition").Resolve();
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

            var booleanEnum = module.Block.GetType("Boolean").Resolve();
            Assert.True(booleanEnum.IsEnum);

            var boolAlias = module.Block.GetType("bool").Resolve();
            Assert.True(boolAlias.IsAlias);

            var boolAliasBooleanEnum = boolAlias.BaseType.Resolve();
            Assert.True(ReferenceEquals(booleanEnum, boolAliasBooleanEnum));

            var boolAliasRootType = boolAlias.BaseType.ResolveNonAlias();
            Assert.True(ReferenceEquals(booleanEnum, boolAliasRootType));

            module.Block.TryGetMemberFirst("goofy", out EventHandlerDefinition goofyEventHandler);
             var resolvedGoofyEventHandler = goofyEventHandler.Resolve();
            Assert.Equal(booleanEnum, resolvedGoofyEventHandler.Conditions[0].Arguments[0].ValueType, TypeReferenceEqualityComparer.OverloadComparer.Default);
        }

        [Fact]
        public void Should_throw_because_unresolvable_function_parameter()
        {
            var module = DefaultParser.Parse(@"function daisy(inter32 test_int);").Module;

            Assert.Throws<ArgumentException>(() => {
                if (!module.Block.TryGetMemberFirst<MethodDefinition>("daisy", out var method)) {
                    throw new InvalidOperationException();
                }

                method.Resolve();
            });
        }

        [Fact]
        public void Should_throw_because_unresolvable_function_generic_parameter()
        {
            var module = DefaultParser.Parse(@"function daisy<Player PlayerId>();").Module;

            Assert.Throws<ArgumentException>(() => {
                if (!module.Block.TryGetMemberFirst<MethodDefinition>("daisy", out var method)) {
                    throw new InvalidOperationException();
                }

                method.Resolve();
            });
        }

        [Fact]
        public void Should_throw_because_unresolvable_event_handler_condition_parameter()
        {
            var content = @"
function cond_one(inter32 int_val);
function daisy() when cond_one<>(2) {}";

            var module = DefaultParser.Parse(content).Module;

            Assert.Throws<ArgumentException>(() => {
                if (!module.Block.TryGetMemberFirst<EventHandlerDefinition>("daisy", out var eventHandler)) {
                    throw new InvalidOperationException();
                }

                eventHandler.Resolve();
            });
        }

        [Fact]
        public void Should_parse_event_handler_without_existing_types() =>
            DefaultParser.Parse(@"function daisy<Player PlayerId>(inter32 test_int) when cond_one<Player.Player2>(""test"") {}");
    }
}
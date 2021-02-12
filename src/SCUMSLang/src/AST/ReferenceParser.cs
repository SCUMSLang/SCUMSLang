using SCUMSLang.Tokenization;
using System;
using static SCUMSLang.AST.ReferenceParserTools;

namespace SCUMSLang.AST
{
    public class ReferenceParser
    {
        public static ReferenceParser Default = new ReferenceParser();

        public ReferenceParserOptions Options { get; }

        public ReferenceParser() =>
            Options = new ReferenceParserOptions();

        public ReferenceParser(Action<ReferenceParserOptions> optionsCallback)
        {
            var options = new ReferenceParserOptions();
            optionsCallback(options);
            Options = options;
        }

        /// <summary>
        /// Parses tokens.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public ReferenceParserResult Parse(ReadOnlySpan<Token> span)
        {
            var tokenReader = new SpanReader<Token>(span, Options.TokenReaderBehaviour);
            var lastRecognizedUpperPosition = -1;
            var block = Options.Module;

            if (!tokenReader.SetPositionTo(Options.TokenReaderStartPosition)) {
                goto exit;
            }

            while (tokenReader.ConsumeNext()) {
                int? newPosition;

                if (tokenReader.ViewLastValue.TryRecognize(TokenType.CloseBrace)) {
                    block.CurrentBlock.EndBlock();
                    newPosition = tokenReader.UpperPosition;
                } else {
                    newPosition = TryRecognize(block.CurrentBlock, tokenReader, Options.RecognizableNodes, out var node);

                    if (newPosition == null) {
                        if (Options.EmptyRecognizationResultsIntoWhileBreak) {
                            break;
                        } else {
                            throw new ParseException(tokenReader.ViewLastValue, "A valid programming structure could not be found.");
                        }
                    } else {
                        lastRecognizedUpperPosition = newPosition.Value;
                    }

                    if (!(node is null)) {
                        if (Options.WhileContinueDelegate?.Invoke(node) ?? false) {
                            continue;
                        } else if (Options.WhileBreakDelegate?.Invoke(node) ?? false) {
                            break;
                        } else if (node is TypeDefinition typeDefinition) {
                            block.AddNode(typeDefinition);
                        } else if (node is DeclarationReference declarationNode) {
                            block.CurrentBlock.AddNode(declarationNode);
                        } else if (node is AssignDefinition assignNode) {
                            block.CurrentBlock.AddAssignment(assignNode);
                        } else if (node is FunctionReference functionNode) {
                            if (functionNode.IsAbstract) {
                                block.AddNode(functionNode);
                            } else {
                                block.BeginBlock(functionNode);
                            }
                        } else {
                            block.AddNode(node);
                        }
                    }
                }

                if (!tokenReader.SetPositionTo(newPosition.Value + 1)) {
                    tokenReader.SetPositionTo(newPosition.Value);
                    break;
                }
            }

            exit:
            return new ReferenceParserResult(block, lastRecognizedUpperPosition);
        }
    }
}

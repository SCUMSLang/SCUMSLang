using SCUMSLang.Tokenization;
using System;
using static SCUMSLang.AST.ParserTools;

namespace SCUMSLang.AST
{
    public class Parser
    {
        public static Parser Default = new Parser();

        public ParserOptions Options { get; }

        public Parser() =>
            Options = new ParserOptions();

        public Parser(Action<ParserOptions> optionsCallback)
        {
            var options = new ParserOptions();
            optionsCallback(options);
            Options = options;
        }

        /// <summary>
        /// Parses tokens.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public ParserResult Parse(ReadOnlySpan<Token> span)
        {
            var tokenReader = new SpanReader<Token>(span, Options.TokenReaderBehaviour);
            var lastRecognizedUpperPosition = -1;
            var block = Options.StaticBlock;

            if (!tokenReader.SetPositionTo(Options.TokenReaderStartPosition)) {
                goto exit;
            }

            while (tokenReader.ConsumeNext()) {
                int? newPosition;

                if (tokenReader.ViewLastValue.TryRecognize(TokenType.CloseBrace)) {
                    block.CurrentBlock.EndBlock();
                    newPosition = tokenReader.UpperPosition;
                } else {
                    newPosition = RecognizeNode(block.CurrentBlock, tokenReader, Options.RecognizableNodes, out var node);

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
                        } else if (node is TypeDefinitionNode typeDefinition) {
                            block.AddNode(typeDefinition);
                        } else if (node is DeclarationNode declarationNode) {
                            block.CurrentBlock.AddNode(declarationNode);
                        } else if (node is AssignNode assignNode) {
                            block.CurrentBlock.AddAssignment(assignNode);
                        } else if (node is FunctionNode functionNode) {
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
            return new ParserResult(block, lastRecognizedUpperPosition);
        }
    }
}

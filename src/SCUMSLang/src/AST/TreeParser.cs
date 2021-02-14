using SCUMSLang.Tokenization;
using System;
using static SCUMSLang.AST.TreeParserTools;

namespace SCUMSLang.AST
{
    public class TreeParser
    {
        public static TreeParser Default = new TreeParser();

        public TreeParserOptions Options { get; }

        public TreeParser() =>
            Options = new TreeParserOptions();

        public TreeParser(Action<TreeParserOptions> optionsCallback)
        {
            var options = new TreeParserOptions();
            optionsCallback(options);
            Options = options;
        }

        /// <summary>
        /// Parses tokens.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public TreeParserResult Parse(ReadOnlySpan<Token> span)
        {
            var tokenReader = new SpanReader<Token>(span, Options.TokenReaderBehaviour);
            var lastRecognizedUpperPosition = -1;
            var module = Options.Module;
            var block = module.Block;

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
                        } else if (node is FieldReference filed) {
                            block.CurrentBlock.AddNode(filed);
                        } else if (node is AssignDefinition assignment) {
                            block.CurrentBlock.AddNode(assignment);
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
            return new TreeParserResult(module, lastRecognizedUpperPosition);
        }
    }
}

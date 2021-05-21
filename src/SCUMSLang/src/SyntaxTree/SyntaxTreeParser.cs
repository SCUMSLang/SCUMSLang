using SCUMSLang.Tokenization;
using System;
using System.Collections.Generic;
using static SCUMSLang.SyntaxTree.SyntaxTreeParserTools;

namespace SCUMSLang.SyntaxTree
{
    public class SyntaxTreeParser
    {
        public static SyntaxTreeParser Default = new SyntaxTreeParser();

        public SyntaxTreeParserOptions Options { get; }

        public SyntaxTreeParser() =>
            Options = new SyntaxTreeParserOptions();

        public SyntaxTreeParser(Action<SyntaxTreeParserOptions> optionsCallback)
        {
            var options = new SyntaxTreeParserOptions();
            optionsCallback(options);
            Options = options;
        }

        /// <summary>
        /// Parses tokens.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public SyntaxTreeParserResult Parse(ReadOnlySpan<Token> span)
        {
            var tokenReader = new SpanReader<Token>(span, Options.TokenReaderBehaviour);
            var lastRecognizedUpperPosition = -1;
            var module = Options.Module;
            var block = module.Block;
            var blockWalker = new BlockDefinition.BlockWalker(block);
            var precedingAttributes = new List<AttributeDefinition>();

            if (!tokenReader.SetPositionTo(Options.TokenReaderStartPosition)) {
                goto exit;
            }

            while (tokenReader.ConsumeNext()) {
                int? newPosition;

                if (tokenReader.ViewLastValue.TryRecognize(TokenType.CloseBrace)) {
                    blockWalker.EndCurrentBlock();
                    newPosition = tokenReader.UpperPosition;
                } else {
                    newPosition = Recognize(blockWalker.CurrentBlock, tokenReader, Options.RecognizableNodes, out var node);

                    if (newPosition == null) {
                        if (Options.EmptyRecognizationResultsIntoWhileBreak) {
                            break;
                        } else {
                            throw new SyntaxTreeParsingException(tokenReader.ViewLastValue, "A valid programming structure could not be identified.");
                        }
                    } else {
                        lastRecognizedUpperPosition = newPosition.Value;
                    }

                    if (!(node is null)) {
                        if (Options.WhileContinueDelegate?.Invoke(node) ?? false) {
                            continue;
                        } else if (Options.WhileBreakDelegate?.Invoke(node) ?? false) {
                            break;
                        } else {
                            if (node is AttributeDefinition precedingAttribute) {
                                precedingAttributes.Add(precedingAttribute);
                            } else if (precedingAttributes.Count > 0) {
                                if (node is IAttributesHolder attributesHolder) {
                                    foreach (var attribute in precedingAttributes) {
                                        attributesHolder.Attributes.Add(attribute);
                                    }

                                    precedingAttributes.Clear();
                                } else {
                                    throw SyntaxTreeThrowHelper.AttributeMisposition(node.GetType().Name);
                                }
                            }

                            blockWalker.CurrentBlock.AddNode(node);
                            // Try begin another block AFTER the block
                            // of the node could be initialized from the
                            // adding procedure.
                            blockWalker.TryBeginAnotherBlock(node);
                        }
                    }
                }

                if (!tokenReader.SetPositionTo(newPosition.Value + 1)) {
                    tokenReader.SetPositionTo(newPosition.Value);
                    break;
                }
            }

            exit:

            if (precedingAttributes.Count > 0) {
                throw SyntaxTreeThrowHelper.AttributeMisposition();
            }

            return new SyntaxTreeParserResult(module, lastRecognizedUpperPosition);
        }
    }
}

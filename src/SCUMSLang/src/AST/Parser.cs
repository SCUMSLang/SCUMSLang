using SCUMSLang.Tokenization;
using System;
using static SCUMSLang.AST.ParserTools;

namespace SCUMSLang.AST
{
    public class Parser
    {
        public static Parser Default = new Parser();

        /// <summary>
        /// Parses tokens.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public BlockNode Parse(ReadOnlySpan<Token> span, ParserOptions? options = null)
        {
            var tokenReader = new SpanReader<Token>(span, options?.TokenReaderBehaviour);
            var block = new StaticBlockNode();

            while (tokenReader.ConsumeNext()) {
                int? newPosition;

                if (tokenReader.ViewLastValue.TryRecognize(TokenType.CloseBrace)) {
                    block.CurrentBlock.EndBlock();
                    newPosition = tokenReader.UpperPosition;
                } else {
                    newPosition = RecognizeNode(tokenReader, out var node);

                    if (newPosition == null) {
                        throw new ParseException(tokenReader.ViewLastValue, "A valid programming structure could not be found.");
                    }

                    if (!(node is null)) {
                        if (!options?.NodeSkipDelegate?.Invoke(node) ?? false) {
                            continue;
                        } else if (!options?.NodeEndDelegate?.Invoke(node) ?? false) {
                            break;
                        } else if (node is DeclarationNode declarationNode) {
                            block.CurrentBlock.AddDeclaration(declarationNode);
                        } else if (node is AssignNode assignNode) {
                            block.CurrentBlock.AddAssignment(assignNode);
                        } else if (node is AttributeNode attribute){
                            block.CurrentBlock.AddAttribute(attribute);
                        } else if (node is FunctionNode functionNode) {
                            if (functionNode.IsAbstract) {
                                block.AddFunction(functionNode);
                            } else {
                                block.BeginBlock(functionNode);
                            }
                        } else {
                            block.AddNode(node);
                        }
                    }
                }

                if (!tokenReader.SetPositionTo(newPosition.Value + 1)) {
                    break;
                }
            }

            return block;
        }
    }
}

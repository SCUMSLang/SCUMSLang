using SCUMSLang.Tokenization;
using System;
using static SCUMSLang.AST.ParserTools;

namespace SCUMSLang.AST
{
    public static class Parser
    {
        public static BlockNode Parse(ReadOnlySpan<Token> span)
        {
            var tokenReader = new Reader<Token>(span);
            var block = new StaticBlockNode();

            while (tokenReader.ConsumeNext()) {
                int? newPosition;

                if (tokenReader.View[0].TryRecognize(TokenType.CloseBrace)) {
                    block.CurrentBlock.EndBlock();
                    newPosition = tokenReader.UpperPosition;
                } else {
                    newPosition = RecognizeNode(tokenReader, out var node);

                    if (newPosition == null) {
                        throw new ParseException(tokenReader.View.Last(), "A valid programming structure could not be found.");
                    }

                    if (!(node is null)) {
                        if (node is DeclarationNode declarationNode) {
                            block.CurrentBlock.AddDeclaration(declarationNode);
                        } else if (node is AssignNode assignNode) {
                            block.CurrentBlock.AddAssignment(assignNode);
                        } else if (node is FunctionNode functionNode) {
                            block.BeginBlock(functionNode);
                        }
                    }
                }

                tokenReader.SetPositionTo(newPosition.Value + 1);
            }

            return block;
        }

        public static BlockNode Parse(string content)
        {
            var tokens = Tokenizer.Tokenize(content);
            var tokenArray = tokens.ToArray();
            return Parse(tokenArray);
        }
    }
}

using SCUMSLang.Tokenization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.AST
{
    public static class ParserTools
    {
        public static bool TryRecognizeDeclarationNode(Reader<Token> reader, TokenType tokenValueType, out int? newPosition, [MaybeNull] out Node node)
        {
            var tokens = reader.View;
            Scope scope;

            if (tokens[0] == TokenType.StaticKeyword)
            {
                scope = Scope.Static;
            }
            else
            {
                scope = Scope.Local;
            }

            if (reader.ConsumeNext(tokenValueType, out var valueTypeToken)
                && reader.ConsumeNext(TokenType.Name, out var nameToken))
            {
                reader.ConsumePrevious(1);

                var nodeValueType = (Attribute.GetCustomAttribute(
                    TokenTypeLibrary.TypeOfTokenType.GetField(Enum.GetName(TokenTypeLibrary.TypeOfTokenType, tokenValueType)!)!,
                    typeof(NodeValueTypeAttribute)) as NodeValueTypeAttribute
                    ?? throw new ArgumentException("The token type has not the information about a node value type."))
                    .ValueType;

                node = new DeclarationNode(scope, nodeValueType, (string)nameToken.Value!);
                newPosition = reader.UpperPosition;
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeAssignmentNode(Reader<Token> reader, TokenType valueType, out int? newPosition, [MaybeNull] out Node node)
        {
            var tokens = reader.View;

            if (tokens[0].TryRecognize(TokenType.Name, out string name)
                && reader.ConsumeNext(TokenType.EqualSign)
                && reader.ConsumeNext(valueType, out var valueToken))
            {
                if (!reader.ConsumeNext(TokenType.Semicolon))
                {
                    throw new MissingTokenException(valueToken, TokenType.Semicolon);
                }

                newPosition = reader.UpperPosition;
                node = new AssignNode(name, new ConstantNode(NodeValueType.Integer, valueToken.Value!));
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static bool TryRecognizeParameters(ref Reader<Token> reader, TokenType openTokenType, TokenType closeTokenType, out List<DeclarationNode> functionParameters)
        {
            functionParameters = new List<DeclarationNode>();

            if (reader.View[0].TryRecognize(openTokenType))
            {
                while (reader.ConsumeNext(out Token lastToken) && lastToken.TryRecognize(TokenTypeLibrary.ValueTypes))
                {
                    if (!reader.ConsumeNext(TokenType.Name, out var parameterNameToken))
                    {
                        throw new ParseException(reader.ViewLastValue, "Parameter name is missing");
                    }

                    var nodeValueType = TokenTypeTools.GetNodeValueType(lastToken.TokenType);
                    functionParameters.Add(new DeclarationNode(Scope.Local, nodeValueType, parameterNameToken.GetValue<string>()));

                    if (!reader.ConsumeNext(out lastToken) && !lastToken.TryRecognize(TokenType.Comma, closeTokenType))
                    {
                        throw new MissingTokenException(reader.ViewLastValue, closeTokenType);
                    }

                    if (lastToken.TokenType == closeTokenType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<ConstantNode> RecognizeArgumentList(ref Reader<Token> reader, TokenType endTokenType, params TokenType[] allowedValueTypeTokens)
        {
            var constantNodes = new List<ConstantNode>();

            do
            {
                if (reader.ViewLastValue == endTokenType)
                {
                    return constantNodes;
                }

                if (!reader.ConsumeNext(out Token nextToken) || !nextToken.TryRecognize(allowedValueTypeTokens))
                {
                    throw new ParseException(reader.ViewLastValue, $"A valid value was expected"); // TODO: Concrete error
                }

                if (reader.PeekNext(TokenType.Comma))
                {
                    reader.ConsumeNext();
                }
            } while (true);
        }

        public static bool TryRecognizeFunctionCallNode(Reader<Token> reader, out int? newPosition, [MaybeNull] out FunctionCallNode node)
        {
            if (reader.ViewLastValue == TokenType.Name
                && reader.ConsumeNext(out ReaderPosition<Token> peekedToken)
                && peekedToken.Value.TryRecognize(TokenType.OpenAngleBracket, TokenType.OpenBracket))
            {
                if (!reader.TakeNextPositionView())
                {
                    throw new ParseException(reader.ViewLastValue, "Condition must have a signature (e.g. '<...>() or '()').");
                }

                List<ConstantNode> genericArguments;

                if (peekedToken.Value == TokenType.OpenAngleBracket)
                {
                    genericArguments = RecognizeArgumentList(ref reader, TokenType.CloseAngleBracket);
                }
                else
                {
                    genericArguments = new List<ConstantNode>();

                    if (!reader.TakeNextPositionView(TokenType.OpenBracket))
                    {
                        throw new ParseException(reader.ViewLastValue, "Condition must be folled by '()'/'(...)'.");
                    }
                }

                var arguments = RecognizeArgumentList(ref reader, TokenType.CloseBracket);
                newPosition = reader.UpperPosition;
                node = new FunctionCallNode(genericArguments, arguments);
                return true;
            }

            newPosition = 0;
            node = null;
            return false;
        }

        public static bool TryRecognizeFunctionNode(Reader<Token> reader, out int? newPosition, [MaybeNull] out Node node)
        {
            if (reader.View[0].TryRecognize(TokenType.FunctionKeyword, out var functionToken))
            {
                if (!reader.ConsumeNext(TokenType.Name, out var functionNameToken))
                {
                    throw new ParseException(functionToken, "Function must have a name.");
                }

                if (reader.ConsumeNext(out ReaderPosition<Token> test))
                {
                    // Prepare for recognizing parameters.
                    reader.TakePositionView(test);
                }
                else
                {
                    throw new ParseException(reader.ViewLastValue, "Function must have a signature (e.g. '<...>()' or '()'.");
                }

                if (TryRecognizeParameters(ref reader, TokenType.OpenAngleBracket, TokenType.CloseAngleBracket, out var genericParameters)
                    && !reader.TakeNextPositionView(TokenType.OpenBracket))
                {
                    throw new ParseException(reader.ViewLastValue, "Function must have parameter list (e.g. '()').");
                }

                TryRecognizeParameters(ref reader, TokenType.OpenBracket, TokenType.CloseBracket, out var parameters);

                if (!reader.ConsumeNext(out Token openBracketNode) || !openBracketNode.TryRecognize(TokenType.OpenBrace, TokenType.WhenKeyword))
                {
                    throw new ParseException(reader.ViewLastValue, "Function must have a block (e.g. '{}') or 'when' keyword after the signature.");
                }

                if (openBracketNode.TokenType == TokenType.OpenBrace)
                {
                    newPosition = reader.UpperPosition;
                    node = new FunctionNode(functionNameToken.GetValue<string>(), genericParameters, parameters);
                    return true;
                }
                else
                {

                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static int? RecognizeNode(Reader<Token> reader, [MaybeNull] out Node node)
        {
            int? newPosition;

            if (TryRecognizeDeclarationNode(reader, TokenType.IntKeyword, out newPosition, out node))
            {
                return newPosition;
            }

            if (TryRecognizeAssignmentNode(reader, TokenType.Integer, out newPosition, out node))
            {
                return newPosition;
            }

            if (TryRecognizeFunctionNode(reader, out newPosition, out node))
            {
                return newPosition;
            }

            node = null;
            return 0;
        }
    }
}

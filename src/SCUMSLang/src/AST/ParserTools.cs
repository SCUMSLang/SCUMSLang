﻿using SCUMSLang.Tokenization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.AST
{
    public static class ParserTools
    {
        private static bool TryRecognizeImportNode(SpanReader<Token> reader, out int? newPosition, [MaybeNullWhen(false)] out Node node)
        {
            if (reader.ViewLastValue == TokenType.ImportKeyword
                && reader.ConsumeNext(out Token stringToken)
                && reader.ConsumeNext(TokenType.Semicolon)) {
                newPosition = reader.UpperPosition;
                node = new ImportNode(stringToken.GetValue<string>());
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeDeclarationNode(SpanReader<Token> reader, TokenType tokenValueType, out int? newPosition, [MaybeNull] out Node node)
        {
            Scope scope;

            if (reader.ViewLastValue == TokenType.StaticKeyword) {
                scope = Scope.Static;

                if (!reader.ConsumeNext()) {
                    throw new ArgumentException("A value type was expected.");
                }
            } else {
                scope = Scope.Local;
            }

            bool hasSemicolon;

            if (reader.PeekNext(out var nameToken) && nameToken.Value.TokenType == TokenType.Name
                && reader.PeekNext(2, out var afterNameToken)
                && afterNameToken.Value.TryRecognize(TokenType.EqualSign, TokenType.Semicolon)) {
                if (afterNameToken.Value.TokenType == TokenType.Semicolon) {
                    newPosition = afterNameToken.UpperReaderPosition;
                } else {
                    newPosition = reader.UpperPosition;
                }

                var nodeValueType = (Attribute.GetCustomAttribute(
                    TokenTypeLibrary.TypeOfTokenType.GetField(Enum.GetName(TokenTypeLibrary.TypeOfTokenType, tokenValueType)!)!,
                    typeof(NodeValueTypeAttribute)) as NodeValueTypeAttribute
                    ?? throw new ArgumentException("The token type has not the information about a node value type."))
                    .ValueType;

                node = new DeclarationNode(scope, nodeValueType, nameToken.Value.GetValue<string>());
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeAssignmentNode(SpanReader<Token> reader, TokenType valueType, out int? newPosition, [MaybeNull] out Node node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.Name, out string name)
                && reader.ConsumeNext(TokenType.EqualSign)
                && reader.ConsumeNext(valueType, out var valueToken)) {
                if (!reader.ConsumeNext(TokenType.Semicolon)) {
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
        public static bool TryRecognizeParameters(
            SpanReader<Token> reader,
            TokenType openTokenType,
            TokenType closeTokenType,
            out List<DeclarationNode> functionParameters,
            [NotNullWhen(true)] out int? newPosition,
            bool required = false,
            bool needConsume = false)
        {
            functionParameters = new List<DeclarationNode>();

            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue.TryRecognize(openTokenType)) {
                while (reader.ConsumeNext(out Token lastToken)) {
                    var hasValueType = lastToken.TryRecognize(TokenTypeLibrary.ValueTypes);
                    var hasEndCloseType = lastToken.TokenType == closeTokenType;

                    if (!hasValueType && !hasEndCloseType) {
                        if (reader.PeekNext(TokenType.Name)) {
                            throw new ParseException(reader.ViewLastValue, "Parameter unit type is incorrect.");
                        } else {
                            throw new ParseException(reader.ViewLastValue, "Parameter list must be closed.");
                        }
                    }

                    if (hasEndCloseType) {
                        break;
                    }

                    if (!reader.ConsumeNext(TokenType.Name, out var parameterNameToken)) {
                        throw new ParseException(reader.ViewLastValue, "Parameter name is missing.");
                    }

                    var nodeValueType = TokenTypeTools.GetNodeValueType(lastToken.TokenType);
                    functionParameters.Add(new DeclarationNode(Scope.Local, nodeValueType, parameterNameToken.GetValue<string>()));

                    if (reader.PeekNext(TokenType.Comma)) {
                        reader.ConsumeNext();
                    }
                }

                newPosition = reader.UpperPosition;
                return true;
            } else if (required) {
                throw new ParseException(reader.ViewLastValue, "Parameter list was expected.");
            }

            newPosition = null;
            return false;
        }

        public static bool TryRecognizeConstantNode(ReaderPosition<Token> position, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out ConstantNode constant)
        {
            var token = position.Value;
            NodeValueType? nullableValueType;

            if (TokenTypeTools.TryGetNodeValueType(token.TokenType, out nullableValueType)) {
                newPosition = position.UpperReaderPosition;
                constant = new ConstantNode(nullableValueType.Value, position.Value);
                return true;
            }

            if (token.TokenType == TokenType.Name
                && token.TryGetValue(out string? stringValue)) {
                if (Enum.TryParse<Player>(stringValue, out var valueType)) {
                    newPosition = position.UpperReaderPosition;
                    constant = new ConstantNode(NodeValueType.Player, valueType);
                    return true;
                }
            }

            newPosition = null;
            constant = null;
            return false;
        }

        public static bool TryRecognizeAttributeNode(SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Node node)
        {
            if (reader.ViewLastValue == TokenType.OpenSquareBracket
                && reader.ConsumeNext(TokenType.Name, out var nameTokenPosition)) {
                List<ConstantNode>? arguments;

                if (reader.PeekNext(TokenType.OpenBracket)
                    && TryRecognizeArgumentList(reader, TokenType.OpenBracket, TokenType.CloseBracket, out newPosition, out arguments, required: true, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                } else {
                    arguments = null;
                }

                if (reader.ConsumeNext(TokenType.CloseSquareBracket)) {
                    newPosition = reader.UpperPosition;
                    node = new AttributeNode(new FunctionCallNode(nameTokenPosition.GetValue<string>(), null, arguments));
                    return true;
                } else {
                    throw new ParseException(reader.ViewLastValue, "The attribute needs to be closed by ']'.");
                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeArgumentList(
            SpanReader<Token> reader,
            TokenType openTokenType,
            TokenType endTokenType,
            [NotNullWhen(true)] out int? newPosition,
            out List<ConstantNode> arguments,
            bool required = false,
            bool needConsume = false)
        {
            arguments = new List<ConstantNode>();
            bool hadPreviousComma = false;

            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue == openTokenType) {
                do {
                    if (!reader.ConsumeNext(out ReaderPosition<Token> tokenPosition)) {
                        throw new ParseException(reader.ViewLastValue, "The argument list needs to be enclosed.");
                    }

                    if (!hadPreviousComma && tokenPosition.Value == endTokenType) {
                        newPosition = reader.UpperPosition;
                        return true;
                    }

                    if (!TryRecognizeConstantNode(tokenPosition, out _, out var node)) {
                        // When you cannot consume or can consume but can not recognize constant, then throw.
                        throw new ParseException(reader.ViewLastValue, $"A value was expected."); // TODO: Concrete error
                    }

                    arguments.Add(node);

                    if (reader.PeekNext(out var peekedToken) && peekedToken.Value == TokenType.Comma) {
                        hadPreviousComma = true;

                        if (!reader.ConsumeNext()) {
                            throw new ParseException(reader.ViewLastValue, "Another value was expected.");
                        }
                    } else {
                        hadPreviousComma = false;
                    }
                } while (true);
            } else if (required) {
                throw new ParseException(reader.ViewLastValue, "A argument list was expected.");
            }

            newPosition = null;
            return false;
        }

        public static bool TryRecognizeFunctionCallNode(
            SpanReader<Token> reader,
            [NotNullWhen(true)] out int? newPosition,
            [MaybeNullWhen(false)] out FunctionCallNode node,
            bool required = false,
            bool needConsume = false)
        {
            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue.TryRecognize(TokenType.Name, out var nameToken)
                && reader.PeekNext(out ReaderPosition<Token> peekedToken)
                && peekedToken.Value.TryRecognize(TokenType.OpenAngleBracket, TokenType.OpenBracket)) {
                List<ConstantNode> genericArguments;

                if (peekedToken.Value == TokenType.OpenAngleBracket
                    && TryRecognizeArgumentList(
                        reader,
                        TokenType.OpenAngleBracket,
                        TokenType.CloseAngleBracket,
                        out newPosition,
                        out genericArguments,
                        needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                } else {
                    genericArguments = new List<ConstantNode>();
                }

                if (TryRecognizeArgumentList(reader, TokenType.OpenBracket, TokenType.CloseBracket, out newPosition, out var arguments, required: true, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                    node = new FunctionCallNode(nameToken.GetValue<string>(), genericArguments, arguments);
                    return true;
                }
            } else if (required) {
                throw new ParseException(reader.ViewLastValue, "Function call was expected.");
            }

            newPosition = 0;
            node = null;
            return false;
        }

        public static bool TryRecognizeFunctionOrEventHandlerNode(SpanReader<Token> reader, out int? newPosition, [MaybeNull] out Node node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.FunctionKeyword, out var functionToken)) {
                if (!reader.ConsumeNext(TokenType.Name, out var functionNameToken)) {
                    throw new ParseException(functionToken, "Function must have a name.");
                }

                if (TryRecognizeParameters(reader, TokenType.OpenAngleBracket, TokenType.CloseAngleBracket, out var genericParameters, out newPosition, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                }

                if (TryRecognizeParameters(reader, TokenType.OpenBracket, TokenType.CloseBracket, out var parameters, out newPosition, required: true, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                }

                if (!reader.ConsumeNext(out Token afterSignatureNode) || !afterSignatureNode.TryRecognize(TokenType.OpenBrace, TokenType.WhenKeyword, TokenType.Semicolon)) {
                    throw new ParseException(reader.ViewLastValue, "Function must have a block (e.g. '{}'), followed by 'when' keyword or end with a semicolon after the signature.");
                }

                if (afterSignatureNode.TryRecognize(TokenType.OpenBrace, TokenType.Semicolon)) {
                    newPosition = reader.UpperPosition;
                    var isFunctionAbstract = afterSignatureNode.TokenType == TokenType.Semicolon;

                    node = new FunctionNode(
                        functionNameToken.GetValue<string>(),
                        genericParameters,
                        parameters,
                        isAbstract: isFunctionAbstract);

                    return true;
                } else {
                    var conditions = new List<FunctionCallNode>();

                    do {
                        if (TryRecognizeFunctionCallNode(reader, out newPosition, out var condition, required: true, needConsume: true)) {
                            reader.SetLengthTo(newPosition.Value);
                            conditions.Add(condition);
                        }

                        if (reader.ConsumeNext(TokenType.OpenBrace)) {
                            newPosition = reader.UpperPosition;
                            node = new EventHandlerNode(functionNameToken.GetValue<string>(), genericParameters, parameters, conditions);
                            return true;
                        }

                        if (!reader.ConsumeNext(TokenType.AndKeyword)) {
                            throw new ParseException(reader.ViewLastValue, "As long as the function block is not opening (e.g. '{') another condition is expected.");
                        }
                    } while (true);
                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static int? RecognizeNode(SpanReader<Token> reader, [MaybeNull] out Node node)
        {
            int? newPosition;

            if (TryRecognizeImportNode(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (TryRecognizeAttributeNode(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (TryRecognizeFunctionOrEventHandlerNode(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (TryRecognizeDeclarationNode(reader, TokenType.IntKeyword, out newPosition, out node)) {
                return newPosition;
            }

            if (TryRecognizeAssignmentNode(reader, TokenType.Integer, out newPosition, out node)) {
                return newPosition;
            }

            node = null;
            return null;
        }
    }
}

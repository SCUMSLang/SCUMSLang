using SCUMSLang.Tokenization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SCUMSLang.SyntaxTree
{
    public static class SyntaxTreeParserTools
    {
        public static bool TryRecognizeImport(ModuleDefinition module, SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.ImportKeyword
                && reader.ConsumeNext(out Token stringToken)
                && reader.ConsumeNext(TokenType.Semicolon)) {
                string importPath;
                var importBasePath = stringToken.GetValue<string>();

                if (module.FilePath is null) {
                    importPath = importBasePath;
                } else {
                    var moduleDirectoryName = Path.GetDirectoryName(module.FilePath)!;
                    importPath = Path.GetFullPath(importBasePath, moduleDirectoryName);
                }

                newPosition = reader.UpperPosition;
                node = new ImportDefinition(importPath);
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeNameList(
            SpanReader<Token> reader,
            TokenType openToken,
            TokenType closeToken,
            [NotNullWhen(true)] out int? newPosition,
            [MaybeNullWhen(false)] out List<string> nameList,
            bool required = false,
            bool needConsume = false)
        {
            if (reader.ConsumeNext(needConsume)) {
                if (reader.ViewLastValue == openToken) {
                    nameList = new List<string>();

                    while (reader.PeekNext(out ReaderPosition<Token> nameTokenPosition)
                        && nameTokenPosition.Value.Value is string name
                        && !string.IsNullOrEmpty(name)) {
                        reader.SetLengthTo(nameTokenPosition.UpperReaderPosition);
                        nameList.Add(name);

                        if (reader.PeekNext(TokenType.Comma)) {
                            reader.SetPositionTo(reader.UpperPosition + 1, length: 1);
                            continue;
                        }
                    }

                    if (!reader.ConsumeNext(closeToken)) {
                        throw new SyntaxTreeParsingException(reader.ViewLastPosition, $"The list needs to be closed by '{TokenTypeLibrary.SequenceDictionary[closeToken]}'.");
                    }

                    newPosition = reader.UpperPosition;
                    return true;
                }
            }

            if (required) {
                throw new SyntaxTreeParsingException(reader.ViewLastPosition, "The enumeration needs to be openend by a brace ('{').");
            }

            newPosition = null;
            nameList = null;
            return false;
        }

        public static int ExpectToken(SpanReader<Token> reader, TokenType tokenType, bool needConsume = false)
        {
            if (!reader.ConsumeNext(needConsume) || !reader.ViewLastValue.TryRecognize(tokenType)) {
                throw new SyntaxTreeParsingException(reader.ViewLastValue, $"Specific token was expected: '{TokenTypeLibrary.SequenceDictionary[tokenType]}'");
            }

            return reader.UpperPosition;
        }

        public static bool TryRecognizeTypeAliasReference(SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.TypeDefKeyword) {
                if (!reader.ConsumeNext(out ReaderPosition<Token> typeTokenPosition)) {
                    throw new SyntaxTreeParsingException(typeTokenPosition, "A type was expected.");
                }

                static ReaderPosition<Token> expectNameAndDelimiterToken(ref SpanReader<Token> reader, out int? newPosition)
                {
                    if (!reader.ConsumeNext(out ReaderPosition<Token> nameToken)
                        || string.IsNullOrEmpty(nameToken.Value?.ToString())) {
                        throw new SyntaxTreeParsingException(reader.ViewLastPosition, "A name was expected.");
                    }

                    newPosition = ExpectToken(reader, TokenType.Semicolon, needConsume: true);
                    return nameToken;
                }

                if (typeTokenPosition.Value == TokenType.EnumKeyword) {
                    _ = TryRecognizeNameList(reader, TokenType.OpenBrace, TokenType.CloseBrace, out newPosition, out var nameList, required: true, needConsume: true);
                    reader.SetLengthTo(newPosition!.Value);

                    var enumNameToken = expectNameAndDelimiterToken(ref reader, out newPosition!);
                    var enumName = enumNameToken.Value.GetValue<string>();
                    node = Reference.CreateEnumDefinition(enumName, nameList!, fieldsAreConstants: true);
                    return true;
                }

                {
                    var sourceTypeName = typeTokenPosition.Value.GetValue<string>();
                    var aliasNameToken = expectNameAndDelimiterToken(ref reader, out newPosition!);
                    var aliasName = aliasNameToken.Value.GetValue<string>();
                    node = TypeDefinition.CreateAliasDefinition(aliasName, new TypeReference(sourceTypeName));
                    return true;
                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeEnumeration(SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.EnumKeyword) {
                if (!reader.ConsumeNext(out ReaderPosition<Token> enumNameTokenPosition)
                    || enumNameTokenPosition.Value != TokenType.Name) {
                    throw new SyntaxTreeParsingException(reader.ViewLastPosition, "A name after the keyword 'enum' was expected.");
                }

                _ = TryRecognizeNameList(reader, TokenType.OpenBrace, TokenType.CloseBrace, out newPosition, out var nameList, required: true, needConsume: true);
                reader.SetLengthTo(newPosition!.Value);

                var enumName = enumNameTokenPosition.Value.GetValue<string>();
                var enumType = Reference.CreateEnumDefinition(enumName, nameList!);
                newPosition = reader.UpperPosition;
                node = enumType;
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static int ExpectTypeReference(SpanReader<Token> reader, out TypeReference typeReference, bool needConsume = false)
        {
            if (!reader.ConsumeNext(needConsume)
                || !(reader.ViewLastValue.Value is string typeName)
                || string.IsNullOrEmpty(typeName)) {
                throw new SyntaxTreeParsingException(reader.ViewLastValue, "A type was expected.");
            }

            typeReference = new TypeReference(typeName);
            return reader.UpperPosition;
        }

        public static int ExpectArgumentTypeReference(SpanReader<Token> reader, out TypeReference typeReference)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.ParamsKeyword)) {
                if (!reader.ConsumeNext()) {
                    throw new SyntaxTreeParsingException(reader.ViewLastValue, "A type was expected after the keyword params.");
                }

                _ = ExpectTypeReference(reader, out var elementType);

                if (!reader.ConsumeNext(TokenType.OpenSquareBracket) || !reader.ConsumeNext(TokenType.CloseSquareBracket)) {
                    throw new SyntaxTreeParsingException(reader.ViewLastValue, "The params parameter must be an one dimensional array. (e.g. params Unit[])");
                }

                typeReference = new ArrayType(elementType);
                return reader.UpperPosition;
            }

            _ = ExpectTypeReference(reader, out typeReference);
            return reader.UpperPosition;
        }

        public static bool TryRecognizeField(SpanReader<Token> reader, out int? newPosition, [MaybeNull] out Reference node)
        {
            bool isStatic;

            if (reader.ViewLastValue == TokenType.StaticKeyword) {
                isStatic = true;

                if (!reader.ConsumeNext()) {
                    throw new ArgumentException("A value type was expected.");
                }
            } else {
                isStatic = false;
            }

            if (reader.PeekNext(out var variableNameToken) && variableNameToken.Value.TokenType == TokenType.Name
                && reader.PeekNext(2, out var equalSignOrSemicolonToken)
                && equalSignOrSemicolonToken.Value.TryRecognize(TokenType.EqualSign, TokenType.Semicolon)) {
                _ = ExpectTypeReference(reader, out var typeReference);

                if (equalSignOrSemicolonToken.Value.TokenType == TokenType.Semicolon) {
                    newPosition = equalSignOrSemicolonToken.UpperReaderPosition;
                } else {
                    newPosition = reader.UpperPosition;
                }

                node = new FieldDefinition(variableNameToken.Value.GetValue<string>(), typeReference) {
                    IsStatic = isStatic
                };

                if (!isStatic) {
                    throw new NotSupportedException("Non-static field declarations are not supported.");
                }

                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeAssignment(SpanReader<Token> reader, TokenType valueType, out int? newPosition, [MaybeNull] out Reference node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.Name, out string name)
                && reader.ConsumeNext(TokenType.EqualSign)
                && reader.ConsumeNext(valueType, out var valueToken)) {
                if (!reader.ConsumeNext(TokenType.Semicolon)) {
                    throw new MissingTokenException(valueToken, TokenType.Semicolon);
                }

                var constant = GetConstant(valueToken);

                newPosition = reader.UpperPosition;
                node = new MemberAssignmenDefinition(new FieldReference(name), constant);
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        /// <summary>
        /// E.g. (Unit unit_id, int int_var)
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static bool TryRecognizeParameterList(
            SpanReader<Token> reader,
            TokenType openTokenType,
            TokenType closeTokenType,
            out List<ParameterDefinition> functionParameters,
            [NotNullWhen(true)] out int? newPosition,
            bool required = false,
            bool needConsume = false)
        {
            functionParameters = new List<ParameterDefinition>();

            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue.TryRecognize(openTokenType)) {
                while (reader.ConsumeNext(out Token variableTypeOrCloseToken)) {
                    var hasEndCloseType = variableTypeOrCloseToken.TokenType == closeTokenType;

                    if (hasEndCloseType) {
                        break;
                    }

                    reader.SetLengthTo(ExpectArgumentTypeReference(reader, out var typeReference));

                    if (!reader.ConsumeNext(TokenType.Name, out var parameterNameToken)) {
                        throw new SyntaxTreeParsingException(reader.ViewLastValue, "Parameter name is missing.");
                    }

                    var parameter = new ParameterDefinition(typeReference, name: parameterNameToken.GetValue<string>());
                    functionParameters.Add(parameter);

                    if (reader.PeekNext(TokenType.Comma)) {
                        reader.ConsumeNext();
                    }
                }

                newPosition = reader.UpperPosition;
                return true;
            } else if (required) {
                throw new SyntaxTreeParsingException(reader.ViewLastValue, "Parameter list was expected.");
            }

            newPosition = null;
            return false;
        }

        public static ConstantDefinition GetConstant(Token constantToken)
        {
            if (constantToken.TokenType == TokenType.Number) {
                var stringTypeReference = TypeReference.Integer;
                return new ConstantDefinition(stringTypeReference, constantToken.GetValue<int>());
            }

            if (constantToken.TokenType == TokenType.String) {
                var stringTypeReference = TypeReference.String;
                return new ConstantDefinition(stringTypeReference, constantToken.GetValue<string>());
            }

            if (constantToken.TokenType == TokenType.Name) {
                var typeReference = new TypeReference(constantToken.GetValue<string>());
                return new ConstantDefinition(typeReference, constantToken.Value);
            }

            if (constantToken.TokenType == TokenType.MemberAccess && constantToken is MemberAccessToken memberAccessToken) {
                var pathFragments = memberAccessToken.PathFragments;

                if (pathFragments is null) {
                    throw new ArgumentNullException(nameof(pathFragments));
                } else if (pathFragments.Count != 2) {
                    throw new ArgumentException("Enumeration field was expected (e.g. Unit.Player1)");
                }

                var enumFieldType = new TypeReference(pathFragments[0]);
                return new ConstantDefinition(enumFieldType, pathFragments[1]);
            }

            throw new SyntaxTreeParsingException(constantToken, "Bad constant.");
        }

        public static bool TryRecognizeArgumentList(
            SpanReader<Token> reader,
            TokenType openTokenType,
            TokenType endTokenType,
            out List<ConstantDefinition> arguments,
            [NotNullWhen(true)] out int? newPosition,
            bool required = false,
            bool needConsume = false)
        {
            arguments = new List<ConstantDefinition>();
            bool hadPreviousComma = false;

            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue == openTokenType) {
                do {
                    if (!reader.ConsumeNext(out ReaderPosition<Token> tokenPosition)) {
                        throw new SyntaxTreeParsingException(reader.ViewLastValue, "The argument list needs to be enclosed.");
                    }

                    if (!hadPreviousComma && tokenPosition.Value == endTokenType) {
                        newPosition = tokenPosition.UpperReaderPosition;
                        return true;
                    }

                    var constantNode = GetConstant(reader.ViewLastValue);
                    //reader.SetLengthTo(newPosition.Value);
                    arguments.Add(constantNode);

                    if (reader.PeekNext(out var peekedToken) && peekedToken.Value == TokenType.Comma) {
                        hadPreviousComma = true;

                        if (!reader.ConsumeNext()) {
                            throw new SyntaxTreeParsingException(reader.ViewLastValue, "Another value was expected.");
                        }
                    } else {
                        hadPreviousComma = false;
                    }
                } while (true);
            } else if (required) {
                throw new SyntaxTreeParsingException(reader.ViewLastValue, "A argument list was expected.");
            }

            newPosition = null;
            return false;
        }

        public static bool TryRecognizeAttribute(SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.OpenSquareBracket
                && reader.ConsumeNext(TokenType.Name, out var nameTokenPosition)) {
                List<ConstantDefinition>? arguments;

                if (reader.PeekNext(TokenType.OpenBracket)
                    && TryRecognizeArgumentList(reader, TokenType.OpenBracket, TokenType.CloseBracket, out arguments, out newPosition, required: true, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                } else {
                    arguments = null;
                }

                if (reader.ConsumeNext(TokenType.CloseSquareBracket)) {
                    newPosition = reader.UpperPosition;
                    var functionName = nameTokenPosition.GetValue<string>();
                    var methodCall = new MethodCallDefinition(functionName, null, arguments);
                    node = new AttributeDefinition(methodCall);
                    return true;
                } else {
                    throw new SyntaxTreeParsingException(reader.ViewLastValue, "The attribute needs to be closed by ']'.");
                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="newPosition"></param>
        /// <param name="node"></param>
        /// <param name="required"></param>
        /// <param name="needConsume">The next token gets consumed before the actual token gets evaluated.</param>
        /// <param name="hasDelimiter">The </param>
        /// <returns></returns>
        public static bool TryRecognizeMethodCall(
            SpanReader<Token> reader,
            [NotNullWhen(true)] out int? newPosition,
            [MaybeNullWhen(false)] out MethodCallDefinition node,
            bool required = false,
            bool needConsume = false,
            bool expectDelimiter = false)
        {
            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue.TryRecognize(TokenType.Name, out var nameToken)
                && reader.PeekNext(out ReaderPosition<Token> peekedToken)
                && peekedToken.Value.TryRecognize(TokenType.OpenAngleBracket, TokenType.OpenBracket)) {
                List<ConstantDefinition> genericArguments;

                if (peekedToken.Value == TokenType.OpenAngleBracket
                    && TryRecognizeArgumentList(
                        reader,
                        TokenType.OpenAngleBracket,
                        TokenType.CloseAngleBracket,
                        out genericArguments,
                        out newPosition,
                        needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                } else {
                    genericArguments = new List<ConstantDefinition>();
                }

                if (TryRecognizeArgumentList(
                        reader,
                        TokenType.OpenBracket,
                        TokenType.CloseBracket,
                        out var arguments,
                        out newPosition,
                        required: true,
                        needConsume: true)) {
                    // Is delimiter expected?
                    if (expectDelimiter) {
                        reader.SetLengthTo(newPosition.Value);
                        newPosition = ExpectToken(reader, TokenType.Semicolon, true);
                    }

                    var functionName = nameToken.GetValue<string>();

                    node = new MethodCallDefinition(
                        functionName,
                        genericArguments,
                        arguments);

                    return true;
                }
            } else if (required) {
                throw new SyntaxTreeParsingException(reader.ViewLastValue, "Function call was expected.");
            }

            newPosition = 0;
            node = null;
            return false;
        }

        public static bool TryRecognizeFunctionOrEventHandler(SpanReader<Token> reader, out int? newPosition, [MaybeNull] out Reference node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.FunctionKeyword, out var functionToken)) {
                if (!reader.ConsumeNext(TokenType.Name, out var functionNameToken)) {
                    throw new SyntaxTreeParsingException(functionToken, "Function must have a name.");
                }

                if (TryRecognizeParameterList(reader, TokenType.OpenAngleBracket, TokenType.CloseAngleBracket, out var genericParameters, out newPosition, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                }

                if (TryRecognizeParameterList(reader, TokenType.OpenBracket, TokenType.CloseBracket, out var parameters, out newPosition, required: true, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                }

                if (!reader.ConsumeNext(out Token afterSignatureNode) || !afterSignatureNode.TryRecognize(TokenType.OpenBrace, TokenType.WhenKeyword, TokenType.Semicolon)) {
                    throw new SyntaxTreeParsingException(reader.ViewLastValue, "Function must have a block (e.g. '{}'), followed by 'when' keyword or end with a semicolon after the signature.");
                }

                if (afterSignatureNode.TryRecognize(TokenType.OpenBrace, TokenType.Semicolon)) {
                    newPosition = reader.UpperPosition;
                    var isFunctionAbstract = afterSignatureNode.TokenType == TokenType.Semicolon;

                    node = new MethodDefinition(
                        functionNameToken.GetValue<string>(),
                        genericParameters,
                        parameters) {
                        IsAbstract = isFunctionAbstract
                    };

                    return true;
                } else {
                    var conditions = new List<MethodCallDefinition>();

                    do {
                        if (TryRecognizeMethodCall(reader, out newPosition, out var condition, required: true, needConsume: true)) {
                            reader.SetLengthTo(newPosition.Value);
                            conditions.Add(condition);
                        }

                        if (reader.ConsumeNext(TokenType.OpenBrace)) {
                            newPosition = reader.UpperPosition;

                            node = new EventHandlerDefinition(
                                functionNameToken.GetValue<string>(),
                                genericParameters,
                                parameters,
                                conditions);

                            return true;
                        }

                        if (!reader.ConsumeNext(TokenType.AndKeyword)) {
                            throw new SyntaxTreeParsingException(reader.ViewLastValue, "As long as the function block is not opening (e.g. '{') another condition is expected.");
                        }
                    } while (true);
                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeUsingStaticDirective(SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.UsingKeyword)
                && reader.ConsumeNext(TokenType.StaticKeyword)) {
                reader.SetLengthTo(ExpectTypeReference(reader, out var usingStaticDirectiveElementType, needConsume: true));
                newPosition = ExpectToken(reader, TokenType.Semicolon, needConsume: true);
                node = new UsingStaticDirective(usingStaticDirectiveElementType);
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeTemplateForExpression(SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.TemplateKeyword)) {
                var forInCollection = new List<ForInDefinition>();
                int? forInArgumentLength = null;

                while (reader.ConsumeNext(TokenType.ForKeyword)) {
                    if (TryRecognizeParameterList(reader, TokenType.OpenBracket, TokenType.CloseBracket, out var parameters, out newPosition, required: true, needConsume: true)) {
                        reader.SetLengthTo(newPosition.Value);
                    }

                    var parameter = parameters.SingleOrDefault();

                    if (parameter is null) {
                        throw new SyntaxTreeParsingException(reader.ViewLastPosition, "Only one parameter was expected. (e.g. '(Player PlayerId)')");
                    }

                    reader.SetLengthTo(ExpectToken(reader, TokenType.InKeyword, needConsume: true));

                    if (TryRecognizeArgumentList(reader, TokenType.OpenBracket, TokenType.CloseBracket, out var arguments, out newPosition, required: true, needConsume: true)) {
                        reader.SetLengthTo(newPosition.Value);
                    }

                    var forInDefinition = new ForInDefinition(parameter, arguments);
                    forInCollection.Add(forInDefinition);

                    if (forInArgumentLength == null) {
                        forInArgumentLength = forInDefinition.Arguments.Count;
                    } else if (forInArgumentLength != forInDefinition.Arguments.Count) {
                        throw new SyntaxTreeParsingException(reader.ViewLastValue, "Argument list count of current 'template for'-expression must be even across all for-in declarations.");
                    }
                }

                newPosition = reader.UpperPosition;
                node = new TemplateForInDefinition(forInCollection);
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static int? Recognize(BlockDefinition block, SpanReader<Token> reader, RecognizableReferences recognizableNodes, [MaybeNull] out Reference node)
        {
            int? newPosition;
            var module = block.Module;

            if (recognizableNodes.HasFlag(RecognizableReferences.Import)
                && TryRecognizeImport(module, reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.UsingStatic)
                && TryRecognizeUsingStaticDirective(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Typedef)
                && TryRecognizeTypeAliasReference(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Enumeration)
                && TryRecognizeEnumeration(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Attribute)
                && TryRecognizeAttribute(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.FunctionOrEventHandler)
                && TryRecognizeFunctionOrEventHandler(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Field)
                && TryRecognizeField(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Assignment)
                && TryRecognizeAssignment(reader, TokenType.Number, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.TemplateFor)
                && TryRecognizeTemplateForExpression(reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.MethodCall)
                && TryRecognizeMethodCall(reader, out newPosition, out var methodCall, expectDelimiter: true)) {
                node = methodCall;
                return newPosition;
            }

            node = null;
            return null;
        }
    }
}

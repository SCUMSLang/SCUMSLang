using SCUMSLang.Tokenization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SCUMSLang.AST
{
    public static class TreeParserTools
    {
        public static bool TryRecognizeImport(ModuleDefinition module, SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.ImportKeyword
                && reader.ConsumeNext(out Token stringToken)
                && reader.ConsumeNext(TokenType.Semicolon)) {
                var importBasePath = stringToken.GetValue<string>();
                var importAbsolutPath = Path.GetFullPath(importBasePath, module.FilePath);
                newPosition = reader.UpperPosition;
                node = new ImportDefinition(importAbsolutPath);
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
                        throw new ParseException(reader.ViewLastPosition, $"The list needs to be closed by '{TokenTypeLibrary.SequenceDictionary[closeToken]}'.");
                    }

                    newPosition = reader.UpperPosition;
                    return true;
                }
            }

            if (required) {
                throw new ParseException(reader.ViewLastPosition, "The enumeration needs to be openend by a brace ('{').");
            }

            newPosition = null;
            nameList = null;
            return false;
        }

        //public static bool TryRecognizeTypeAlias(BlockDefinition block, SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        //{
        //    if (reader.ViewLastValue == TokenType.TypeDefKeyword) {
        //        if (!reader.ConsumeNext(out ReaderPosition<Token> typeTokenPosition)) {
        //            throw new ParseException(typeTokenPosition, "A type was expected.");
        //        }

        //        static ReaderPosition<Token> expectNameAndEndToken(ref SpanReader<Token> reader, out int? newPosition)
        //        {
        //            if (!reader.ConsumeNext(out ReaderPosition<Token> nameToken)
        //                || string.IsNullOrEmpty(nameToken.Value?.ToString())) {
        //                throw new ParseException(reader.ViewLastPosition, "A name was expected.");
        //            }

        //            if (!reader.ConsumeNext(TokenType.Semicolon)) {
        //                throw new ParseException(reader.ViewLastPosition, "A semicolon was expected.");
        //            }

        //            newPosition = reader.UpperPosition;
        //            return nameToken;
        //        }

        //        //if (typeTokenPosition.Value == TokenType.EnumKeyword) {
        //        //    _ = TryRecognizeNameList(reader, TokenType.OpenBrace, TokenType.CloseBrace, out newPosition, out var nameList, required: true, needConsume: true);
        //        //    reader.SetLengthTo(newPosition!.Value);

        //        //    var nameToken = expectNameAndEndToken(ref reader, out newPosition!);
        //        //    var name = nameToken.Value.GetValue<string>();
        //        //    node = new EnumerationTypeReference(name, hasReservedNames: true, nameList!);

        //        //    return true;
        //        //}

        //        {
        //            var sourceTypeDefinition = block.GetTypeDefinition(new[] { typeTokenPosition.Value.GetValue<string>() }, TypeReferenceViewpoint.Type);

        //            var nameToken = expectNameAndEndToken(ref reader, out newPosition!);
        //            var name = nameToken.Value.GetValue<string>();
        //            node = new TypeAliasReference(name, sourceTypeDefinition);

        //            return true;
        //        }
        //    }

        //    newPosition = null;
        //    node = null;
        //    return false;
        //}

        public static bool TryRecognizeTypeAliasReference(ModuleDefinition module, SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.TypeDefKeyword) {
                if (!reader.ConsumeNext(out ReaderPosition<Token> typeTokenPosition)) {
                    throw new ParseException(typeTokenPosition, "A type was expected.");
                }

                static ReaderPosition<Token> expectNameAndEndToken(ref SpanReader<Token> reader, out int? newPosition)
                {
                    if (!reader.ConsumeNext(out ReaderPosition<Token> nameToken)
                        || string.IsNullOrEmpty(nameToken.Value?.ToString())) {
                        throw new ParseException(reader.ViewLastPosition, "A name was expected.");
                    }

                    if (!reader.ConsumeNext(TokenType.Semicolon)) {
                        throw new ParseException(reader.ViewLastPosition, "A semicolon was expected.");
                    }

                    newPosition = reader.UpperPosition;
                    return nameToken;
                }

                if (typeTokenPosition.Value == TokenType.EnumKeyword) {
                    _ = TryRecognizeNameList(reader, TokenType.OpenBrace, TokenType.CloseBrace, out newPosition, out var nameList, required: true, needConsume: true);
                    reader.SetLengthTo(newPosition!.Value);

                    var enumNameToken = expectNameAndEndToken(ref reader, out newPosition!);
                    var enumName = enumNameToken.Value.GetValue<string>();
                    node = TypeDefinition.CreateEnumDefinition(module, enumName, nameList!, usableAsConstants: true);
                    return true;
                }

                {
                    var sourceTypeName = typeTokenPosition.Value.GetValue<string>();
                    var aliasNameToken = expectNameAndEndToken(ref reader, out newPosition!);
                    var aliasName = aliasNameToken.Value.GetValue<string>();
                    node = TypeDefinition.CreateAliasDefinition(module, aliasName, new TypeReference(sourceTypeName, module));
                    return true;
                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeEnumeration(ModuleDefinition module, SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.EnumKeyword) {
                if (!reader.ConsumeNext(out ReaderPosition<Token> enumNameTokenPosition)
                    || enumNameTokenPosition.Value != TokenType.Name) {
                    throw new ParseException(reader.ViewLastPosition, "A name after the keyword 'enum' was expected.");
                }

                _ = TryRecognizeNameList(reader, TokenType.OpenBrace, TokenType.CloseBrace, out newPosition, out var nameList, required: true, needConsume: true);
                reader.SetLengthTo(newPosition!.Value);

                var enumName = enumNameTokenPosition.Value.GetValue<string>();
                var enumType = TypeDefinition.CreateEnumDefinition(module, enumName, nameList!);
                newPosition = reader.UpperPosition;
                node = enumType;
                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        //public static TypeDefinition GetTypeDefinition(BlockDefinition block, Token typeToken)
        //{
        //    if (!(typeToken.Value is string typeName) || string.IsNullOrEmpty(typeName)) {
        //        throw new ParseException(typeToken, "A type was expected.");
        //    }

        //    if (TokenTypeLibrary.SystemTypes.TryGetValue(typeToken.TokenType, out var definitionType)) {
        //        return block.GetTypeDefinition(definitionType);
        //    } else {
        //        return block.GetTypeDefinition(new[] { typeName }, TypeReferenceViewpoint.Type);
        //    }
        //}

        public static TypeReference GetTypeReference(ModuleDefinition module, Token nameTypeToken)
        {
            if (!(nameTypeToken.Value is string typeName) || string.IsNullOrEmpty(typeName)) {
                throw new ParseException(nameTypeToken, "A type was expected.");
            }

            return new TypeReference(typeName, module);
        }

        //public static TypeDefinition GetValueTypeDefinition(BlockDefinition block, Token valueTypeToken)
        //{
        //    if (valueTypeToken is MemberAccessToken memberAccessToken) {
        //        return block.GetTypeDefinition(memberAccessToken.PathFragments, TypeReferenceViewpoint.Value);
        //    }

        //    if (TokenTypeLibrary.SystemTypes.TryGetValue(valueTypeToken.TokenType, out var definitionType)) {
        //        return block.GetTypeDefinition(definitionType);
        //    }

        //    if (!(valueTypeToken.Value is string valueName) || string.IsNullOrEmpty(valueName)) {
        //        throw new ParseException(valueTypeToken, "A type was expected.");
        //    }

        //    return block.GetTypeDefinition(new[] { valueName }, TypeReferenceViewpoint.Value);
        //}

        public static bool TryRecognizeField(ModuleDefinition module, SpanReader<Token> reader, out int? newPosition, [MaybeNull] out Reference node)
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

            var fieldTypeToken = reader.ViewLastValue;

            if (reader.PeekNext(out var variableNameToken) && variableNameToken.Value.TokenType == TokenType.Name
                && reader.PeekNext(2, out var equalSignOrSemicolonToken)
                && equalSignOrSemicolonToken.Value.TryRecognize(TokenType.EqualSign, TokenType.Semicolon)) {
                var typeReference = GetTypeReference(module, fieldTypeToken);

                if (equalSignOrSemicolonToken.Value.TokenType == TokenType.Semicolon) {
                    newPosition = equalSignOrSemicolonToken.UpperReaderPosition;
                } else {
                    newPosition = reader.UpperPosition;
                }

                node = new FieldDefinition(variableNameToken.Value.GetValue<string>(), typeReference) {
                    IsStatic = isStatic
                };

                return true;
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeAssignment(ModuleDefinition module, SpanReader<Token> reader, TokenType valueType, out int? newPosition, [MaybeNull] out Reference node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.Name, out string name)
                && reader.ConsumeNext(TokenType.EqualSign)
                && reader.ConsumeNext(valueType, out var valueToken)) {
                if (!reader.ConsumeNext(TokenType.Semicolon)) {
                    throw new MissingTokenException(valueToken, TokenType.Semicolon);
                }

                //if (!block.TryGetFirstNode<DeclarationReference>(name, out var declaration)) {
                //    throw new ParseException(reader.ViewLastValue, $"There is no declaration by name '{name}' for assignment.");
                //}

                //module.CurrentBlock.TryGet

                var constant = GetConstant(module, valueToken);

                newPosition = reader.UpperPosition;
                node = new AssignDefinition(name, constant);
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
        public static bool TryRecognizeParameters(
            ModuleDefinition module,
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

                    var typeReference = GetTypeReference(module, variableTypeOrCloseToken);

                    if (!reader.ConsumeNext(TokenType.Name, out var parameterNameToken)) {
                        throw new ParseException(reader.ViewLastValue, "Parameter name is missing.");
                    }

                    var parameter = new ParameterDefinition(parameterNameToken.GetValue<string>(), typeReference);
                    functionParameters.Add(parameter);

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

        public static ConstantDefinition GetConstant(ModuleDefinition module, Token constantToken)
        {
            if (TokenTypeLibrary.SystemTypes.TryGetValue(constantToken.TokenType, out var definitionType)) {
                var typeReference = new TypeReference(SystemTypeLibrary.Sequences[definitionType], module);
                return new ConstantDefinition(typeReference, constantToken.Value);
            } else if (constantToken.TokenType == TokenType.MemberAccess && constantToken is MemberAccessToken memberAccessToken) {
                var pathFragments = memberAccessToken.PathFragments;

                if (pathFragments is null) {
                    throw new ArgumentNullException(nameof(pathFragments));
                } else if (pathFragments.Count != 2) {
                    throw new ArgumentException("Enumeration field was expected (e.g. Unit.Player1)");
                }

                var enumFieldType = new TypeReference(pathFragments[0], module);
                return new ConstantDefinition(enumFieldType, pathFragments[1]);
            } else if (constantToken.TokenType == TokenType.Name) {
                var typeReference = new TypeReference(constantToken.GetValue<string>(), module);
                return new ConstantDefinition(typeReference, constantToken.Value);
            } else {
                throw new ParseException(constantToken, "Bad constant.");
            }
        }

        public static bool TryRecognizeArgumentList(
            ModuleDefinition module,
            SpanReader<Token> reader,
            TokenType openTokenType,
            TokenType endTokenType,
            [NotNullWhen(true)] out int? newPosition,
            out List<ConstantDefinition> arguments,
            bool required = false,
            bool needConsume = false)
        {
            arguments = new List<ConstantDefinition>();
            bool hadPreviousComma = false;

            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue == openTokenType) {
                do {
                    if (!reader.ConsumeNext(out ReaderPosition<Token> tokenPosition)) {
                        throw new ParseException(reader.ViewLastValue, "The argument list needs to be enclosed.");
                    }

                    if (!hadPreviousComma && tokenPosition.Value == endTokenType) {
                        newPosition = tokenPosition.UpperReaderPosition;
                        return true;
                    }

                    var constantNode = GetConstant(module, reader.ViewLastValue);
                    //reader.SetLengthTo(newPosition.Value);
                    arguments.Add(constantNode);

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

        public static bool TryRecognizeAttribute(ModuleDefinition module, SpanReader<Token> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Reference node)
        {
            if (reader.ViewLastValue == TokenType.OpenSquareBracket
                && reader.ConsumeNext(TokenType.Name, out var nameTokenPosition)) {
                List<ConstantDefinition>? arguments;

                if (reader.PeekNext(TokenType.OpenBracket)
                    && TryRecognizeArgumentList(module, reader, TokenType.OpenBracket, TokenType.CloseBracket, out newPosition, out arguments, required: true, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                } else {
                    arguments = null;
                }

                if (reader.ConsumeNext(TokenType.CloseSquareBracket)) {
                    newPosition = reader.UpperPosition;
                    var functionName = nameTokenPosition.GetValue<string>();
                    var methodCall = new MethodCallDefinition(functionName, null, arguments, module.Block.CurrentBlock.DeclaringType);
                    node = new AttributeDefinition(methodCall);
                    return true;
                } else {
                    throw new ParseException(reader.ViewLastValue, "The attribute needs to be closed by ']'.");
                }
            }

            newPosition = null;
            node = null;
            return false;
        }

        public static bool TryRecognizeMethodCall(
            ModuleDefinition module,
            SpanReader<Token> reader,
            [NotNullWhen(true)] out int? newPosition,
            [MaybeNullWhen(false)] out MethodCallDefinition node,
            bool required = false,
            bool needConsume = false)
        {
            if (reader.ConsumeNext(needConsume)
                && reader.ViewLastValue.TryRecognize(TokenType.Name, out var nameToken)
                && reader.PeekNext(out ReaderPosition<Token> peekedToken)
                && peekedToken.Value.TryRecognize(TokenType.OpenAngleBracket, TokenType.OpenBracket)) {
                List<ConstantDefinition> genericArguments;

                if (peekedToken.Value == TokenType.OpenAngleBracket
                    && TryRecognizeArgumentList(
                        module,
                        reader,
                        TokenType.OpenAngleBracket,
                        TokenType.CloseAngleBracket,
                        out newPosition,
                        out genericArguments,
                        needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                } else {
                    genericArguments = new List<ConstantDefinition>();
                }

                if (TryRecognizeArgumentList(
                        module,
                        reader,
                        TokenType.OpenBracket,
                        TokenType.CloseBracket,
                        out newPosition,
                        out var arguments,
                        required: true,
                        needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);

                    var functionName = nameToken.GetValue<string>();

                    node = new MethodCallDefinition(
                        functionName,
                        genericArguments,
                        arguments,
                        module.Block.CurrentBlock.DeclaringType);

                    return true;
                }
            } else if (required) {
                throw new ParseException(reader.ViewLastValue, "Function call was expected.");
            }

            newPosition = 0;
            node = null;
            return false;
        }

        public static bool TryRecognizeFunctionOrEventHandler(ModuleDefinition module, SpanReader<Token> reader, out int? newPosition, [MaybeNull] out Reference node)
        {
            if (reader.ViewLastValue.TryRecognize(TokenType.FunctionKeyword, out var functionToken)) {
                if (!reader.ConsumeNext(TokenType.Name, out var functionNameToken)) {
                    throw new ParseException(functionToken, "Function must have a name.");
                }

                if (TryRecognizeParameters(module, reader, TokenType.OpenAngleBracket, TokenType.CloseAngleBracket, out var genericParameters, out newPosition, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                }

                if (TryRecognizeParameters(module, reader, TokenType.OpenBracket, TokenType.CloseBracket, out var parameters, out newPosition, required: true, needConsume: true)) {
                    reader.SetLengthTo(newPosition.Value);
                }

                if (!reader.ConsumeNext(out Token afterSignatureNode) || !afterSignatureNode.TryRecognize(TokenType.OpenBrace, TokenType.WhenKeyword, TokenType.Semicolon)) {
                    throw new ParseException(reader.ViewLastValue, "Function must have a block (e.g. '{}'), followed by 'when' keyword or end with a semicolon after the signature.");
                }

                if (afterSignatureNode.TryRecognize(TokenType.OpenBrace, TokenType.Semicolon)) {
                    newPosition = reader.UpperPosition;
                    var isFunctionAbstract = afterSignatureNode.TokenType == TokenType.Semicolon;

                    node = new MethodDefinition(
                        functionNameToken.GetValue<string>(),
                        genericParameters,
                        parameters,
                        module.Block.CurrentBlock.DeclaringType) {
                        IsAbstract = isFunctionAbstract
                    };

                    return true;
                } else {
                    var conditions = new List<MethodCallDefinition>();

                    do {
                        if (TryRecognizeMethodCall(module, reader, out newPosition, out var condition, required: true, needConsume: true)) {
                            reader.SetLengthTo(newPosition.Value);
                            conditions.Add(condition);
                        }

                        if (reader.ConsumeNext(TokenType.OpenBrace)) {
                            newPosition = reader.UpperPosition;

                            node = new EventHandlerDefinition(
                                functionNameToken.GetValue<string>(),
                                genericParameters,
                                parameters,
                                conditions,
                                module.Block.CurrentBlock.DeclaringType);

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

        public static int? TryRecognize(BlockDefinition block, SpanReader<Token> reader, RecognizableReferences recognizableNodes, [MaybeNull] out Reference node)
        {
            int? newPosition;
            var module = block.Module;

            if (recognizableNodes.HasFlag(RecognizableReferences.Import)
                && TryRecognizeImport(module, reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.TypeAlias)
                && TryRecognizeTypeAliasReference(module, reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Enumeration)
                && TryRecognizeEnumeration(module, reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Attribute)
                && TryRecognizeAttribute(module, reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.FunctionOrEventHandler)
                && TryRecognizeFunctionOrEventHandler(module, reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Declaration)
                && TryRecognizeField(module, reader, out newPosition, out node)) {
                return newPosition;
            }

            if (recognizableNodes.HasFlag(RecognizableReferences.Assignment)
                && TryRecognizeAssignment(module, reader, TokenType.Integer, out newPosition, out node)) {
                return newPosition;
            }

            node = null;
            return null;
        }
    }
}

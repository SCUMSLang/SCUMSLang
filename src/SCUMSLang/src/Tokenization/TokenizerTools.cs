using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static SCUMSLang.Tokenization.TokenTools;

namespace SCUMSLang.Tokenization
{
    internal static class TokenizerTools
    {
        public static bool TryRecognizeWhiteSpaces(SpanReader<char> reader, out int? newPosition)
        {
            if (char.IsWhiteSpace(reader.ViewLastValue)) {
                while (reader.PeekNext(out var peekedPosition) && char.IsWhiteSpace(peekedPosition.Value)) {
                    if (reader.ConsumeNext()) {
                        continue;
                    }
                }

                newPosition = reader.UpperPosition;
                return true;
            }

            newPosition = null;
            return false;
        }

        private static bool TryRecognizeComment(SpanReader<char> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Token token)
        {
            if (reader.ViewLastValue == '/') {
                TokenType tokenType;

                if (!reader.ConsumeNext(out ReaderPosition<char> consumedCharacter) && consumedCharacter.Value == '/') {
                    throw new TokenizationException(reader.ViewLastPosition, "A comment was expected.");
                }

                if (reader.PeekNext(out consumedCharacter) && consumedCharacter.Value == '/'
                    && (!reader.PeekNext(2, out var peekedSecondCharacter) || peekedSecondCharacter.Value != '/')) {
                    reader.SetLengthTo(consumedCharacter.UpperReaderPosition);
                    tokenType = TokenType.XmlComment;
                } else {
                    tokenType = TokenType.Comment;
                    reader.ConsumeUntilNot('/', EqualityComparer<char>.Default);
                }

                if (reader.ConsumeNext()) {
                    reader.SetPositionTo(reader.UpperPosition);
                    reader.ConsumeUntil((ref ReaderPosition<char> character) => character.Value == '\r' || character.Value == 'n');
                    token = new Token(tokenType, reader.ReadPosition, reader.ViewReadLength, reader.View.ToString().Trim(), Channel.Comments);
                } else {
                    token = new Token(tokenType, reader.UpperPosition, 0, string.Empty, Channel.Comments);
                }

                newPosition = reader.UpperPosition;
                return true;
            }

            newPosition = reader.UpperPosition;
            token = null;
            return false;
        }

        public static bool TryRecognizeName(
            SpanReader<char> reader,
            [NotNullWhen(true)] out int? newPosition,
            [MaybeNullWhen(false)] out string name,
            bool required = false)
        {
            if (reader.ViewReadLength > 0
                || reader.ConsumeNext()) {
                var characters = reader.View;

                static bool isBeginningToken(char letter) =>
                        char.IsLetter(letter) || letter == '_';

                if (isBeginningToken(characters[0])) {
                    static bool isValidCharacter(char letter) =>
                        isBeginningToken(letter) || char.IsDigit(letter);

                    while (reader.PeekNext(out var peekedPosition) && isValidCharacter(peekedPosition.View.Last())) {
                        reader.ConsumeNext(out characters);
                        continue;
                    }

                    newPosition = reader.UpperPosition;
                    name = characters.ToString();
                    return true;
                }
            } else if (required) {
                throw new ParseException(reader.UpperPosition, 0, "A name was expected.");
            }

            newPosition = null;
            name = null;
            return false;
        }

        private static bool TryRecognizeMemberAccess(SpanReader<char> reader, [NotNullWhen(true)] out int? newPosition, [MaybeNullWhen(false)] out Token token)
        {
            var pathFragments = new List<string>();

            do {
                if (!TryRecognizeName(reader, out newPosition, out var pathFragment)) {

                    if (pathFragments.Count >= 1) {
                        throw new TokenizationException(reader.UpperPosition, "After a dot another path fragment was expected.");
                    }

                    break;
                }

                reader.SetLengthTo(newPosition.Value);
                pathFragments.Add(pathFragment);
            } while (reader.PeekNext('.', EqualityComparer<char>.Default)
                && reader.SetPositionTo(reader.UpperPosition + 2));

            if (pathFragments.Count > 1) {
                newPosition = reader.UpperPosition;
                token = new MemberAccessToken(reader.ReadPosition, reader.ViewReadLength, pathFragments);
                return true;
            } else if (pathFragments.Count == 1) {
                var pathFragment = pathFragments[0];
                token = new Token(TokenType.Name, reader.ReadPosition, pathFragment.Length, pathFragment);
                return false;
            }

            newPosition = null;
            token = null;
            return false;
        }

        public static bool RecognizeKeywordOrName(Token nameToken, [MaybeNullWhen(false)] out Token token)
        {
            var name = nameToken.GetValue<string>();
            token = null;

            foreach (var (TokenType, Keyword) in TokenTypeLibrary.TokenAscendedKeywords) {
                if (name == Keyword) {
                    token = new Token(TokenType, nameToken.Position, nameToken.Length, nameToken.Value);
                    break;
                }
            }

            if (token is null) {
                token = nameToken;
            }

            return true;
        }

        public static bool TryRecognizeInteger(SpanReader<char> reader, out int? newPosition, [MaybeNull] out Token token)
        {
            var characters = reader.View;

            if (char.IsDigit(characters[0])) {
                bool isHexadecimal = false;

                if (characters[0] == '0' && reader.PeekNext(out var peedkPosition) && peedkPosition.View[1] == 'x') {
                    reader.ConsumeNext(out characters);
                    isHexadecimal = true;
                }

                if (isHexadecimal) {
                    while (reader.PeekNext(out var peekedPosition)) {
                        char lastCharacter = peekedPosition.Value;

                        if (char.IsDigit(lastCharacter)) {
                            reader.ConsumeNext(out characters);
                            continue;
                        } else if (char.IsLetter(lastCharacter)) {
                            reader.ConsumeNext(out characters);
                            lastCharacter = char.ToUpper(lastCharacter);

                            if (lastCharacter >= 'A' && lastCharacter <= 'F') {
                                continue;
                            } else {
                                throw new TokenizationException(reader.UpperPosition, "Token of type Integer can only have valid hexadecimal literals: [A-Fa-f1-9].");
                            }
                        } else {
                            break;
                        }
                    }
                } else {
                    while (reader.PeekNext(out var peekedPosition) && char.IsDigit(peekedPosition.Value)) {
                        reader.ConsumeNext(out characters);
                        continue;
                    }
                }

                var integerChars = characters.Slice(0, characters.Length);
                int integer;

                if (isHexadecimal) {
                    integer = Convert.ToInt32(integerChars.ToString(), 16);
                } else {
                    integer = int.Parse(integerChars);
                }

                newPosition = reader.UpperPosition;
                token = CreateToken(TokenType.Integer, integer, reader);
                return true;
            }

            newPosition = null;
            token = null;
            return false;
        }

        public static bool TryRecognizeSingleCharacterToken(
                SpanReader<char> reader,
                char character,
                TokenType tokenType,
                out Token? token)
        {
            var characters = reader.View;

            if (characters[0] == character) {
                token = CreateToken(tokenType, reader);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryRecognizeSequentialCharactersToken(
            SpanReader<char> reader,
            TokenType tokenType,
            out int? newPosition,
            out Token? token,
            params char[] sequentialCharacters)
        {
            var characters = reader.View;
            int stage = 0;

            while (sequentialCharacters.Length <= characters.Length) {
                if (characters[stage] != sequentialCharacters[stage]) {
                    newPosition = null;
                    token = null;
                    return false;
                }

                stage++;
            }

            newPosition = reader.ReadPosition + stage;
            token = CreateToken(tokenType, reader);
            return true;
        }

        public static bool TryRecognizeSequentialCharactersToken(
            SpanReader<char> reader,
            string sequentialCharacters,
            TokenType tokenType,
            out int? charactersRead,
            out Token? token) =>
            TryRecognizeSequentialCharactersToken(reader, tokenType, out charactersRead, out token, sequentialCharacters.ToCharArray());

        public static bool TryRecognizeStringToken(SpanReader<char> reader, out int? newPosition, [MaybeNull] out Token token)
        {
            var characters = reader.View;

            if (characters[0] == '"') {
                do {
                    if (!reader.ConsumeNext(out characters)) {
                        throw new TokenizationException(reader.UpperPosition, "Token of type String was expected to be closed with another \".");
                    }
                } while (characters.Last() != '"');

                token = CreateToken(TokenType.String, characters.Slice(1, characters.Length - 2).ToString(), reader);
                newPosition = reader.UpperPosition;
                return true;
            }

            newPosition = 0;
            token = null;
            return false;
        }

        public static int? RecognizeToken(SpanReader<char> reader, [MaybeNull] out Token token)
        {
            int? newPosition;

            if (TryRecognizeWhiteSpaces(reader, out newPosition)) {
                token = null;
                return newPosition;
            }

            if (TryRecognizeComment(reader, out newPosition, out token)) {
                return newPosition;
            }

            if (TryRecognizeStringToken(reader, out newPosition, out token)) {
                return newPosition;
            }

            if (TryRecognizeMemberAccess(reader, out newPosition, out token)) {
                return newPosition;
            }

            if (!(token is null) && RecognizeKeywordOrName(token, out token)) {
                return token.UpperPosition;
            }

            if (TryRecognizeInteger(reader, out newPosition, out token)) {
                return newPosition;
            }

            var charactersLength = reader.View.Length;

            if (charactersLength == 1
                && (TryRecognizeSingleCharacterToken(reader, '(', TokenType.OpenBracket, out token)
                    || TryRecognizeSingleCharacterToken(reader, ')', TokenType.CloseBracket, out token)
                    || TryRecognizeSingleCharacterToken(reader, '{', TokenType.OpenBrace, out token)
                    || TryRecognizeSingleCharacterToken(reader, '}', TokenType.CloseBrace, out token)
                    || TryRecognizeSingleCharacterToken(reader, '[', TokenType.OpenSquareBracket, out token)
                    || TryRecognizeSingleCharacterToken(reader, ']', TokenType.CloseSquareBracket, out token)
                    || TryRecognizeSingleCharacterToken(reader, ',', TokenType.Comma, out token)
                    || TryRecognizeSingleCharacterToken(reader, ';', TokenType.Semicolon, out token))) {
                return reader.UpperPosition;
            }

            if (charactersLength == 2
                && (TryRecognizeSequentialCharactersToken(reader, "==", TokenType.EqualOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "<=", TokenType.LessThanOrEqual, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, ">=", TokenType.GreaterThanOrEqual, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "&&", TokenType.AndLogicalOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "||", TokenType.OrLogicalOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "++", TokenType.IncrementOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "--", TokenType.DecrementOperator, out newPosition, out token))) {
                return newPosition;
            }

            if (TryRecognizeSingleCharacterToken(reader, '=', TokenType.EqualSign, out token)
                || TryRecognizeSingleCharacterToken(reader, '<', TokenType.OpenAngleBracket, out token)
                || TryRecognizeSingleCharacterToken(reader, '>', TokenType.CloseAngleBracket, out token)
                || TryRecognizeSingleCharacterToken(reader, '+', TokenType.AdditionOperator, out token)
                || TryRecognizeSingleCharacterToken(reader, '-', TokenType.SubtractionOperator, out token)) {
                return reader.UpperPosition;
            }

            token = null;
            return null;
        }

        //public static List<Token> FilterTokens(IEnumerable<Token> tokens, Channel[]? channels = null)
        //{
        //    var filteredTokens = new List<Token>();

        //    foreach (var token in tokens) {
        //        if (channels is null || channels.Contains(token.Channel)) {
        //            filteredTokens.Add(token);
        //        }
        //    }

        //    return filteredTokens;
        //}
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using static SCUMSLang.Tokenization.TokenTools;

namespace SCUMSLang.Tokenization
{
    internal static class TokenizerTools
    {
        public static bool TryRecognizeWhiteSpaces(Reader<char> reader, out int? newPosition)
        {
            if (char.IsWhiteSpace(reader.ViewLastValue))
            {
                while (reader.PeekNext(out var peekedPosition) && char.IsWhiteSpace(peekedPosition.Value))
                {
                    if (reader.ConsumeNext())
                    {
                        continue;
                    }
                }

                newPosition = reader.UpperPosition;
                return true;
            }

            newPosition = null;
            return false;
        }

        //public static bool TryRecognizeNewLine(Reader<char> reader, out int charactersRead)
        //{
        //    var characters = reader.View;

        //    if (characters[0] == '\\')
        //    {
        //        if (characters.Length == 1)
        //        {
        //            charactersRead = 0;
        //            return true;
        //        }

        //        if (characters[1] == 'r' || characters[1] == 'n')
        //        {
        //            charactersRead = 2;
        //            return true;
        //        }
        //        else
        //        {
        //            throw new TokenizationException(reader.UpperPosition, "Illegal character found beginning with '\\'. Only white-spaces (e.g. \\r or \\n) are allowed.");
        //        }
        //    }

        //    charactersRead = 0;
        //    return false;
        //}

        public static bool TryRecognizeKeywordOrName(Reader<char> reader, out int? newPosition, [MaybeNull] out Token token)
        {
            var characters = reader.View;
            token = null;

            if (char.IsLetter(characters[0]))
            {
                static bool isLetter(char letter) =>
                    char.IsLetter(letter) || letter == '_';

                while (reader.PeekNext(out var peekedPosition) && isLetter(peekedPosition.View.Last()))
                {
                    reader.ConsumeNext(out characters);
                    continue;
                }

                string name = characters.ToString();

                foreach (var (TokenType, Keyword) in TokenTypeLibrary.AscendKeywordedTokenTypes)
                {
                    if (name == Keyword)
                    {
                        token = CreateToken(TokenType, reader);
                        break;
                    }
                }

                if (token is null)
                {
                    token = CreateToken(TokenType.Name, name.ToString(), reader);
                    newPosition = reader.UpperPosition;
                    return true;
                }

                newPosition = reader.UpperPosition;
                return true;
            }

            newPosition = null;
            return false;
        }

        public static bool TryRecognizeInteger(Reader<char> reader, out int? newPosition, [MaybeNull] out Token token)
        {
            var characters = reader.View;

            if (char.IsDigit(characters[0]))
            {
                bool isHexadecimal = false;

                if (characters[0] == '0' && reader.PeekNext(out var peedkPosition) && peedkPosition.View[1] == 'x')
                {
                    reader.ConsumeNext(out characters);
                    isHexadecimal = true;
                }

                if (isHexadecimal)
                {
                    while (reader.PeekNext(out var peekedPosition))
                    {
                        char lastCharacter = peekedPosition.Value;

                        if (char.IsDigit(lastCharacter))
                        {
                            reader.ConsumeNext(out characters);
                            continue;
                        }
                        else if (char.IsLetter(lastCharacter))
                        {
                            reader.ConsumeNext(out characters);
                            lastCharacter = char.ToUpper(lastCharacter);

                            if (lastCharacter >= 'A' && lastCharacter <= 'F')
                            {
                                continue;
                            }
                            else
                            {
                                throw new TokenizationException(reader.UpperPosition, "Token of type Integer can only have valid hexadecimal literals: [A-Fa-f1-9].");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    while (reader.PeekNext(out var peekedPosition) && char.IsDigit(peekedPosition.Value))
                    {
                        reader.ConsumeNext(out characters);
                        continue;
                    }
                }

                var integerChars = characters.Slice(0, characters.Length);
                int integer;

                if (isHexadecimal)
                {
                    integer = Convert.ToInt32(integerChars.ToString(), 16);
                }
                else
                {
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
                Reader<char> reader,
                char character,
                TokenType tokenType,
                out Token? token)
        {
            var characters = reader.View;

            if (characters[0] == character)
            {
                token = CreateToken(tokenType, reader);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryRecognizeSequentialCharactersToken(
            Reader<char> reader,
            TokenType tokenType,
            out int? newPosition,
            out Token? token,
            params char[] sequentialCharacters)
        {
            var characters = reader.View;
            int stage = 0;

            while (sequentialCharacters.Length <= characters.Length)
            {
                if (characters[stage] != sequentialCharacters[stage])
                {
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
            Reader<char> reader,
            string sequentialCharacters,
            TokenType tokenType,
            out int? charactersRead,
            out Token? token) =>
            TryRecognizeSequentialCharactersToken(reader, tokenType, out charactersRead, out token, sequentialCharacters.ToCharArray());

        public static bool TryRecognizeStringToken(Reader<char> reader, out int? newPosition, [MaybeNull] out Token token)
        {
            var characters = reader.View;

            if (characters[0] == '"')
            {
                do
                {
                    if (!reader.ConsumeNext(out characters))
                    {
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

        public static int? RecognizeToken(Reader<char> reader, [MaybeNull] out Token token)
        {
            int? newPosition;

            if (TryRecognizeWhiteSpaces(reader, out newPosition))
            {
                token = null;
                return newPosition;
            }

            if (TryRecognizeStringToken(reader, out newPosition, out token))
            {
                return newPosition;
            }

            if (TryRecognizeKeywordOrName(reader, out newPosition, out token))
            {
                return newPosition;
            }

            if (TryRecognizeInteger(reader, out newPosition, out token))
            {
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
                    || TryRecognizeSingleCharacterToken(reader, ';', TokenType.Semicolon, out token)))
            {
                return reader.UpperPosition;
            }

            if (charactersLength == 2
                && (TryRecognizeSequentialCharactersToken(reader, "==", TokenType.EqualOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "<=", TokenType.LessThanOrEqual, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, ">=", TokenType.GreaterThanOrEqual, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "&&", TokenType.AndLogicalOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "||", TokenType.OrLogicalOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "++", TokenType.IncrementOperator, out newPosition, out token)
                    || TryRecognizeSequentialCharactersToken(reader, "--", TokenType.DecrementOperator, out newPosition, out token)))
            {
                return newPosition;
            }

            if (TryRecognizeSingleCharacterToken(reader, '=', TokenType.EqualSign, out token)
                || TryRecognizeSingleCharacterToken(reader, '<', TokenType.OpenAngleBracket, out token)
                || TryRecognizeSingleCharacterToken(reader, '>', TokenType.CloseAngleBracket, out token)
                || TryRecognizeSingleCharacterToken(reader, '+', TokenType.AdditionOperator, out token)
                || TryRecognizeSingleCharacterToken(reader, '-', TokenType.SubtractionOperator, out token))
            {
                return reader.UpperPosition;
            }

            token = null;
            return reader.View.Length;
        }
    }
}

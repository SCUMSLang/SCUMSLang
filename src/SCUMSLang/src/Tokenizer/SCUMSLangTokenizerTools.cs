using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SCUMSLang.Tokenizer
{
    internal static class SCUMSLangTokenizerTools
    {
        public static bool TryRecognizeWhiteSpaces(ReadOnlySpan<char> characters, out int charactersRead)
        {
            var whitespacesLength = 0;

            while (whitespacesLength < characters.Length)
            {
                if (!char.IsWhiteSpace(characters[whitespacesLength]))
                {
                    break;
                }

                whitespacesLength++;
            }

            if (whitespacesLength > 0)
            {
                charactersRead = whitespacesLength;
                return true;
            }

            charactersRead = 0;
            return false;
        }

        public static bool TryRecognizeNewLine(ReadOnlySpan<char> characters, out int charactersRead)
        {
            if (characters[0] == '\\')
            {
                if (characters.Length == 1)
                {
                    charactersRead = 0;
                    return true;
                }

                if (characters[1] == 'r' || characters[1] == 'n')
                {
                    charactersRead = 2;
                    return true;
                }
                else
                {
                    throw new TokenizerException("Illegal character found beginning with '\\'. Only white-spaces (e.g. \\r or \\n) are allowed.");
                }
            }

            charactersRead = 0;
            return false;
        }

        public static bool TryRecognizeKeywordOrName(ContentReader reader, ref ReadOnlySpan<char> characters, out int charactersRead, [MaybeNull] out Token token)
        {
            token = null;

            if (char.IsLetter(characters[0]))
            {
                static bool isLetter(char letter) =>
                    char.IsLetter(letter) || letter == '_';

                while (reader.MoveNext(out characters) && isLetter(characters[characters.Length - 1]))
                {
                    continue;
                }

                if (characters.Length != 1)
                {
                    reader.Rewind(1, out characters);
                }

                string name = characters.ToString();

                if (name == "if")
                {
                    token = new Token(TokenType.IfKeyword);
                }

                if (name == "and")
                {
                    token = new Token(TokenType.AndKeyword);
                }

                if (name == "for")
                {
                    token = new Token(TokenType.ForKeyword);
                }

                if (name == "when")
                {
                    token = new Token(TokenType.WhenKeyword);
                }

                if (name == "else")
                {
                    token = new Token(TokenType.ElseKeyword);
                }

                if (name == "true" || name == "false")
                {
                    token = new Token(TokenType.Boolean, bool.Parse(name));
                }

                if (name == "static")
                {
                    token = new Token(TokenType.StaticKeyword);
                }

                if (name == "function")
                {
                    token = new Token(TokenType.FunctionKeyword);
                }

                if (name == "template")
                {
                    token = new Token(TokenType.TemplateKeyword);
                }

                if (name == "ordered")
                {
                    token = new Token(TokenType.OrderedKeyword);
                }

                if (token is null)
                {
                    token = new Token(TokenType.Name, name.ToString());
                    charactersRead = name.Length;
                    return true;
                }

                charactersRead = name.Length;
                return true;
            }

            charactersRead = 0;
            return false;
        }

        public static bool TryRecognizeInteger(ContentReader reader, ref ReadOnlySpan<char> characters, out int charactersRead, [MaybeNull] out Token token)
        {
            if (char.IsDigit(characters[0]))
            {
                bool isHexadecimal = false;

                if (characters[0] == '0' && reader.MoveNext(out characters) && characters[1] == 'x')
                {
                    isHexadecimal = true;
                }

                if (isHexadecimal)
                {
                    while (reader.MoveNext(out characters))
                    {
                        char lastCharacter = characters[characters.Length];

                        if (char.IsDigit(lastCharacter))
                        {
                            continue;
                        }
                        else if (char.IsLetter(lastCharacter))
                        {
                            lastCharacter = char.ToUpper(lastCharacter);

                            if (lastCharacter >= 'A' && lastCharacter <= 'F')
                            {
                                continue;
                            }
                            else
                            {
                                throw new TokenizerException("Token of type Integer can only have valid hexadecimal literals: [A-Fa-f1-9].");
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
                    while (reader.MoveNext(out characters) && char.IsDigit(characters[characters.Length - 1]))
                    {
                        continue;
                    }
                }

                if (characters.Length != 1)
                {
                    reader.Rewind(1, out characters);
                }

                var integerChars = characters.Slice(0, characters.Length);
                int integer;

                if (isHexadecimal)
                {
                    integer = int.Parse(integerChars, NumberStyles.HexNumber);
                }
                else
                {
                    integer = int.Parse(integerChars);
                }

                charactersRead = integerChars.Length;
                token = new Token(TokenType.Integer, integer);
                return true;
            }

            charactersRead = 0;
            token = null;
            return false;
        }

        public static bool TryRecognizeSingleCharacterToken(
                ReadOnlySpan<char> characters,
                char character,
                TokenType tokenType,
                out Token? token)
        {
            if (characters[0] == character)
            {
                token = new Token(tokenType);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryRecognizeSequentialCharactersToken(
            ReadOnlySpan<char> characters,
            TokenType tokenType,
            out int charactersRead,
            out Token? token,
            params char[] sequentialCharacters)
        {
            int stage = 0;

            while (sequentialCharacters.Length <= characters.Length)
            {
                if (characters[stage] != sequentialCharacters[stage])
                {
                    charactersRead = 0;
                    token = null;
                    return false;
                }

                stage++;
            }

            charactersRead = sequentialCharacters.Length;
            token = new Token(tokenType);
            return true;
        }

        public static bool TryRecognizeStringToken(ContentReader reader, ref ReadOnlySpan<char> characters, out int charactersRead, [MaybeNull] out Token token)
        {
            if (characters[0] == '"')
            {
                do
                {
                    if (!reader.MoveNext(out characters))
                    {
                        throw new TokenizerException("Token of type String was expected to be closed with another \".");
                    }
                } while (characters[characters.Length - 1] != '"');

                token = new Token(TokenType.String, characters.Slice(1, characters.Length - 2).ToString());
                charactersRead = characters.Length;
                return true;
            }

            charactersRead = 0;
            token = null;
            return false;
        }

        public static int RecognizeToken(ContentReader reader, [MaybeNull] out Token token)
        {
            var characters = reader.ReadCharacters();
            int charactersRead;

            if (TryRecognizeWhiteSpaces(characters, out charactersRead))
            {
                token = null;
                return charactersRead;
            }

            if (TryRecognizeStringToken(reader, ref characters, out charactersRead, out token))
            {
                return charactersRead;
            }

            if (TryRecognizeKeywordOrName(reader, ref characters, out charactersRead, out token))
            {
                return charactersRead;
            }

            if (TryRecognizeInteger(reader, ref characters, out charactersRead, out token))
            {
                return charactersRead;
            }

            var charactersLength = characters.Length;

            if (charactersLength == 1
                && (TryRecognizeSingleCharacterToken(characters, '(', TokenType.OpenBracket, out token)
                    || TryRecognizeSingleCharacterToken(characters, ')', TokenType.CloseBracket, out token)
                    || TryRecognizeSingleCharacterToken(characters, '{', TokenType.OpenBrace, out token)
                    || TryRecognizeSingleCharacterToken(characters, '}', TokenType.CloseBrace, out token)
                    || TryRecognizeSingleCharacterToken(characters, '[', TokenType.OpenSquareBracket, out token)
                    || TryRecognizeSingleCharacterToken(characters, ']', TokenType.CloseSquareBracket, out token)
                    || TryRecognizeSingleCharacterToken(characters, ',', TokenType.Comma, out token)
                    || TryRecognizeSingleCharacterToken(characters, ';', TokenType.Semicolon, out token)))
            {
                return 1;
            }

            if (charactersLength == 2
                && (TryRecognizeSequentialCharactersToken(characters, TokenType.EqualOperator, out charactersRead, out token, "==".ToCharArray())
                    || TryRecognizeSequentialCharactersToken(characters, TokenType.LessThanOrEqual, out charactersRead, out token, "<=".ToCharArray())
                    || TryRecognizeSequentialCharactersToken(characters, TokenType.GreaterThanOrEqual, out charactersRead, out token, ">=".ToCharArray())
                    || TryRecognizeSequentialCharactersToken(characters, TokenType.AndLogicalOperator, out charactersRead, out token, "&&".ToCharArray())
                    || TryRecognizeSequentialCharactersToken(characters, TokenType.OrLogicalOperator, out charactersRead, out token, "||".ToCharArray())))
            {
                return charactersRead;
            }

            if (TryRecognizeSingleCharacterToken(characters, '=', TokenType.EqualSign, out token)
                || TryRecognizeSingleCharacterToken(characters, '<', TokenType.OpenAngleBracket, out token)
                || TryRecognizeSingleCharacterToken(characters, '>', TokenType.CloseAngleBracket, out token))
            {
                return 1;
            }

            token = null;
            return characters.Length;
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SCRUML.Tokenizer
{
    public class SCRUMLTokenizer
    {
        public string FilePath { get; }

        public SCRUMLTokenizer(string filePath)
        {
            FilePath = filePath;
        }

        private int tryRecognizeToken(ReadOnlySpan<char> characters, [MaybeNull] out Token token)
        {
            var charactersLength = characters.Length;

            {
                var whitespaces = 0;

                while (whitespaces < charactersLength)
                {
                    if (!char.IsWhiteSpace(characters[whitespaces]))
                    {
                        break;
                    }

                    whitespaces++;
                }

                if (whitespaces > 0)
                {
                    token = null;
                    return whitespaces;
                }
            }

            if (characters[0] == '\\')
            {
                if (charactersLength == 1)
                {
                    token = null;
                    return 0;
                }

                if (characters[1] == 'r' || characters[1] == 'n')
                {
                    token = null;
                    return 2;
                }
                else
                {
                    throw new TokenizerException();
                }
            }

            var charactersLastIndex = charactersLength - 1;

            if (char.IsLetter(characters[0]))
            {
                token = null;

                if (char.IsLetter(characters[charactersLastIndex]))
                {
                    return 0;
                }

                var chars = characters.Slice(0, charactersLastIndex);

                if (chars == "static")
                {
                    token = new Token(TokenType.StaticKeyword);
                }

                if (chars == "function")
                {
                    token = new Token(TokenType.FunctionKeyword);
                }

                if (chars == "when")
                {
                    token = new Token(TokenType.FunctionKeyword);
                }

                if (chars == "template")
                {
                    token = new Token(TokenType.TemplateKeyword);
                }

                if (chars == "ordered")
                {
                    token = new Token(TokenType.OrderedKeyword);
                }

                if (chars == "and")
                {
                    token = new Token(TokenType.Name);
                }

                if (chars == "if")
                {
                    token = new Token(TokenType.IfKeyword);
                }

                if (chars == "else")
                {
                    token = new Token(TokenType.ElseKeyword);
                }

                if (chars == "true" || chars == "false")
                {
                    token = new Token(TokenType.Boolean);
                }

                token ??= new Token(TokenType.Name, chars.ToString());
                return chars.Length;
            }

            if (char.IsDigit(characters[0]))
            {
                if (charactersLength == 1)
                {
                    token = null;
                    return 0;
                }

                var isHexadecimal = characters[charactersLastIndex] == 'x';

                if (charactersLength == 2 && isHexadecimal) {
                    token = null;
                    return 0;
                }


                if (!char.IsDigit(characters[charactersLastIndex])) {
                    var chars = characters.Slice(0, charactersLastIndex);
                    int integer;


                    if (isHexadecimal)
                    {
                        integer = int.Parse(chars, NumberStyles.HexNumber);
                    }
                    else {
                        integer = int.Parse(chars);
                    }

                    token = new Token(TokenType.Integer, integer);
                    return charactersLastIndex;
                }
            }

            if (characters[0] == '"')
            {
                if (charactersLength == 1)
                {
                    token = null;
                    return 0;
                }

                if (characters[charactersLastIndex] == '"')
                {
                    token = new Token(TokenType.String);
                    return charactersLength;
                }
            }



            bool tryRecognizeSingleCharacterToken(
                ReadOnlySpan<char> characters,
                char character,
                TokenType closeToken,
                out Token? token)
            {
                if (characters[0] == character)
                {
                    token = new Token(TokenType.OpenBracket);
                    return true;
                }

                token = null;
                return false;
            }

            if (charactersLength == 1
                && (tryRecognizeSingleCharacterToken(characters, '(', TokenType.OpenBracket, out token)
                    || tryRecognizeSingleCharacterToken(characters, ')', TokenType.CloseBracket, out token)
                    || tryRecognizeSingleCharacterToken(characters, '{', TokenType.OpenBrace, out token)
                    || tryRecognizeSingleCharacterToken(characters, '}', TokenType.CloseBrace, out token)
                    //|| tryRecognizeSingleCharacterToken(characters, '<', TokenType.OpenAngleBracket, out token)
                    //|| tryRecognizeSingleCharacterToken(characters, '>', TokenType.CloseAngleBracket, out token)
                    || tryRecognizeSingleCharacterToken(characters, '[', TokenType.OpenSquareBracket, out token)
                    || tryRecognizeSingleCharacterToken(characters, ']', TokenType.CloseSquareBracket, out token)
                    || tryRecognizeSingleCharacterToken(characters, ',', TokenType.CloseSquareBracket, out token)))
            {
                return 1;
            }

            tryRecognizeDoubleToken

            if (charactersLength == 2
                && (
                    )) { 
                
            }
        }

        public async Task TokenizeAsync()
        {
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileBytes = new byte[stream.Length];

            {
                var fileBytesLeft = (int)stream.Length;
                var fileBytesRead = 0;

                while (fileBytesLeft > 0)
                {
                    var bytesProcessed = await stream.ReadAsync(fileBytes, fileBytesRead, fileBytesLeft).ConfigureAwait(false);

                    if (bytesProcessed == 0)
                    {
                        break;
                    }

                    fileBytesRead += bytesProcessed;
                    fileBytesLeft -= bytesProcessed;
                }
            }

            {


                var contentToBeTokenized = Encoding.ASCII.GetString(fileBytes);
                var contentLeft = contentToBeTokenized.Length;
                var contentRead = 0;
                var contentReadLength = 0;

                while (contentLeft > 0)
                {
                    contentReadLength++;

                    if (tryRecognizeToken(contentToBeTokenized.AsSpan(contentRead, contentReadLength)))
                    {

                    }
                }
            }
        }
    }
}

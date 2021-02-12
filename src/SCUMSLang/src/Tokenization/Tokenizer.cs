using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static SCUMSLang.Tokenization.TokenizerTools;

namespace SCUMSLang.Tokenization
{
    public static class Tokenizer
    {
        public static List<Token> Tokenize(ReadOnlySpan<char> content)
        {
            var charReader = new SpanReader<char>(content);
            var tokens = new List<Token>();

            while (charReader.ConsumeNext()) {
                var newPosition = RecognizeToken(charReader, out var token);

                if (newPosition == null) {
                    throw new TokenizationException(charReader.UpperPosition, "Character(s) couldn't be recognized as token.");
                }

                if (!(token is null)) {
                    tokens.Add(token);
                }

                if (!charReader.SetPositionTo(newPosition.Value + 1)) {
                    break;
                }
            }

            return tokens;
        }

        public static List<Token> Tokenize(string content) =>
            Tokenize(content.AsSpan());

        public static async Task<List<Token>> TokenizeAsync(FileStream fileStream)
        {
            var fileBytes = new byte[fileStream.Length];

            {
                var fileBytesLeft = (int)fileStream.Length;
                var fileBytesRead = 0;

                while (fileBytesLeft > 0) {
                    var bytesProcessed = await fileStream.ReadAsync(fileBytes, fileBytesRead, fileBytesLeft).ConfigureAwait(false);

                    if (bytesProcessed == 0) {
                        break;
                    }

                    fileBytesRead += bytesProcessed;
                    fileBytesLeft -= bytesProcessed;
                }
            }

            return Tokenize(Encoding.ASCII.GetString(fileBytes));
        }
    }
}

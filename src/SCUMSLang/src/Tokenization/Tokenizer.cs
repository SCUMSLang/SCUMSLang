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
            var charReader = new Reader<char>(content);
            var tokens = new List<Token>();

            while (charReader.ConsumeNext()) {
                var newPosition = RecognizeToken(charReader, out var token);

                if (newPosition == null) {
                    throw new TokenizationException(charReader.UpperPosition);
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

        public static async Task<List<Token>> TokenizeAsync(Stream stream)
        {
            var fileBytes = new byte[stream.Length];

            {
                var fileBytesLeft = (int)stream.Length;
                var fileBytesRead = 0;

                while (fileBytesLeft > 0) {
                    var bytesProcessed = await stream.ReadAsync(fileBytes, fileBytesRead, fileBytesLeft).ConfigureAwait(false);

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

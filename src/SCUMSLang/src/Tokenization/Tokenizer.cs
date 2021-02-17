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
        public static ReadOnlyMemory<Token> Tokenize(ReadOnlySpan<char> content)
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

            return new ReadOnlyMemory<Token>(tokens.ToArray());
        }

        public static ReadOnlyMemory<Token> Tokenize(string content) =>
            Tokenize(content.AsSpan());

        public static async Task<ReadOnlyMemory<Token>> TokenizeAsync(FileStream fileStream)
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

            var utf8Preamble = Encoding.UTF8.GetPreamble();
            var fileBytesMemory = new ReadOnlyMemory<byte>(fileBytes);

            if (fileBytesMemory.Length >= utf8Preamble.Length && fileBytesMemory.Span.StartsWith(utf8Preamble)) {
                fileBytesMemory = fileBytesMemory.Slice(fileBytesMemory.Length);   
            }

            return Tokenize(Encoding.ASCII.GetString(fileBytesMemory.Span));
        }
    }
}

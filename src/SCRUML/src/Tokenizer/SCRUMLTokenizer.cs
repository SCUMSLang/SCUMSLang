using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static SCRUML.Tokenizer.SCRUMLTokenizerTools;

namespace SCRUML.Tokenizer
{
    public static class SCRUMLTokenizer
    {
        public static List<Token> Tokenize(string content)
        {

            var contentReader = new ContentReader(content);
            var tokens = new List<Token>();

            while (contentReader.MoveNext())
            {
                var charactersRead = RecognizeToken(contentReader, out var token);

                if (!(token is null))
                {
                    tokens.Add(token);
                }

                contentReader.FastForward(charactersRead);
            }

            return tokens;
        }

        public static async Task<List<Token>> TokenizeAsync(Stream stream)
        {
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

            return Tokenize(Encoding.ASCII.GetString(fileBytes));
        }
    }
}

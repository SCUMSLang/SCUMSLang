using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SCUMSLang.Text;
using static SCUMSLang.Tokenization.TokenizerTools;

namespace SCUMSLang.Tokenization
{
    public static class Tokenizer
    {
        public static ReadOnlyMemory<Token> Tokenize(ReadOnlySpan<char> content, byte positionOffset = 0, string? filePath = null)
        {
            var charReader = new SpanReader<char>(content);
            var tokens = new List<Token>();
            int previousPosition = 0;
            var previousLastLine = 1;
            var previousLastLinePosition = 0;
            char detectedEndOfLine = '\0';

            while (charReader.ConsumeNext()) {
                int? newPosition;
                Token? token;

                try {
                    newPosition = RecognizeToken(charReader, out token);
                } catch (TokenParsingException error) {
                    error.FileLine = previousLastLine;
                    throw;
                }

                if (newPosition == null) {
                    var position = charReader.UpperPosition;
                    var fileLinePosition = position - previousLastLinePosition;

                    throw new TokenParsingException(position, "Character(s) couldn't be recognized as token.") {
                        FilePositionOffset = positionOffset,
                        FileLine = previousLastLine,
                        FileLinePosition = fileLinePosition
                    };
                }

                var currentIndex = newPosition.Value;
                var lastLine = previousLastLine;

                while (currentIndex > previousPosition) {
                    void setNextLine()
                    {
                        lastLine++;

                        if (currentIndex > previousLastLinePosition) {
                            previousLastLinePosition = currentIndex + 1;
                        }
                    }

                    if (detectedEndOfLine != '\0') {
                        if (detectedEndOfLine == content[currentIndex]) {
                            setNextLine();
                        }
                    } else {
                        var character = content[currentIndex];

                        if (character == '\r' || character == '\n') {
                            detectedEndOfLine = character;
                            setNextLine();
                        }
                    }

                    currentIndex--;
                }

                if (!(token is null)) {
                    var tokenPosition = token.FilePosition;
                    token.FileLine = previousLastLine;
                    token.FilePosition = tokenPosition;
                    token.FilePositionOffset = positionOffset;
                    token.FileLinePosition = tokenPosition - previousLastLinePosition;
                    token.FilePath = filePath;
                    tokens.Add(token);
                }

                if (!charReader.SetPositionTo(newPosition.Value + 1)) {
                    break;
                }

                previousPosition = newPosition.Value;
                previousLastLine = lastLine;
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

            var fileBytesMemory = new ReadOnlyMemory<byte>(fileBytes);
            int filePositionOffset;

            if (EncodingTools.IsUTF8PreamblePresent(fileBytesMemory.Span, out filePositionOffset)) {
                fileBytesMemory = fileBytesMemory.Slice(filePositionOffset);
            }

            return Tokenize(
                Encoding.ASCII.GetString(fileBytesMemory.Span), 
                positionOffset: (byte)filePositionOffset, 
                filePath: fileStream.Name);
        }
    }
}

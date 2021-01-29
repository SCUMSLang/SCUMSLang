using System;
using System.Diagnostics;

namespace SCUMSLang.Tokenizer
{
    internal class ContentReader
    {
        public string Content { get; }

        private int contentReadPosition = 0;
        private int contentReadLength = 0;

        public ContentReader(string content)
        {
            Content = content;
        }

        public bool MoveNext() {
            Debug.Assert(contentReadLength >= 0, "Content read length should not be lesser than zero.");
            contentReadLength++;

            if (contentReadPosition + contentReadLength > Content.Length) {
                return false;
            }

            return true;
        }


        public bool MoveNext(out ReadOnlySpan<char> characters)
        {
            if (!MoveNext())
            {
                characters = ReadOnlySpan<char>.Empty;
                return false;
            }

            characters = ReadCharacters();
            return true;
        }

        public ReadOnlySpan<char> ReadCharacters() =>
            Content.AsSpan(contentReadPosition, contentReadLength);

        public void Rewind(int charactersRead) {
            contentReadLength -= charactersRead;
        }

        public void Rewind(int charactersRead, out ReadOnlySpan<char> characters) {
            Rewind(charactersRead);
            characters = ReadCharacters();
        }

        public void FastForward(int charactersRead)
        {
            contentReadPosition += charactersRead;
            contentReadLength -= charactersRead;
        }
    }
}

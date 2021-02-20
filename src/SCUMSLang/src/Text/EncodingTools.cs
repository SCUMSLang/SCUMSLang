using System;
using System.Text;

namespace SCUMSLang.Text
{
    public class EncodingTools
    {
        public static byte[] UTF8Preamble = Encoding.UTF8.GetPreamble();

        public static bool IsUTF8PreamblePresent(ReadOnlySpan<byte> bytes, out int sliceAt)
        {
            if (bytes.Length >= UTF8Preamble.Length && bytes.StartsWith(UTF8Preamble)) {
                sliceAt = UTF8Preamble.Length;
                return true;
            }

            sliceAt = 0;
            return false;
        }

        public static ReadOnlySpan<byte> CutOffUTF8Preamble(ReadOnlySpan<byte> bytes)
        {
            if (IsUTF8PreamblePresent(bytes, out var sliceAt)) {
                return bytes.Slice(sliceAt);
            }

            return bytes;
        }

        public static ReadOnlyMemory<byte> CutOffUTF8Preamble(ReadOnlyMemory<byte> bytes)
        {
            if (IsUTF8PreamblePresent(bytes.Span, out var sliceAt)) {
                return bytes.Slice(sliceAt);
            } 
            
            return bytes;
        }
    }
}

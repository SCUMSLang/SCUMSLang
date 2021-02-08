using System.Collections.Generic;

namespace SCUMSLang.Tokenization
{
    public class MemberAccessToken : Token
    {
        public IReadOnlyList<string> PathFragments { get; }

        public MemberAccessToken(int position, int length, IReadOnlyList<string> pathFragments)
            : base(TokenType.MemberAccess, position, length) =>
            PathFragments = pathFragments;
    }
}

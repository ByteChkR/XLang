using System.Collections.Generic;
using System.Text;

using XLang.Core;
using XLang.Parser.Token;

namespace XLang.Parser.Shared
{
    public class XLangTokenReadException : XLangTokenParseException
    {

        public readonly XLangTokenType[] Expected;
        public readonly IEnumerable<IXLangToken> Sequence;
        public readonly XLangTokenType Unmatched;

        public XLangTokenReadException(
            IEnumerable<IXLangToken> tokenSequence, XLangTokenType[] expected, XLangTokenType unmatched, int start) :
            base($"Expected '{GetExpectedTokenString(expected)}' but got '{unmatched} at index {start}'")
        {
            Sequence = tokenSequence;
            Expected = expected;
            Unmatched = unmatched;
        }

        public XLangTokenReadException(
            IEnumerable<IXLangToken> tokenSequence, XLangTokenType expected, XLangTokenType unmatched,
            int start) : this(
                              tokenSequence,
                              new[] { expected },
                              unmatched,
                              start
                             )
        {
        }

        private static string GetExpectedTokenString(XLangTokenType[] expected)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < expected.Length; i++)
            {
                sb.Append(expected[i]);
                if (i != expected.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

    }
}
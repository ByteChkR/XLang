using System.Collections.Generic;
using System.Text;
using XLang.Core;
using XLang.Exceptions;
using XLang.Parser.Shared;
using XLang.Parser.Token;

/// <summary>
/// Contains Parser Specific Exceptions
/// </summary>
namespace XLang.Parser.Exceptions
{
    /// <summary>
    /// Occurs if the Parser Encounters a Token that is unexpected
    /// </summary>
    public class XLangTokenReadException : XLangTokenParseException
    {
        /// <summary>
        /// The Expected Tokens
        /// </summary>
        private readonly XLangTokenType[] expected;

        /// <summary>
        /// The Sequence that was unexpected
        /// </summary>
        private readonly IEnumerable<IXLangToken> sequence;
        /// <summary>
        /// The Token that led to the Exception
        /// </summary>
        private readonly XLangTokenType unmatched;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="tokenSequence">Token Sequence</param>
        /// <param name="expected">Expected Tokens</param>
        /// <param name="unmatched">Unmatched Token</param>
        /// <param name="start">Start index in source</param>
        public XLangTokenReadException(
            IEnumerable<IXLangToken> tokenSequence, XLangTokenType[] expected, XLangTokenType unmatched, int start) :
            base($"Expected '{GetExpectedTokenString(expected)}' but got '{unmatched} at index {start}'")
        {
            sequence = tokenSequence;
            this.expected = expected;
            this.unmatched = unmatched;
        }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="tokenSequence">Token Sequence</param>
        /// <param name="expected">Expected Token</param>
        /// <param name="unmatched">Unmatched Token</param>
        /// <param name="start">Start index in source</param>
        public XLangTokenReadException(
            IEnumerable<IXLangToken> tokenSequence, XLangTokenType expected, XLangTokenType unmatched,
            int start) : this(
            tokenSequence,
            new[] {expected},
            unmatched,
            start
        )
        {
        }

        /// <summary>
        /// Returns the string representation of the expected tokens
        /// </summary>
        /// <param name="expected">Expected Tokens</param>
        /// <returns></returns>
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
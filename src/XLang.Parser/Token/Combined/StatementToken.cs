using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Implements a Statement Token
    /// </summary>
    public class StatementToken : CombinedToken
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="start">Start index in source</param>
        /// <param name="subtokens">Child Tokens</param>
        public StatementToken(int start, IXLangToken[] subtokens) : base(XLangTokenType.OpStatement, subtokens, start)
        {
        }
    }
}
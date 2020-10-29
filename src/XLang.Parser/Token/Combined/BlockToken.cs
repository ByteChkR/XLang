using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Block Token Implementation
    /// </summary>
    public class BlockToken : CombinedToken
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="start">Start in Source</param>
        /// <param name="subtokens">Child Tokens</param>
        public BlockToken(int start, IXLangToken[] subtokens) : base(XLangTokenType.OpBlockToken, subtokens, start)
        {
        }
    }
}
using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    public class StatementToken : CombinedToken
    {
        public StatementToken(int start, IXLangToken[] subtokens) : base(XLangTokenType.OpStatement, subtokens, start)
        {
        }
    }
}
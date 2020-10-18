using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    public class BlockToken : CombinedToken
    {

        public BlockToken(int start, IXLangToken[] subtokens) : base(XLangTokenType.OpBlockToken, subtokens, start)
        {
        }

    }
}
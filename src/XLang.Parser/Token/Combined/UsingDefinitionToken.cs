using System.Linq;
using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    public class UsingDefinitionToken : CombinedToken
    {
        public readonly IXLangToken UsingKey;
        public readonly IXLangToken[] UsingParts;

        public UsingDefinitionToken(XLangTokenType type, IXLangToken usingKey, IXLangToken[] usingParts, int start) : base(type, new []{usingKey}.Concat(usingParts).ToArray(), start)
        {
            UsingKey = usingKey;
            UsingParts = usingParts;
        }
    }
}
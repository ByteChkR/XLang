using System.Collections.Generic;
using System.Linq;

using XLang.Parser.Shared;
using XLang.Parser.Token;

namespace XLang.Parser.Reader
{
    public class XLangExpressionReader
    {

        public readonly List<IXLangToken> tokens;

        private int currentIdx;

        public XLangExpressionReader(List<IXLangToken> tokens)
        {
            this.tokens = tokens.ToList();
        }

        public IXLangToken PeekNext(int advance = 1)
        {
            XLangParsingTools.ReadAnyOrNone(tokens, currentIdx + advance-1, out IXLangToken result);
            return result;
        }

        public IXLangToken GetNext()
        {
            XLangParsingTools.ReadAnyOrNone(tokens, currentIdx, out IXLangToken result);
            currentIdx++;
            return result;
        }

    }
}
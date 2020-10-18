using System.Collections.Generic;

using XLang.Core;

namespace XLang.Parser.Token.BaseTokens
{
    public class SymbolToken : IXLangToken
    {

        private readonly XLangSettings settings;

        public SymbolToken(XLangSettings settings, XLangTokenType type, int startIndex)
        {
            this.settings = settings;
            StartIndex = startIndex;
            Type = type;
        }

        public int StartIndex { get; }

        public XLangTokenType Type { get; }

        public List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public virtual string GetValue()
        {
            return settings.ReverseReservedSymbols[Type].ToString();
        }

        public override string ToString()
        {
            return GetValue();
        }

    }
}
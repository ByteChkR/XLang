using System.Collections.Generic;
using XLang.Core;

namespace XLang.Parser.Token.BaseTokens
{
    public class TextToken : IXLangToken
    {
        public TextToken(XLangTokenType type, string value, int startIndex)
        {
            Type = type;
            Value = value;
            StartIndex = startIndex;
        }

        public string Value { get; }

        public XLangTokenType Type { get; }

        public int StartIndex { get; }

        public virtual string GetValue()
        {
            return Value;
        }


        public List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public override string ToString()
        {
            return GetValue();
        }
    }
}
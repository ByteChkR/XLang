using System.Collections.Generic;
using System.Linq;
using System.Text;

using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    public abstract class CombinedToken : IXLangToken
    {

        public readonly List<IXLangToken> SubTokens;

        protected CombinedToken(XLangTokenType type, IXLangToken[] subtokens, int start)
        {
            SubTokens = subtokens.ToList();
            StartIndex = start;
            Type = type;
        }


        public int StartIndex { get; }

        public XLangTokenType Type { get; }

        public List<IXLangToken> GetChildren()
        {
            return SubTokens;
        }


        public string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IXLangToken token in SubTokens)
            {
                sb.Append(token.GetValue());
            }

            return sb.ToString();
        }

        protected static string Unpack(IXLangToken[] t)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IXLangToken token in t)
            {
                sb.Append(token.GetValue());
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return GetValue();
        }

    }
}
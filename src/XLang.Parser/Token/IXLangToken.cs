using System.Collections.Generic;

using XLang.Core;

namespace XLang.Parser.Token
{
    public interface IXLangToken
    {

        XLangTokenType Type { get; }

        int StartIndex { get; }

        List<IXLangToken> GetChildren();

        string GetValue();

    }
}
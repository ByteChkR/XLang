using System;

namespace XLang.Parser.Shared
{
    public class XLangTokenParseException : Exception
    {
        public XLangTokenParseException(string message) : base(message)
        {
        }
    }
}
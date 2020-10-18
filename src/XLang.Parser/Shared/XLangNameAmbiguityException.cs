using System;

namespace XLang.Parser.Shared
{
    public class XLangNameAmbiguityException : Exception
    {

        public XLangNameAmbiguityException(string message) : base(message)
        {
        }

    }
}
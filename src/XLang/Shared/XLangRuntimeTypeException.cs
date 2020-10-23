using System;

namespace XLang.Shared
{
    public class XLangRuntimeTypeException : Exception
    {
        public XLangRuntimeTypeException(string message) : base(message)
        {
        }
    }
}
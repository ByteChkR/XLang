using System;

namespace XLang.Exceptions
{
    /// <summary>
    ///     Gets thrown if an invalid operation is performed on a type
    /// </summary>
    public class XLangRuntimeTypeException : Exception
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="message"></param>
        public XLangRuntimeTypeException(string message) : base(message)
        {
        }
    }
}
using System;

namespace XLang.Exceptions
{
    /// <summary>
    /// Gets thrown if a generic parsing error occurred.
    /// </summary>
    public class XLangTokenParseException : Exception
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public XLangTokenParseException(string message) : base(message)
        {
        }
    }
}
using System;

/// <summary>
/// Contains XLang Exceptions
/// </summary>
namespace XLang.Exceptions
{
    /// <summary>
    /// Gets Thrown if a redefinition of a Symbol occurs
    /// </summary>
    public class XLangNameAmbiguityException : Exception
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public XLangNameAmbiguityException(string message) : base(message)
        {
        }
    }
}
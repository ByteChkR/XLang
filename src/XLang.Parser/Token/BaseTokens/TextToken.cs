using System.Collections.Generic;
using XLang.Core;

/// <summary>
/// Contains Base Token Implementations
/// </summary>
namespace XLang.Parser.Token.BaseTokens
{
    /// <summary>
    ///     Represents a Token that contains a Sequence of Characters
    /// </summary>
    public class TextToken : IXLangToken
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="type">Token Type</param>
        /// <param name="value">Token Value</param>
        /// <param name="startIndex">Start index in the source stream</param>
        public TextToken(XLangTokenType type, string value, int startIndex)
        {
            Type = type;
            Value = value;
            StartIndex = startIndex;
        }

        /// <summary>
        ///     The Token Value
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     The Token type
        /// </summary>
        public XLangTokenType Type { get; }

        /// <summary>
        ///     The Start index in the source stream
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        ///     returns the Value of the token
        /// </summary>
        /// <returns>Value of the Token</returns>
        public virtual string GetValue()
        {
            return Value;
        }


        /// <summary>
        ///     Returns all Children of this token
        /// </summary>
        /// <returns>Child Tokens</returns>
        public List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }


        /// <summary>
        ///     Returns the String Representation of this token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetValue();
        }
    }
}
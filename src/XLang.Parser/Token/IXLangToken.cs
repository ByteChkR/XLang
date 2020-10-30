using System.Collections.Generic;
using XLang.Core;

/// <summary>
/// Contains All Implemented Parser Tokens
/// </summary>
namespace XLang.Parser.Token
{
    /// <summary>
    ///     Defines the Interface of a XLang Parsing Token
    /// </summary>
    public interface IXLangToken
    {
        /// <summary>
        ///     The Token Type
        /// </summary>
        XLangTokenType Type { get; }

        /// <summary>
        ///     The Start index in the source code.
        /// </summary>
        int SourceIndex { get; }

        /// <summary>
        ///     Returns the Child Tokens
        /// </summary>
        /// <returns>Child Tokens</returns>
        List<IXLangToken> GetChildren();

        /// <summary>
        ///     Get the value of the Token
        /// </summary>
        /// <returns>Token Value</returns>
        string GetValue();
    }
}
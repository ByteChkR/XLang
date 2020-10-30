using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLang.Core;

/// <summary>
/// Contains all Combined Tokens
/// </summary>
namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Represents a Token that is constructed from its child tokens
    /// </summary>
    public abstract class CombinedToken : IXLangToken
    {
        /// <summary>
        ///     The Child Tokens
        /// </summary>
        public readonly List<IXLangToken> SubTokens;

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="type">Token Type</param>
        /// <param name="subtokens">Child Tokens</param>
        /// <param name="start">Start index in the source</param>
        protected CombinedToken(XLangTokenType type, IXLangToken[] subtokens, int start)
        {
            SubTokens = subtokens.ToList();
            SourceIndex = start;
            Type = type;
        }

        /// <summary>
        ///     Start index in the source
        /// </summary>
        public int SourceIndex { get; }

        /// <summary>
        ///     The Token Type
        /// </summary>
        public XLangTokenType Type { get; }

        /// <summary>
        ///     Returns the Child Tokens
        /// </summary>
        /// <returns></returns>
        public List<IXLangToken> GetChildren()
        {
            return SubTokens;
        }


        /// <summary>
        ///     Returns the Value of this token
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            return Unpack(SubTokens.ToArray());
        }

        /// <summary>
        ///     Helper function that unpacks a Sequence of tokens into a string representation
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected static string Unpack(IXLangToken[] t)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IXLangToken token in t)
            {
                sb.Append(token.GetValue());
            }

            return sb.ToString();
        }

        /// <summary>
        ///     String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetValue();
        }
    }
}
using System.Linq;
using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Implements a Using Definition Token
    /// </summary>
    public class UsingDefinitionToken : CombinedToken
    {
        /// <summary>
        ///     Using Key
        /// </summary>
        public readonly IXLangToken UsingKey;

        /// <summary>
        ///     The Using Parts (Namespace name)
        /// </summary>
        public readonly IXLangToken[] UsingParts;

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="type">Token Type</param>
        /// <param name="usingKey">The Using Key</param>
        /// <param name="usingParts">The Namespace Name</param>
        /// <param name="start">Start in source</param>
        public UsingDefinitionToken(XLangTokenType type, IXLangToken usingKey, IXLangToken[] usingParts, int start) :
            base(type, new[] {usingKey}.Concat(usingParts).ToArray(), start)
        {
            UsingKey = usingKey;
            UsingParts = usingParts;
        }
    }
}
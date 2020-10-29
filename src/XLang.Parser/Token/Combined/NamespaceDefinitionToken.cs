using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Implements a Namespace Definition Token
    /// </summary>
    public class NamespaceDefinitionToken : CombinedToken
    {
        /// <summary>
        ///     Namespace name
        /// </summary>
        public readonly IXLangToken Name;
        /// <summary>
        ///     Namespace Key
        /// </summary>
        public readonly IXLangToken NamespaceKey;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="nameSpace">Namespace Key</param>
        /// <param name="name">Namespace Name</param>
        /// <param name="subtokens">Child Tokens</param>
        public NamespaceDefinitionToken(IXLangToken nameSpace, IXLangToken name, IXLangToken[] subtokens) : base(
            XLangTokenType
                .OpNamespaceDefinition,
            subtokens,
            nameSpace
                .StartIndex
        )
        {
            Name = name;
            NamespaceKey = nameSpace;
        }

        /// <summary>
        ///     String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{NamespaceKey.GetValue()} {Name.GetValue()}";
        }
    }
}
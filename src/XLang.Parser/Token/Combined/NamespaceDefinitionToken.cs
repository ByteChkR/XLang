using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    public class NamespaceDefinitionToken : CombinedToken
    {
        public readonly IXLangToken Name;
        public readonly IXLangToken NamespaceKey;

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

        public override string ToString()
        {
            return $"{NamespaceKey.GetValue()} {Name.GetValue()}";
        }
    }
}
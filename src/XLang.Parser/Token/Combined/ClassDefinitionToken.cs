using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    public class ClassDefinitionToken : CombinedToken
    {
        public readonly IXLangToken BaseClass;
        public readonly IXLangToken ClassKey;
        public readonly IXLangToken[] Modifiers;
        public readonly IXLangToken Name;

        public ClassDefinitionToken(
            IXLangToken classKey, IXLangToken name, IXLangToken baseClass, IXLangToken[] modifiers,
            IXLangToken[] subtokens) : base(
            XLangTokenType
                .OpClassDefinition,
            subtokens,
            classKey
                .StartIndex
        )
        {
            BaseClass = baseClass;
            Name = name;
            Modifiers = modifiers;
            ClassKey = classKey;
        }

        public override string ToString()
        {
            return $"{Unpack(Modifiers)} {ClassKey.GetValue()} {Name.GetValue()}";
        }
    }
}
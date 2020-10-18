using XLang.Core;
using XLang.Parser.Token.Expressions;

namespace XLang.Parser.Token.Combined
{
    public class VariableDefinitionToken : CombinedToken
    {

        public readonly XLangExpression InitializerExpression;

        public readonly IXLangToken[] Modifiers;
        public readonly IXLangToken Name;
        public readonly IXLangToken TypeName;

        public VariableDefinitionToken(
            IXLangToken name, IXLangToken typeName, IXLangToken[] modifiers, IXLangToken[] subtokens,
            XLangExpression initializerExpression) : base(
                                                          XLangTokenType.OpVariableDefinition,
                                                          subtokens,
                                                          typeName.StartIndex
                                                         )
        {
            Modifiers = modifiers;
            Name = name;
            TypeName = typeName;
            InitializerExpression = initializerExpression;
        }

        public override string ToString()
        {
            return $"{Unpack(Modifiers)} {TypeName.GetValue()} {Name.GetValue()}";
        }

    }
}
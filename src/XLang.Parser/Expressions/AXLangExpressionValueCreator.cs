using XLang.Parser.Token.Expressions;

namespace XLang.Parser.Expressions
{
    public abstract class AXLangExpressionValueCreator
    {

        public abstract XLangExpression CreateValue(XLangExpressionParser parser);

    }
}
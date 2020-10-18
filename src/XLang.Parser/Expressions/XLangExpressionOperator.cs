using XLang.Parser.Token.Expressions;

namespace XLang.Parser.Expressions
{
    public abstract class XLangExpressionOperator
    {
        public abstract int PrecedenceLevel { get; }
        public abstract bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode);
        public abstract XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode);
    }
}
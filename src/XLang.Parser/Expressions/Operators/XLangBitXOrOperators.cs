using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangBitXOrOperators : XLangExpressionOperator
    {
        public override int PrecedenceLevel => 7;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpCap;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpCap);
            return new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpCap,
                parser.ParseExpr(PrecedenceLevel));
        }
    }
}
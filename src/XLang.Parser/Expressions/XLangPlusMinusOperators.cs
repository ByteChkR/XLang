using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangPlusMinusOperators : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 12;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpPlus ||
                   parser.CurrentToken.Type == XLangTokenType.OpMinus;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            XLangTokenType type = parser.CurrentToken.Type;
            parser.Eat(parser.CurrentToken.Type);
            XLangExpression token = new XLangBinaryOp(parser.Context, currentNode, type, parser.ParseExpr(PrecedenceLevel));
            return token;
        }

    }
}
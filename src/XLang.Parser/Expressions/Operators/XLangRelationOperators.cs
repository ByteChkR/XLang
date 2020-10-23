using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangRelationOperators : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 10;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpLessThan ||
                   parser.CurrentToken.Type == XLangTokenType.OpGreaterThan;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            XLangTokenType type = parser.CurrentToken.Type;
            parser.Eat(parser.CurrentToken.Type);
            XLangExpression node = null;

            if (type == XLangTokenType.OpLessThan)
            {
                if (parser.CurrentToken.Type == XLangTokenType.OpEquality)
                {
                    parser.Eat(XLangTokenType.OpEquality);
                    node = new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpLessOrEqual, parser.ParseExpr(PrecedenceLevel));
                }
                else
                {
                    node = new XLangBinaryOp(parser.Context, currentNode, type, parser.ParseExpr(PrecedenceLevel));
                }
            }
            else if (type == XLangTokenType.OpGreaterThan)
            {
                if (parser.CurrentToken.Type == XLangTokenType.OpEquality)
                {
                    parser.Eat(XLangTokenType.OpEquality);
                    node = new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpGreaterOrEqual, parser.ParseExpr(PrecedenceLevel));
                }
                else
                {
                    node = new XLangBinaryOp(parser.Context, currentNode, type, parser.ParseExpr(PrecedenceLevel));
                }
            }

            return node;
        }

    }
}
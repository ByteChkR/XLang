using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangAssignmentOperators : XLangExpressionOperator
    {
        public override int PrecedenceLevel => 2;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpEquality &&
                   parser.Reader.PeekNext().Type != XLangTokenType.OpEquality;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpEquality);
            return new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpEquality, parser.ParseExpr(0));
        }
    }
}
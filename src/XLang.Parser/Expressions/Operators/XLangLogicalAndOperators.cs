using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangLogicalAndOperators : XLangExpressionOperator
    {
        public override int PrecedenceLevel => 5;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpAnd &&
                   parser.Reader.PeekNext().Type == XLangTokenType.OpAnd;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpAnd);
            parser.Eat(XLangTokenType.OpAnd);
            return new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpLogicalAnd,
                parser.ParseExpr(PrecedenceLevel));
        }
    }
}
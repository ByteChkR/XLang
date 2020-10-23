using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangEqualityOperators : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 9;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpEquality &&
                   parser.Reader.PeekNext().Type == XLangTokenType.OpEquality;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpEquality);
            parser.Eat(XLangTokenType.OpEquality);

            return new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpComparison, parser.ParseExpr(PrecedenceLevel));

        }

    }

    public class XLangInEqualityOperators : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 9;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpBang &&
                   parser.Reader.PeekNext().Type == XLangTokenType.OpEquality;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpBang);
            parser.Eat(XLangTokenType.OpEquality);

            return new XLangUnaryOp(parser.Context, new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpComparison, parser.ParseExpr(PrecedenceLevel)), XLangTokenType.OpBang);
            
        }

    }
}
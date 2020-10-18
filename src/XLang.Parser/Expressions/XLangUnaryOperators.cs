using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangUnaryOperators : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 14;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpMinus ||
                   parser.CurrentToken.Type == XLangTokenType.OpPlus ||
                   parser.CurrentToken.Type == XLangTokenType.OpBang ||
                   parser.CurrentToken.Type == XLangTokenType.OpTilde;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(parser.CurrentToken.Type);
            XLangExpression token = new XLangUnaryOp(parser.Context, parser.ParseExpr(0), parser.CurrentToken.Type);
            return token;
        }

    }
}
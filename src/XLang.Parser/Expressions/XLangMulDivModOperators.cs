using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangMulDivModOperators : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 13;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpAsterisk ||
                   parser.CurrentToken.Type == XLangTokenType.OpFwdSlash ||
                   parser.CurrentToken.Type == XLangTokenType.OpPercent;
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
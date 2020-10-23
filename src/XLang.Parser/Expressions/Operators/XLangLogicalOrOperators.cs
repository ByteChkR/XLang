using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions
{
    public class XLangLogicalOrOperators : XLangExpressionOperator
    {
        public override int PrecedenceLevel => 4;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpPipe &&
                   parser.Reader.PeekNext().Type == XLangTokenType.OpPipe;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpPipe);
            parser.Eat(XLangTokenType.OpPipe);
            return new XLangBinaryOp(parser.Context, currentNode, XLangTokenType.OpLogicalOr,
                parser.ParseExpr(PrecedenceLevel));
        }
    }
}
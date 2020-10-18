using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators.Special;

namespace XLang.Parser.Expressions
{
    public class XLangMemberSelectorOperator : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 15;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpDot;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpDot);
            string name = parser.CurrentToken.GetValue();
            parser.Eat(parser.CurrentToken.Type);
            return new XLangMemberAccessOp(parser.Context, currentNode, name);
        }

    }
}
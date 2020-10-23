using System.Collections.Generic;

using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators.Special;

namespace XLang.Parser.Expressions
{
    public class XLangInvocationSelectorOperator : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 15;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpBracketOpen;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            parser.Eat(XLangTokenType.OpBracketOpen);
            List<XLangExpression> parameterList = new List<XLangExpression>();
            bool comma = false;
            while (parser.CurrentToken.Type != XLangTokenType.OpBracketClose)
            {
                if (comma)
                {
                    parser.Eat(XLangTokenType.OpComma);
                    comma = false;
                }
                else
                {
                    parameterList.Add(parser.ParseExpr(PrecedenceLevel));
                    comma = true;
                }
            }

            parser.Eat(XLangTokenType.OpBracketClose);

            return new XLangInvocationOp(parser.Context, currentNode, parameterList.ToArray());
        }

    }
}
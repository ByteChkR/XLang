using System.Collections.Generic;

using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators.Special;

namespace XLang.Parser.Expressions
{
    public class XLangArraySubscriptOperator : XLangExpressionOperator
    {

        public override int PrecedenceLevel => 15;

        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpIndexerBracketOpen;
        }

        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {

            parser.Eat(XLangTokenType.OpIndexerBracketOpen);
            List<XLangExpression> parameterList = new List<XLangExpression>();
            bool comma = false;
            while (parser.CurrentToken.Type != XLangTokenType.OpIndexerBracketClose)
            {
                if (comma)
                {
                    parser.Eat(XLangTokenType.OpComma);
                }
                else
                {
                    parameterList.Add(parser.ParseExpr(PrecedenceLevel));
                    comma = true;
                }
            }

            parser.Eat(XLangTokenType.OpIndexerBracketClose);
            return new XLangArrayAccessorOp(parser.Context, currentNode, parameterList);
        }

    }
}
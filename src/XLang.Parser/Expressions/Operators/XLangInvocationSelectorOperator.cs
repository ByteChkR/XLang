using System.Collections.Generic;
using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators.Special;

namespace XLang.Parser.Expressions.Operators
{
    /// <summary>
    ///     Implements Invocation Operator
    /// </summary>
    public class XLangInvocationSelectorOperator : XLangExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 15;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpBracketOpen;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
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
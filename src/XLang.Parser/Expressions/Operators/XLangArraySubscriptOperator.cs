using System.Collections.Generic;
using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators.Special;

/// <summary>
/// Contains Operator Implementations for the XLangExpressionParser
/// </summary>
namespace XLang.Parser.Expressions.Operators
{
    /// <summary>
    ///     Implements ArraySubscript Operator
    /// </summary>
    public class XLangArraySubscriptOperator : XLangExpressionOperator
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
            return parser.CurrentToken.Type == XLangTokenType.OpIndexerBracketOpen;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
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
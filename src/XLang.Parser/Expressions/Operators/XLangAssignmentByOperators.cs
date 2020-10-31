using System;

using XLang.Core;
using XLang.Parser.Token;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions.Operators
{
    /// <summary>
    ///     Implements Assignment by Sum and Difference
    /// </summary>
    public class XLangAssignmentByOperators : XLangExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 2;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return (parser.CurrentToken.Type == XLangTokenType.OpPlus ||
                    parser.CurrentToken.Type == XLangTokenType.OpMinus ||
                    parser.CurrentToken.Type == XLangTokenType.OpAsterisk ||
                    parser.CurrentToken.Type == XLangTokenType.OpFwdSlash ||
                    parser.CurrentToken.Type == XLangTokenType.OpPercent ||
                    parser.CurrentToken.Type == XLangTokenType.OpAnd ||
                    parser.CurrentToken.Type == XLangTokenType.OpPipe ||
                    parser.CurrentToken.Type == XLangTokenType.OpCap) &&
                   parser.Reader.PeekNext().Type == XLangTokenType.OpEquality;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            IXLangToken token = parser.CurrentToken;
            XLangTokenType tt;

            switch (parser.CurrentToken.Type)
            {
                case XLangTokenType.OpPlus:
                    tt = XLangTokenType.OpSumAssign;
                    break;
                case XLangTokenType.OpMinus:
                    tt = XLangTokenType.OpDifAssign;
                    break;
                case XLangTokenType.OpAsterisk:
                    tt = XLangTokenType.OpProdAssign;
                    break;
                case XLangTokenType.OpFwdSlash:
                    tt = XLangTokenType.OpQuotAssign;
                    break;
                case XLangTokenType.OpPercent:
                    tt = XLangTokenType.OpRemAssign;
                    break;
                case XLangTokenType.OpPipe:
                    tt = XLangTokenType.OpOrAssign;
                    break;
                case XLangTokenType.OpAnd:
                    tt = XLangTokenType.OpAndAssign;
                    break;
                case XLangTokenType.OpCap:
                    tt = XLangTokenType.OpXOrAssign;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            parser.Eat(token.Type);
            parser.Eat(XLangTokenType.OpEquality);
            return new XLangBinaryOp(parser.Context, currentNode, tt, parser.ParseExpr(0));
        }
    }
}
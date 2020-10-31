using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions.Operators
{
    /// <summary>
    ///     Implements Unary Plus/Minus by Assignment Operators
    /// </summary>
    public class XLangAssignmentPlusMinusOperators : XLangExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 14;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpPlus &&
                   parser.Reader.PeekNext().Type == XLangTokenType.OpPlus ||
                   parser.CurrentToken.Type == XLangTokenType.OpMinus &&
                   parser.Reader.PeekNext().Type == XLangTokenType.OpMinus;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            XLangTokenType tt = parser.CurrentToken.Type == XLangTokenType.OpPlus
                                    ? XLangTokenType.OpUnaryIncrement
                                    : XLangTokenType.OpUnaryDecrement;
            XLangTokenType type = parser.CurrentToken.Type;
            parser.Eat(parser.CurrentToken.Type);
            parser.Eat(parser.CurrentToken.Type);
            XLangExpression token =
                new XLangUnaryOp(parser.Context, currentNode, tt);
            return token;
        }
    }
}
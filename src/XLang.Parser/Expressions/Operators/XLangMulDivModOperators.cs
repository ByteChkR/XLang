using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions.Operators
{
    /// <summary>
    ///     Implements Multiply/Divide Operators
    /// </summary>
    public class XLangMulDivModOperators : XLangExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 13;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode)
        {
            return parser.CurrentToken.Type == XLangTokenType.OpAsterisk ||
                   parser.CurrentToken.Type == XLangTokenType.OpFwdSlash ||
                   parser.CurrentToken.Type == XLangTokenType.OpPercent;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            XLangTokenType type = parser.CurrentToken.Type;
            parser.Eat(parser.CurrentToken.Type);
            XLangExpression token =
                new XLangBinaryOp(parser.Context, currentNode, type, parser.ParseExpr(PrecedenceLevel));
            return token;
        }
    }
}
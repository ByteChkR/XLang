using XLang.Core;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;

namespace XLang.Parser.Expressions.Operators
{
    /// <summary>
    ///     Implements Unary Operators
    /// </summary>
    public class XLangUnaryOperators : XLangExpressionOperator
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
            return parser.CurrentToken.Type == XLangTokenType.OpMinus ||
                   parser.CurrentToken.Type == XLangTokenType.OpPlus ||
                   parser.CurrentToken.Type == XLangTokenType.OpBang ||
                   parser.CurrentToken.Type == XLangTokenType.OpTilde;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode)
        {
            XLangExpression token = new XLangUnaryOp(parser.Context, parser.ParseExpr(0), parser.CurrentToken.Type);
            parser.Eat(parser.CurrentToken.Type);
            return token;
        }
    }
}
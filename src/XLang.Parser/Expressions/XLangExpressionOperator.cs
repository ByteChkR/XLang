using XLang.Parser.Token.Expressions;

namespace XLang.Parser.Expressions
{
    /// <summary>
    ///     Abstract XLangExpressionOperator
    /// </summary>
    public abstract class XLangExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operator
        /// </summary>
        public abstract int PrecedenceLevel { get; }

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public abstract bool CanCreate(XLangExpressionParser parser, XLangExpression currentNode);

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public abstract XLangExpression Create(XLangExpressionParser parser, XLangExpression currentNode);
    }
}
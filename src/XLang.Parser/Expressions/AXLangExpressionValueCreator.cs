using XLang.Parser.Token.Expressions;

/// <summary>
/// Contains The XLang Expression Parser Implementation
/// </summary>
namespace XLang.Parser.Expressions
{
    /// <summary>
    ///     Abstract Expression Value Creator
    /// </summary>
    public abstract class AXLangExpressionValueCreator
    {
        /// <summary>
        ///     Creates an XLangExpression from the current Position of the Parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <returns>Parsed XLangExpression</returns>
        public abstract XLangExpression CreateValue(XLangExpressionParser parser);
    }
}
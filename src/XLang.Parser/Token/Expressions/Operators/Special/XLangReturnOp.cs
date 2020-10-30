using System.Collections.Generic;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    /// <summary>
    ///     Return Operator Implementation
    /// </summary>
    public class XLangReturnOp : XLangExpression
    {
        /// <summary>
        ///     Right side expression (return value)
        /// </summary>
        private readonly XLangExpression right;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="right">Right side Expression</param>
        public XLangReturnOp(XLangContext context, XLangExpression right, int sourceIdx) : base(context, sourceIdx)
        {
            this.right = right;
        }


        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        /// <summary>
        ///     Returns String representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return $"return {right}";
        }

        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            scope.SetReturn(right?.Process(scope, instance));
            return null;
        }
    }
}
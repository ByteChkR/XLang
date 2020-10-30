using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators
{
    /// <summary>
    ///     Implements Unary Operators
    /// </summary>
    public class XLangUnaryOp : XLangExpression
    {
        /// <summary>
        ///     Left Side of the Expression
        /// </summary>
        public readonly XLangExpression Left;

        /// <summary>
        ///     The Expression Type
        /// </summary>
        public readonly XLangTokenType OperationType;


        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side</param>
        /// <param name="operationType">Operation Type</param>
        public XLangUnaryOp(XLangContext context, XLangExpression left, XLangTokenType operationType) : base(context, left.SourceIndex)
        {
            Left = left;
            OperationType = operationType;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken> {Left};
        }

        /// <summary>
        ///     Returns String representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return $"{OperationType}({Left.GetValue()})";
        }

        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            IXLangRuntimeTypeInstance left = Left.Process(scope, instance);

            if (OperationType == XLangTokenType.OpNew)
            {
                return left;
            }

            return Context.GetUnaryOperatorImplementation(left.Type, OperationType).Invoke(null, new[] {left});
        }
    }
}
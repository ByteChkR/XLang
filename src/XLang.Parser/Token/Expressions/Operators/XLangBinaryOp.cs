using System.Collections.Generic;
using System.Linq;
using XLang.Core;
using XLang.Exceptions;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared;

/// <summary>
/// Contains XLangExpression Implementations
/// </summary>
namespace XLang.Parser.Token.Expressions.Operators
{
    /// <summary>
    ///     Implements Binary Operators
    /// </summary>
    public class XLangBinaryOp : XLangExpression
    {
        /// <summary>
        ///     Left side of the Expression
        /// </summary>
        public readonly XLangExpression Left;
        /// <summary>
        ///     The Operation Type
        /// </summary>
        public readonly XLangTokenType OperationType;
        /// <summary>
        ///     Right side of the Expression
        /// </summary>
        public readonly XLangExpression Right;

        /// <summary>
        ///     The Operation Function Implementation Cache
        /// </summary>
        private IXLangRuntimeFunction opCache;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="right">Right Side</param>
        public XLangBinaryOp(
            XLangContext context, XLangExpression left, XLangTokenType operationType,
            XLangExpression right) : base(context)
        {
            Left = left;
            OperationType = operationType;
            Right = right;
        }

        /// <summary>
        ///     Start index in source
        /// </summary>
        public override int StartIndex { get; }


        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>
            {
                Left,
                Right
            };
        }

        /// <summary>
        ///     Returns String representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return $"({Left} {OperationType} {Right})";
        }

        /// <summary>
        ///     Returns the Operator Implementation Function
        /// </summary>
        /// <param name="left">Left Side</param>
        /// <param name="right">Right Side</param>
        /// <param name="scope">Execution Scope</param>
        /// <returns></returns>
        private IXLangRuntimeFunction GetOperatorImpl(
            IXLangRuntimeTypeInstance left, IXLangRuntimeTypeInstance right, XLangRuntimeScope scope)
        {
            if (opCache != null)
            {
                return opCache;
            }
            if (Context.TryGetBinaryOperatorImplementation(
                left.Type,
                right.Type,
                OperationType,
                out IXLangRuntimeFunction impl
            ))
            {
                opCache = impl;
                return impl;
            }

            throw new XLangRuntimeTypeException(
                $"No operator implementation for operator: '{OperationType}' on types '{left?.Type.FullName}',  '{right?.Type.FullName}'"
            );
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
            IXLangRuntimeTypeInstance right = Right.Process(scope, instance);

            if (right is XLangFunctionAccessInstance aci && aci.Member.First() is IXLangRuntimeProperty prop)
            {
                right = prop.GetValue(aci.Instance);
            }

            return GetOperatorImpl(left, right, scope).Invoke(null, new[] {left, right});
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using XLang.Core;
using XLang.Queries;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

/// <summary>
/// Contains Special XLangExpression Implementations
/// </summary>
namespace XLang.Parser.Token.Expressions.Operators.Special
{
    /// <summary>
    ///     Array Accessor Operator Implementation
    /// </summary>
    public class XLangArrayAccessorOp : XLangExpression
    {
        /// <summary>
        ///     Left Side (the array)
        /// </summary>
        public readonly XLangExpression Left;
        /// <summary>
        ///     The Accessor Arguments
        /// </summary>
        public readonly XLangExpression[] ParameterList;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="list">Left Side Array</param>
        /// <param name="parameterList">Array Accessor Parameters</param>
        public XLangArrayAccessorOp(
            XLangContext context, XLangExpression list, List<XLangExpression> parameterList) : base(context)
        {
            Left = list;
            StartIndex = Left.StartIndex;
            ParameterList = parameterList.ToArray();
        }

        /// <summary>
        ///     The Operator Token
        /// </summary>
        public override XLangTokenType Type => XLangTokenType.OpArrayAccess;

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
            return new List<IXLangToken>();
        }

        /// <summary>
        ///     Returns the Operator Implementation Function
        /// </summary>
        /// <param name="left">Left Side</param>
        /// <returns></returns>
        private IXLangRuntimeFunction GetOperatorImpl(IXLangRuntimeTypeInstance left)
        {
            return left.Type.GetMembers(
                    XLangTokenType.OpArrayAccess.ToString(),
                    XLangBindingQuery.Private |
                    XLangBindingQuery.Static |
                    XLangBindingQuery.Override |
                    XLangBindingQuery.Operator
                ).Cast<IXLangRuntimeFunction>()
                .FirstOrDefault();
        }

        /// <summary>
        ///     Returns String representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return $"{Left.GetValue()}[]";
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
            return GetOperatorImpl(left).Invoke(left, ParameterList.Select(x => x.Process(scope, left)).ToArray());
        }
    }
}
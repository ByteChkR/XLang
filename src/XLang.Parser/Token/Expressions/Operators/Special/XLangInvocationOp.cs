using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLang.Core;
using XLang.Queries;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    /// <summary>
    ///     Invocation () Operator Implementation
    /// </summary>
    public class XLangInvocationOp : XLangExpression
    {
        /// <summary>
        ///     Left side Expression
        /// </summary>
        public readonly XLangExpression Left;
        /// <summary>
        ///     Invocation Arguments
        /// </summary>
        public readonly XLangExpression[] ParameterList;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side Expression</param>
        /// <param name="parameterList">Parameter list</param>
        public XLangInvocationOp(
            XLangContext context, XLangExpression left, XLangExpression[] parameterList) : base(context,
            left.SourceIndex)
        {
            Left = left;
            ParameterList = parameterList;
        }

        /// <summary>
        ///     Operation Type
        /// </summary>
        public override XLangTokenType Type => XLangTokenType.OpInvocation;


        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken> {Left};
        }


        /// <summary>
        ///     Returns the Operator Implementation Function
        /// </summary>
        /// <param name="left">Left Side</param>
        /// <returns></returns>
        private IXLangRuntimeFunction GetOperatorImpl(IXLangRuntimeTypeInstance left)
        {
            return left.Type.GetMembers(
                    XLangTokenType.OpInvocation.ToString(),
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
            StringBuilder sb = new StringBuilder($"{Left.GetValue()}(");
            for (int i = 0; i < ParameterList.Length; i++)
            {
                sb.Append(ParameterList[i].GetValue());
                if (i != ParameterList.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
            return sb.ToString();
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
            IXLangRuntimeFunction impl = GetOperatorImpl(left);
            IXLangRuntimeTypeInstance[] parameter = ParameterList.Select(x => x.Process(scope, instance)).ToArray();
            return impl.Invoke(left, parameter);
        }
    }
}
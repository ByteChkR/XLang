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
    public class XLangInvocationOp : XLangExpression
    {
        public readonly XLangExpression Left;
        public readonly XLangExpression[] ParameterList;
        public override XLangTokenType Type => XLangTokenType.OpInvocation;

        public XLangInvocationOp(
            XLangContext context, XLangExpression left, XLangExpression[] parameterList) : base(context)
        {
            Left = left;
            ParameterList = parameterList;
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken> {Left};
        }


        private IXLangRuntimeFunction GetOperatorImpl(IXLangRuntimeTypeInstance left)
        {
            return left.Type.GetMembers(
                    XLangTokenType.OpInvocation.ToString(),
                    XLangBindingQuery.Override |
                    XLangBindingQuery.Operator
                ).Cast<IXLangRuntimeFunction>()
                .FirstOrDefault();
        }

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

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            IXLangRuntimeTypeInstance left = Left.Process(scope, instance);
            IXLangRuntimeFunction impl = GetOperatorImpl(left);
            IXLangRuntimeTypeInstance[] parameter = ParameterList.Select(x => x.Process(scope, instance)).ToArray();
            return impl.Invoke(left, parameter);
        }
    }
}
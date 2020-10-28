using System.Collections.Generic;
using System.Linq;
using XLang.Core;
using XLang.Queries;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    public class XLangArrayAccessorOp : XLangExpression
    {
        public readonly XLangExpression Left;
        public readonly XLangExpression[] ParameterList;
        public override XLangTokenType Type => XLangTokenType.OpArrayAccess;

        public XLangArrayAccessorOp(
            XLangContext context, XLangExpression list, List<XLangExpression> parameterList) : base(context)
        {
            Left = list;
            StartIndex = Left.StartIndex;
            ParameterList = parameterList.ToArray();
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

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

        public override string GetValue()
        {
            return $"{Left.GetValue()}[]";
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            IXLangRuntimeTypeInstance left = Left.Process(scope, instance);
            return GetOperatorImpl(left).Invoke(left, ParameterList.Select(x => x.Process(scope, left)).ToArray());
        }
    }
}
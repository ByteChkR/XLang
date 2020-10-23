using System.Collections.Generic;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    public class XLangReturnOp : XLangExpression
    {
        private readonly XLangExpression right;

        public XLangReturnOp(XLangContext context, XLangExpression right) : base(context)
        {
            this.right = right;
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public override string GetValue()
        {
            return $"return {right}";
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            scope.SetReturn(right?.Process(scope, instance));
            return null;
        }
    }
}
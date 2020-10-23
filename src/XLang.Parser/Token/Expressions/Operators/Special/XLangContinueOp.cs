using System.Collections.Generic;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    public class XLangContinueOp : XLangExpression
    {
        public XLangContinueOp(XLangContext context) : base(context)
        {
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public override string GetValue()
        {
            return "continue";
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            scope.SetFlag(XLangRuntimeScope.ScopeFlags.Continue);
            return null;
        }
    }
}
using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions
{
    public abstract class XLangExpression : IXLangToken
    {
        protected readonly XLangContext Context;

        protected XLangExpression(XLangContext context)
        {
            Context = context;
        }

        public abstract int StartIndex { get; }

        public virtual XLangTokenType Type => XLangTokenType.OpExpression;

        public abstract List<IXLangToken> GetChildren();

        public abstract string GetValue();

        public abstract IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance);
    }
}
using System.Collections.Generic;

using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared;

namespace XLang.Parser.Token.Expressions.Operands
{
    public class XLangValueOperand : XLangExpression
    {

        public readonly IXLangToken Value;
        private object valueCache;

        public XLangValueOperand(XLangContext context, IXLangToken value) : base(context)
        {
            Value = value;
        }

        private XLangRuntimeType numberType => Context.GetType("XL.number");

        private XLangRuntimeType numTypeCache;

        private XLangRuntimeType stringType => Context.GetType("XL.string");
        private XLangRuntimeType stringTypeCache;

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken> { Value };
        }

        public override string GetValue()
        {
            return Value.ToString();
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            if (Value.Type == XLangTokenType.OpNumber)
            {
                if (numTypeCache == null) numTypeCache = numberType;
                if (valueCache != null) return new CSharpTypeInstance(numTypeCache, valueCache);
                return new CSharpTypeInstance(numTypeCache, valueCache = decimal.Parse(Value.GetValue()));
            }

            if (Value.Type == XLangTokenType.OpStringLiteral)
            {
                if (stringTypeCache == null) stringTypeCache = stringType;
                return new CSharpTypeInstance(stringTypeCache, Value.GetValue());
            }

            throw new XLangRuntimeTypeException("No Built in type found for " + Value.Type);
        }

    }
}
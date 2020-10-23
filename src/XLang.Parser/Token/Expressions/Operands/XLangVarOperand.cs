using System;
using System.Collections.Generic;
using XLang.Queries;
using XLang.Runtime;
using XLang.Runtime.Implementations;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operands
{
    public class XLangVarOperand : XLangExpression
    {
        protected XLangVarOperand(XLangContext context) : base(context)
        {
        }

        public XLangVarOperand(XLangContext context, IXLangToken value) : base(context)
        {
            Value = value;
        }

        public virtual IXLangToken Value { get; }


        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken> {Value};
        }

        public override string GetValue()
        {
            return Value.GetValue();
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            //string[] parts = Value.GetValue().Split('.');
            XLangRuntimeScope.XLangRuntimeScopedVar var = scope.ResolveVar(Value.GetValue());
            if (var != null)
            {

                return var.GetValue();


                //IXLangScopeAccess acVar =
                //    XLangRuntimeResolver.ResolveDynamicItem(Context, Value.GetValue(), var.GetValue().Type);
                //return new XLangFunctionTypeInstance((IXLangRuntimeMember)
                //                                     acVar, var.GetValue(), Context.GetType("XL.function"));
            }
            IXLangRuntimeItem ac =
                XLangRuntimeResolver.ResolveItem(scope, Value.GetValue(), null, scope.OwnerType);

            if (ac == null)
            {
                throw new Exception("Can not find Item: " + Value.GetValue());
            }

            return new XLangFunctionAccessInstance(ac, instance, Context.GetType("XL.function"));

        }
    }
}
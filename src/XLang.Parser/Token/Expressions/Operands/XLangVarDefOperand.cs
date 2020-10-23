using XLang.Parser.Token.Combined;
using XLang.Queries;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operands
{
    public class XLangVarDefOperand : XLangVarOperand
    {
        public readonly VariableDefinitionToken value;


        public XLangVarDefOperand(XLangContext context, VariableDefinitionToken value) : base(context)
        {
            this.value = value;
        }

        public override IXLangToken Value => value.Name;


        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            XLangRuntimeScope.XLangRuntimeScopedVar var = scope.Declare(
                value.Name.GetValue(),
                (XLangRuntimeType) XLangRuntimeResolver
                    .ResolveItem(
                        scope,
                        value
                            .TypeName.GetValue(),
                        null,
                        scope.OwnerType
                    )
            );
            if (value.InitializerExpression != null)
            {
                var.SetValue(value.InitializerExpression.Process(scope, instance));
            }


            return base.Process(scope, instance);
        }
    }
}
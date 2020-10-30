using System.Linq;
using XLang.Parser.Token.Combined;
using XLang.Queries;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operands
{
    /// <summary>
    ///     Implements a Variable Operand that is also a Variable Definition
    /// </summary>
    public class XLangVarDefOperand : XLangVarOperand
    {
        /// <summary>
        ///     The Definition Token
        /// </summary>
        public readonly VariableDefinitionToken value;


        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="value">Variable Value</param>
        public XLangVarDefOperand(XLangContext context, VariableDefinitionToken value) : base(context, value.SourceIndex)
        {
            this.value = value;
        }

        /// <summary>
        ///     The Variable Value
        /// </summary>
        public override IXLangToken Value => value.Name;


        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
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
                    ).First()
            );
            if (value.InitializerExpression != null)
            {
                var.SetValue(value.InitializerExpression.Process(scope, instance));
            }


            return base.Process(scope, instance);
        }
    }
}
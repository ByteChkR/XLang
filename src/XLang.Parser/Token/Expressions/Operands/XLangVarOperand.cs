using System;
using System.Collections.Generic;
using XLang.Exceptions;
using XLang.Queries;
using XLang.Runtime;
using XLang.Runtime.Implementations;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

/// <summary>
/// Contains XLangExpression Implementations for Operand Values.
/// </summary>
namespace XLang.Parser.Token.Expressions.Operands
{
    /// <summary>
    ///     Implements a Variable Operand
    /// </summary>
    public class XLangVarOperand : XLangExpression
    {
        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        protected XLangVarOperand(XLangContext context, int sourceIdx) : base(context, sourceIdx)
        {
        }

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="value">Variable Value</param>
        public XLangVarOperand(XLangContext context, IXLangToken value, int sourceIdx) : base(context, sourceIdx)
        {
            Value = value;
        }

        /// <summary>
        ///     The Token Value
        /// </summary>
        public virtual IXLangToken Value { get; }
        

        /// <summary>
        ///     Returns all Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken> {Value};
        }

        /// <summary>
        ///     Returns the String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return Value.GetValue();
        }


        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
           XLangRuntimeScope.XLangRuntimeScopedVar var = scope.ResolveVar(Value.GetValue());
            if (var != null)
            {

                return var.GetValue();
            }
            IXLangRuntimeItem[] ac =
                XLangRuntimeResolver.ResolveItem(scope, Value.GetValue(), null, scope.OwnerType);

            if (ac == null)
            {
                throw new XLangRuntimeTypeException("Can not find Item: " + Value.GetValue());
            }

            return new XLangFunctionAccessInstance(ac, instance, Context.GetType("XL.function"));

        }
    }
}
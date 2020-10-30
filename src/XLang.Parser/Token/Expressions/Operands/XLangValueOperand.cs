using System.Collections.Generic;
using XLang.Core;
using XLang.Exceptions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared;

namespace XLang.Parser.Token.Expressions.Operands
{
    /// <summary>
    ///     Implements a (terminal) value expression
    /// </summary>
    public class XLangValueOperand : XLangExpression
    {
        /// <summary>
        ///     The Value
        /// </summary>
        public readonly IXLangToken Value;

        /// <summary>
        ///     The cached type "XL.number"
        /// </summary>
        private XLangRuntimeType numTypeCache;

        /// <summary>
        ///     The cached type "XL.string"
        /// </summary>
        private XLangRuntimeType stringTypeCache;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">The XL Context</param>
        /// <param name="value">The Value of this Token</param>
        public XLangValueOperand(XLangContext context, IXLangToken value) : base(context, value.SourceIndex)
        {
            Value = value;
        }

        /// <summary>
        ///     Number Type Property
        /// </summary>
        private XLangRuntimeType numberType => Context.GetType("XL.number");


        /// <summary>
        ///     StringType Property
        /// </summary>
        private XLangRuntimeType stringType => Context.GetType("XL.string");

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
            return Value.ToString();
        }

        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            if (Value.Type == XLangTokenType.OpNumber)
            {
                if (numTypeCache == null)
                {
                    numTypeCache = numberType;
                }
                //if (valueCache != null) return new CSharpTypeInstance(numTypeCache, valueCache);
                return new CSharpTypeInstance(numTypeCache, decimal.Parse(Value.GetValue()));
            }

            if (Value.Type == XLangTokenType.OpStringLiteral)
            {
                if (stringTypeCache == null)
                {
                    stringTypeCache = stringType;
                }
                return new CSharpTypeInstance(stringTypeCache, Value.GetValue());
            }

            throw new XLangRuntimeTypeException("No Built in type found for " + Value.Type);
        }
    }
}
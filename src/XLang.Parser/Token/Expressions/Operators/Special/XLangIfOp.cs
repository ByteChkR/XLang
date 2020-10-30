using System;
using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    /// <summary>
    ///     If/Else Operator Implementation
    /// </summary>
    public class XLangIfOp : XLangExpression
    {
        /// <summary>
        ///     Condition Map
        /// </summary>
        private readonly List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)> conditionMap;

        /// <summary>
        ///     Else Branch Block
        /// </summary>
        private readonly Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> elseBranch;

        /// <summary>
        ///     Operation Type
        /// </summary>
        public readonly XLangTokenType OperationType = XLangTokenType.OpIf;


        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="conditionMap">Condition Map</param>
        /// <param name="elseBranch">Else Branch Block</param>
        public XLangIfOp(
            XLangContext context,
            List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)> conditionMap,
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> elseBranch, int sourceIdx) : base(context, sourceIdx)
        {
            this.conditionMap = conditionMap;
            this.elseBranch = elseBranch;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        /// <summary>
        ///     Returns String representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return $"({OperationType})";
        }

        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            for (int i = 0; i < conditionMap.Count; i++)
            {
                if ((decimal) conditionMap[i].Item1.Process(scope, instance).GetRaw() != 0)
                {
                    conditionMap[i].Item2(scope, instance);
                    return null;
                }
            }

            elseBranch?.Invoke(scope, instance);
            return null;
        }
    }
}
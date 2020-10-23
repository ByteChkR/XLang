using System;
using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    public class XLangIfOp : XLangExpression
    {
        private readonly List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)> conditionMap;

        private readonly Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> elseBranch;

        public readonly XLangTokenType OperationType;

        public XLangIfOp(
            XLangContext context, XLangTokenType operationType,
            List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)> conditionMap,
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> elseBranch) : base(context)
        {
            this.conditionMap = conditionMap;
            this.elseBranch = elseBranch;
            OperationType = operationType;
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public override string GetValue()
        {
            return $"({OperationType})";
        }

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
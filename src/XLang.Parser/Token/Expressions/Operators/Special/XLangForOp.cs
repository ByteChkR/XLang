using System;
using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    public class XLangForOp : XLangExpression
    {
        public readonly XLangExpression Condition;
        private readonly Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> ExprBody;
        public readonly XLangTokenType OperationType;


        public readonly XLangExpression VDecl;
        public readonly XLangExpression VInc;

        public XLangForOp(
            XLangContext context, XLangExpression vDecl, XLangExpression condition, XLangExpression vInc,
            XLangTokenType operationType,
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> exprBody) : base(context)
        {
            Condition = condition;
            VDecl = vDecl;
            VInc = vInc;
            OperationType = operationType;
            ExprBody = exprBody;
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public override string GetValue()
        {
            return $"({OperationType} ({Condition}) {ExprBody})";
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            VDecl.Process(scope, instance);
            IXLangRuntimeTypeInstance condReturn = Condition.Process(scope, instance);
            while ((decimal) condReturn.GetRaw() != 0)
            {
                if (!scope.Check(XLangRuntimeScope.ScopeFlags.Continue))
                {
                    ExprBody(scope, instance);
                    if (scope.Check(XLangRuntimeScope.ScopeFlags.Break | XLangRuntimeScope.ScopeFlags.Return))
                    {
                        break;
                    }
                }
                else
                {
                    scope.ClearFlag(XLangRuntimeScope.ScopeFlags.Continue);
                }

                VInc.Process(scope, instance);
                condReturn = Condition.Process(scope, instance);
            }

            scope.ClearFlag(XLangRuntimeScope.ScopeFlags.Break);
            return null;
        }
    }
}
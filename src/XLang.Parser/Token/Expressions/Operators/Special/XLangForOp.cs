using System;
using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    /// <summary>
    ///     For Operator Implementation
    /// </summary>
    public class XLangForOp : XLangExpression
    {
        /// <summary>
        ///     Continue Condition
        /// </summary>
        private readonly XLangExpression Condition;

        /// <summary>
        ///     The Expression Body
        /// </summary>
        private readonly Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> ExprBody;

        /// <summary>
        ///     Expression Body
        /// </summary>
        private readonly XLangTokenType OperationType = XLangTokenType.OpFor;


        /// <summary>
        ///     Variable Declaration
        /// </summary>
        private readonly XLangExpression VDecl;

        /// <summary>
        ///     Variable Change Expression
        /// </summary>
        private readonly XLangExpression VInc;


        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="vDecl">Variable Declaration</param>
        /// <param name="condition">For Continue Condition</param>
        /// <param name="vInc">Variable Change Expression</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="exprBody">The Expression Body</param>
        public XLangForOp(
            XLangContext context, XLangExpression vDecl, XLangExpression condition, XLangExpression vInc,
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> exprBody, int sourceIdx) : base(context, sourceIdx)
        {
            Condition = condition;
            VDecl = vDecl;
            VInc = vInc;
            ExprBody = exprBody;
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
            return $"for ({OperationType} ({Condition}) {ExprBody})";
        }

        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            XLangRuntimeScope lScope = scope.Clone();
            VDecl.Process(lScope, instance);
            IXLangRuntimeTypeInstance condReturn = Condition.Process(lScope, instance);
            while ((decimal) condReturn.GetRaw() != 0)
            {
                if (!lScope.Check(XLangRuntimeScope.ScopeFlags.Continue))
                {
                    ExprBody(lScope, instance);
                    if (lScope.Check(XLangRuntimeScope.ScopeFlags.Break))
                    {
                        break;
                    }
                    if (lScope.Check(XLangRuntimeScope.ScopeFlags.Return))
                    {
                        scope.SetReturn(lScope.Return);
                        break;
                    }
                }
                else
                {
                    lScope.ClearFlag(XLangRuntimeScope.ScopeFlags.Continue);
                }

                VInc.Process(lScope, instance);
                condReturn = Condition.Process(lScope, instance);
            }

            lScope.ClearFlag(XLangRuntimeScope.ScopeFlags.Break);
            return null;
        }
    }
}
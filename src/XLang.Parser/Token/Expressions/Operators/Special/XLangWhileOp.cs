﻿using System;
using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    /// <summary>
    ///     While Operator Implementation
    /// </summary>
    public class XLangWhileOp : XLangExpression
    {
        /// <summary>
        ///     Continue Expression
        /// </summary>
        private readonly XLangExpression Condition;

        /// <summary>
        ///     Expression Body
        /// </summary>
        private readonly Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> ExprBody;

        /// <summary>
        ///     Operation Type
        /// </summary>
        private readonly XLangTokenType OperationType = XLangTokenType.OpWhile;


        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="condition">Continue Condition</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="exprBody">Expression Body</param>
        public XLangWhileOp(
            XLangContext context, XLangExpression condition,
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> exprBody, int sourceIdx) : base(context, sourceIdx)
        {
            Condition = condition;
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
            return $"({OperationType} ({Condition}) {ExprBody})";
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
            IXLangRuntimeTypeInstance condReturn = Condition.Process(lScope, instance);
            while ((decimal) condReturn.GetRaw() != 0)
            {
                if (!lScope.Check(XLangRuntimeScope.ScopeFlags.Continue))
                {
                    ExprBody(lScope, instance);
                    if (lScope.Check(XLangRuntimeScope.ScopeFlags.Break | XLangRuntimeScope.ScopeFlags.Return))
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

                condReturn = Condition.Process(lScope, instance);
            }

            lScope.ClearFlag(XLangRuntimeScope.ScopeFlags.Break);
            
            return null;
        }
    }
}
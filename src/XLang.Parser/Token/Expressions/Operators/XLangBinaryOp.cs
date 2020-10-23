using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared;

namespace XLang.Parser.Token.Expressions.Operators
{
    public class XLangBinaryOp : XLangExpression
    {
        public readonly XLangExpression Left;
        public readonly XLangTokenType OperationType;
        public readonly XLangExpression Right;

        private IXLangRuntimeFunction opCache;

        public XLangBinaryOp(
            XLangContext context, XLangExpression left, XLangTokenType operationType,
            XLangExpression right) : base(context)
        {
            Left = left;
            OperationType = operationType;
            Right = right;
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>
            {
                Left,
                Right
            };
        }

        public override string GetValue()
        {
            return $"({Left} {OperationType} {Right})";
        }

        private IXLangRuntimeFunction GetOperatorImpl(
            IXLangRuntimeTypeInstance left, IXLangRuntimeTypeInstance right, XLangRuntimeScope scope)
        {
            if (opCache != null)
            {
                return opCache;
            }
            if (Context.TryGetBinaryOperatorImplementation(
                left.Type,
                right.Type,
                OperationType,
                out IXLangRuntimeFunction impl
            ))
            {
                opCache = impl;
                return impl;
            }

            throw new XLangRuntimeTypeException(
                $"No operator implementation for operator: '{OperationType}' on types '{left?.Type.FullName}',  '{right?.Type.FullName}'"
            );
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            IXLangRuntimeTypeInstance left = Left.Process(scope, instance);
            IXLangRuntimeTypeInstance right = Right.Process(scope, instance);
            return GetOperatorImpl(left, right, scope).Invoke(null, new[] {left, right});
        }
    }
}
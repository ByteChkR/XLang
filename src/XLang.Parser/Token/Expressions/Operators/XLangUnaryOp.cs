using System.Collections.Generic;

using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators
{
    public class XLangUnaryOp : XLangExpression
    {

        public readonly XLangExpression Left;
        public readonly XLangTokenType OperationType;


        public XLangUnaryOp(XLangContext context, XLangExpression left, XLangTokenType operationType) : base(context)
        {
            Left = left;
            OperationType = operationType;
        }

        public override int StartIndex => Left.StartIndex;

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken> { Left };
        }

        public override string GetValue()
        {
            return $"{OperationType}({Left.GetValue()})";
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {
            IXLangRuntimeTypeInstance left = Left.Process(scope, instance);

            if (OperationType == XLangTokenType.OpNew)
            {
                return left;
            }

            return Context.GetUnaryOperatorImplementation(left.Type, OperationType).Invoke(null, new[] { left });
        }

    }
}
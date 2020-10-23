using System;
using System.Collections.Generic;
using XLang.Queries;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    public class XLangMemberAccessOp : XLangExpression
    {
        public readonly XLangExpression Left;
        public readonly string MemberName;

        public XLangMemberAccessOp(XLangContext context, XLangExpression left, string memberName) : base(context)
        {
            Left = left;
            MemberName = memberName;
        }

        public override int StartIndex { get; }

        public override List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public override string GetValue()
        {
            return ToString();
        }

        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {

            IXLangRuntimeTypeInstance inst = Left.Process(scope, instance); //TODO: Return Member of Return Instance
            if (inst is XLangFunctionAccessInstance acI)
            {
                if (acI.Member is IXLangRuntimeProperty prop)
                {

                    return new XLangFunctionAccessInstance(
                        prop.PropertyType.GetMember(MemberName),
                        acI.Instance,
                        Context.GetType("XL.function")
                    );
                }

                if (acI.Member is XLangRuntimeType type)
                {
                    return new XLangFunctionAccessInstance(
                        type.GetMember(MemberName),
                        acI.Instance,
                        Context.GetType("XL.function")
                    );
                }

                throw new Exception("Invalid Access");
            }
            return new XLangFunctionAccessInstance(
                (IXLangRuntimeMember) XLangRuntimeResolver.ResolveItem(
                    scope,
                    MemberName,
                    inst.Type,
                    scope
                        .OwnerType
                ),
                inst,
                Context.GetType("XL.function")
            );
            //Can Either be Type or Instance
        }
    }
}
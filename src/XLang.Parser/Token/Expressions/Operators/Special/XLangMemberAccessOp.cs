using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
                if (acI.Member.All(x => x is IXLangRuntimeProperty))
                {
                    IXLangRuntimeProperty prop = (IXLangRuntimeProperty)acI.Member.First();
                    return new XLangFunctionAccessInstance(
                        prop.PropertyType.GetMembers(MemberName),
                        prop.GetValue(acI.Instance),
                        Context.GetType("XL.function")
                    );
                }

                if (acI.Member.First() is XLangRuntimeType type)
                {
                    return new XLangFunctionAccessInstance(
                        type.GetMembers(MemberName),
                        acI.Instance,
                        Context.GetType("XL.function")
                    );
                }

                throw new Exception("Invalid Access");
            }

            IXLangRuntimeMember[] rm = XLangRuntimeResolver.ResolveItem(
                scope,
                MemberName,
                inst.Type,
                scope
                    .OwnerType
            ).Cast<IXLangRuntimeMember>().ToArray();

            if (rm == null)
                throw new Exception("Invalid Access.");

            return new XLangFunctionAccessInstance(
                rm,
                inst,
                Context.GetType("XL.function")
            );
            //Can Either be Type or Instance
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using XLang.Queries;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Token.Expressions.Operators.Special
{
    /// <summary>
    ///     Member Access . Operator Implementation
    /// </summary>
    public class XLangMemberAccessOp : XLangExpression
    {
        /// <summary>
        ///     Left Side expression
        /// </summary>
        public readonly XLangExpression Left;
        /// <summary>
        ///     Name of the Member that is beeing accessed.
        /// </summary>
        public readonly string MemberName;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side Expression</param>
        /// <param name="memberName"></param>
        public XLangMemberAccessOp(XLangContext context, XLangExpression left, string memberName) : base(context, left.SourceIndex)
        {
            Left = left;
            MemberName = memberName;
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
            return ToString();
        }

        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">Expression Type Instance</param>
        /// <returns></returns>
        public override IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
        {

            IXLangRuntimeTypeInstance inst = Left.Process(scope, instance); //TODO: Return Member of Return Instance
            if (inst is XLangFunctionAccessInstance acI)
            {
                if (acI.Member.All(x => x is IXLangRuntimeProperty))
                {
                    IXLangRuntimeProperty prop = (IXLangRuntimeProperty) acI.Member.First();
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
            {
                throw new Exception("Invalid Access.");
            }

            return new XLangFunctionAccessInstance(
                rm,
                inst,
                Context.GetType("XL.function")
            );
            //Can Either be Type or Instance
        }
    }
}
using System.Linq;
using XLang.Core;
using XLang.Exceptions;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.BaseTypes
{
    /// <summary>
    ///     Implements "XL.function"
    /// </summary>
    public class XLangFunctionType
    {
        /// <summary>
        ///     The Containing Namespace
        /// </summary>
        private readonly XLCoreNamespace containingNamespace;


        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="core">Core Namespace</param>
        public XLangFunctionType(XLCoreNamespace core)
        {
            containingNamespace = core;
        }


        /// <summary>
        ///     Creates the Runtime Type
        /// </summary>
        /// <param name="objectType">Type "XL.object"</param>
        /// <returns>"XL.function" type</returns>
        public XLangRuntimeType GetObject(XLangRuntimeType objectType)
        {
            XLangRuntimeType functionType = new XLangRuntimeType(
                "function",
                containingNamespace,
                objectType,
                XLangBindingFlags.Instance | XLangBindingFlags.Public
            );

            DelegateXLFunction funcInvocation =
                new DelegateXLFunction(
                    XLangTokenType.OpInvocation.ToString(),
                    InvokeFunction,
                    objectType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    functionType
                );


            functionType.SetMembers(new IXLangRuntimeMember[] {funcInvocation});
            return functionType;
        }

        /// <summary>
        ///     Invocation Operator override for XL.function
        /// </summary>
        /// <param name="instance">Function Instance</param>
        /// <param name="args">Function Arguments</param>
        /// <returns>Function Return</returns>
        private IXLangRuntimeTypeInstance InvokeFunction(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangFunctionAccessInstance ts = (XLangFunctionAccessInstance) instance;
            if (ts.Member.All(x => x is IXLangRuntimeFunction))
            {
                return ts.Member.Cast<IXLangRuntimeFunction>().First(x => x.ParameterList.Length == args.Length)
                    .Invoke(ts.Instance, args);
            }
            if (ts.Member.First() is XLangRuntimeType type)
            {
                IXLangRuntimeTypeInstance newItem = type.CreateEmptyBase();
                ((IXLangRuntimeFunction) type.GetMember(XLangBindingQuery.Constructor)).Invoke(
                    newItem,
                    args
                );
                return newItem;
            }

            throw new XLangRuntimeTypeException("Invocation Failure");
        }
    }
}
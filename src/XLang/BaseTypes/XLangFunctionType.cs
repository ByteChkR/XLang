using System;

using XLang.Core;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.BaseTypes
{
    public class XLangFunctionType
    {

        private readonly XLCoreNamespace containingNamespace;


        public XLangFunctionType(XLCoreNamespace core)
        {
            containingNamespace = core;
        }

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
                                       XLangMemberFlags.Override
                                      );


            functionType.SetMembers(new IXLangRuntimeMember[] { funcInvocation });
            return functionType;
        }

        private IXLangRuntimeTypeInstance InvokeFunction(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangFunctionAccessInstance ts = (XLangFunctionAccessInstance)instance;
            if (ts.Member is IXLangRuntimeFunction func)
            {
                return func.Invoke(ts.Instance, args);
            }
            if (ts.Member is XLangRuntimeType type)
            {
                IXLangRuntimeTypeInstance newItem = type.CreateEmptyBase();
                ((IXLangRuntimeFunction) type.GetMember(XLangBindingQuery.Constructor)).Invoke(
                                                                                               newItem,
                                                                                               args
                                                                                              );
                return newItem;
            }

            throw new Exception("Invocation Failure");
        }

    }
}
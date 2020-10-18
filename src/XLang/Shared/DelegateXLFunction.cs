using System;

using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Shared
{
    public class DelegateXLFunction : IXLangRuntimeFunction
    {

        private readonly Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance[], IXLangRuntimeTypeInstance>
            onInvoke;

        public DelegateXLFunction(
            string name, Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance[], IXLangRuntimeTypeInstance> invoke,
            XLangRuntimeType returnType, XLangMemberFlags flags, params IXLangRuntimeFunctionArgument[] parameters)
        {
            Name = name;
            ReturnType = returnType;
            ParameterList = parameters;
            onInvoke = invoke;
            BindingFlags = flags;
        }

        public XLangMemberFlags BindingFlags { get; set; }

        public XLangMemberType ItemType => XLangMemberType.Function;

        public string Name { get; }

        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        public XLangRuntimeType ImplementingClass { get; }

        public XLangMemberType MemberType => XLangMemberType.Function;

        public XLangRuntimeType ReturnType { get; }

        public IXLangRuntimeFunctionArgument[] ParameterList { get; }

        public IXLangRuntimeTypeInstance Invoke(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] arguments)
        {
            return onInvoke(instance, arguments);
        }

    }
}
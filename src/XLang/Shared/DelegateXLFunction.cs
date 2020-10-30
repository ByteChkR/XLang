using System;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Shared
{
    /// <summary>
    ///     Function Implementation that allows implementing Functions with delegates in c#
    /// </summary>
    public class DelegateXLFunction : IXLangRuntimeFunction
    {
        /// <summary>
        ///     Function Delegate
        /// </summary>
        private readonly Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance[], IXLangRuntimeTypeInstance>
            onInvoke;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Function Name</param>
        /// <param name="invoke">Function Delegate</param>
        /// <param name="returnType">Return type of this Function</param>
        /// <param name="flags">Binding Flags</param>
        /// <param name="implementingClass">The Implementing class of this function</param>
        /// <param name="parameters">The Function Arguments.</param>
        public DelegateXLFunction(
            string name, Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance[], IXLangRuntimeTypeInstance> invoke,
            XLangRuntimeType returnType, XLangMemberFlags flags, XLangRuntimeType implementingClass,
            params IXLangRuntimeFunctionArgument[] parameters)
        {
            ImplementingClass = implementingClass;
            Name = name;
            ReturnType = returnType;
            ParameterList = parameters;
            onInvoke = invoke;
            BindingFlags = flags;
        }

        /// <summary>
        ///     Binding Flags of this Function
        /// </summary>
        public XLangMemberFlags BindingFlags { get; set; }

        /// <summary>
        ///     The Item Type (Function)
        /// </summary>
        public XLangMemberType ItemType => XLangMemberType.Function;

        /// <summary>
        ///     The Function Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The Accessibility Level of this Function
        /// </summary>
        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        /// <summary>
        ///     Binding Flags of this Item
        /// </summary>
        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        /// <summary>
        ///     The implementing class of this function
        /// </summary>
        public XLangRuntimeType ImplementingClass { get; }


        /// <summary>
        ///     The Return type of this function
        /// </summary>
        public XLangRuntimeType ReturnType { get; }

        /// <summary>
        ///     Function Arguments
        /// </summary>
        public IXLangRuntimeFunctionArgument[] ParameterList { get; }

        /// <summary>
        ///     Invokes this Function
        /// </summary>
        /// <param name="instance">Implementing Class instance. Null for Static Functions</param>
        /// <param name="arguments">Function Arguments</param>
        /// <returns></returns>
        public IXLangRuntimeTypeInstance Invoke(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] arguments)
        {
            return onInvoke(instance, arguments);
        }

        /// <summary>
        ///     To String Implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ImplementingClass.FullName + "." + Name;
        }
    }
}
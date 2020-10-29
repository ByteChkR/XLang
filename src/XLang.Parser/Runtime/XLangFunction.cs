using System.Linq;
using XLang.Parser.Token.Expressions;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

/// <summary>
/// Contains Runtime Implementations for Executable Runtime Elements
/// </summary>
namespace XLang.Parser.Runtime
{
    /// <summary>
    ///     XLang Runtime Function Implementation
    /// </summary>
    public class XLangFunction : IXLangRuntimeFunction
    {
        /// <summary>
        ///     Function Body Expressions
        /// </summary>
        private readonly XLangExpression[] Block;

        /// <summary>
        ///     XL Context
        /// </summary>
        private readonly XLangContext context;

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="name">Function Name</param>
        /// <param name="returnType">Return Type</param>
        /// <param name="implementingClass">Implementing Class Type</param>
        /// <param name="parameterList">Parameters</param>
        /// <param name="flags">Binding Flags</param>
        /// <param name="block">Function Body</param>
        /// <param name="context">XL Context</param>
        public XLangFunction(
            string name, XLangRuntimeType returnType, XLangRuntimeType implementingClass,
            IXLangRuntimeFunctionArgument[] parameterList, XLangMemberFlags flags, XLangExpression[] block,
            XLangContext context)
        {
            Name = name;
            ReturnType = returnType;
            ImplementingClass = implementingClass;
            ParameterList = parameterList;
            BindingFlags = flags;
            Block = block;
            this.context = context;
        }


        /// <summary>
        ///     The Binding Flags
        /// </summary>
        public XLangMemberFlags BindingFlags { get; }


        /// <summary>
        ///     The Item Type
        /// </summary>
        public XLangMemberType ItemType =>
            (BindingFlags & XLangMemberFlags.Constructor) != 0 ? XLangMemberType.Constructor : XLangMemberType.Function;

        /// <summary>
        ///     The Function Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The Accessibilty Level
        /// </summary>
        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        /// <summary>
        ///     The Binding Flags
        /// </summary>
        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        /// <summary>
        ///     The Implementing Class Type
        /// </summary>
        public XLangRuntimeType ImplementingClass { get; }

        /// <summary>
        ///     The Function Return Type
        /// </summary>
        public XLangRuntimeType ReturnType { get; }

        /// <summary>
        ///     The Parameters for this Function
        /// </summary>
        public IXLangRuntimeFunctionArgument[] ParameterList { get; }

        /// <summary>
        ///     Invokes this Function
        /// </summary>
        /// <param name="instance">Instance of Implementing Type(null for static)</param>
        /// <param name="arguments">Function Arguments</param>
        /// <returns>Return of the Function</returns>
        public IXLangRuntimeTypeInstance Invoke(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] arguments)
        {
            XLangRuntimeScope scope = new XLangRuntimeScope(context, this);

            for (int i = 0; i < ParameterList.Length; i++)
            {
                IXLangRuntimeFunctionArgument xLangRuntimeFunctionArgument = ParameterList[i];
                scope.Declare(xLangRuntimeFunctionArgument.Name, xLangRuntimeFunctionArgument.Type)
                    .SetValue(arguments[i]);
            }

            ImplementingClass.AddStatics(scope);
            instance?.AddLocals(scope);


            for (int i = 0; i < Block.Length; i++)
            {
                Block[i].Process(scope, instance);
                if (scope.Check(XLangRuntimeScope.ScopeFlags.Return))
                {
                    return scope.Return;
                }
            }

            return scope.Return;
        }

        /// <summary>
        ///     Returns the Expression Block
        /// </summary>
        /// <returns></returns>
        public XLangExpression[] InspectBlock()
        {
            return Block.ToArray();
        }
    }
}
using System.Linq;

using XLang.Parser.Token.Expressions;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Runtime
{
    public class XLangFunction : IXLangRuntimeFunction
    {

        private readonly XLangExpression[] Block;

        private readonly XLangContext context;

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

        public XLangMemberFlags BindingFlags { get; }

        public XLangMemberType ItemType =>
            (BindingFlags & XLangMemberFlags.Constructor) != 0 ? XLangMemberType.Constructor : XLangMemberType.Function;

        public string Name { get; }

        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        public XLangRuntimeType ImplementingClass { get; }

        public XLangMemberType MemberType =>
            (BindingFlags & XLangMemberFlags.Constructor) != 0 ? XLangMemberType.Constructor : XLangMemberType.Function;

        public XLangRuntimeType ReturnType { get; }

        public IXLangRuntimeFunctionArgument[] ParameterList { get; }

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
                    return scope.Return;
            }

            return scope.Return;
        }

        public XLangExpression[] InspectBlock()
        {
            return Block.ToArray();
        }

    }
}
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.BaseTypes
{
    public class XLangStringType
    {
        private readonly XLCoreNamespace containingNamespace;

        public XLangStringType(XLCoreNamespace core)
        {
            containingNamespace = core;
        }


        public static XLangRuntimeType CreateConsole(XLCoreNamespace ns, XLangRuntimeType voidType,
            XLangRuntimeType stringType)
        {
            XLangRuntimeType cmdType = new XLangRuntimeType(
                "Console",
                ns,
                voidType,
                XLangBindingFlags.Static | XLangBindingFlags.Public
            );
            DelegateXLFunction printLnFunc = new DelegateXLFunction("WriteLine", (x, y) => PrintLnImpl(ns, x, y),
                stringType, XLangMemberFlags.Public | XLangMemberFlags.Static,
                new XLangFunctionArgument("message", stringType));

            cmdType.SetMembers(new[] {printLnFunc});
            return cmdType;
        }

        private static IXLangRuntimeTypeInstance PrintLnImpl(XLCoreNamespace ns, IXLangRuntimeTypeInstance instance,
            IXLangRuntimeTypeInstance[] parameters)
        {
            ns.InvokeWriteLine(parameters[0].GetRaw().ToString());
            return new CSharpTypeInstance(
                ns.GetType(
                    "string",
                    XLangBindingQuery.Public |
                    XLangBindingQuery.Instance
                ),
                parameters[0].GetRaw().ToString()
            );
        }

        private IXLangRuntimeTypeInstance ObjectToString(
            IXLangRuntimeTypeInstance arg1, IXLangRuntimeTypeInstance[] arg2)
        {
            return new CSharpTypeInstance(
                containingNamespace.GetType(
                    "string",
                    XLangBindingQuery.Public |
                    XLangBindingQuery.Instance
                ),
                arg1.GetRaw().ToString()
            );
        }

        private IXLangRuntimeTypeInstance GetStringLengthProperty(IXLangRuntimeTypeInstance instance)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public | XLangBindingQuery.Instance
            );
            return new CSharpTypeInstance(type, instance.GetRaw().ToString().Length);
        }

        private IXLangRuntimeTypeInstance GetStringLength(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public | XLangBindingQuery.Instance
            );
            return new CSharpTypeInstance(type, instance.GetRaw().ToString().Length);
        }

        public XLangRuntimeType GetObject(XLangRuntimeType objectType, XLangRuntimeType numberType)
        {
            XLangRuntimeType stringType = new XLangRuntimeType(
                "string",
                containingNamespace,
                objectType,
                XLangBindingFlags.Instance | XLangBindingFlags.Public,
                x => new CSharpTypeInstance(x, "")
            );


            DelegateXLFunction stringLengthFunction =
                new DelegateXLFunction(
                    "GetLength",
                    GetStringLength,
                    numberType,
                    XLangMemberFlags.Instance | XLangMemberFlags.Public
                );
            DelegateXLProperty stringLengthProperty =
                new DelegateXLProperty(
                    "Length",
                    GetStringLengthProperty,
                    numberType,
                    XLangMemberFlags.Public | XLangMemberFlags.Instance
                );
            DelegateXLFunction toString = new DelegateXLFunction(
                "ToString",
                ObjectToString,
                stringType,
                XLangMemberFlags.Public | XLangMemberFlags.Instance
            );

            stringType.SetMembers(new IXLangRuntimeMember[] {stringLengthFunction, stringLengthProperty});
            objectType.InjectMember(toString);
            return stringType;
        }
    }
}
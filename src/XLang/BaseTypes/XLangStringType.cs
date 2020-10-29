using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.BaseTypes
{
    /// <summary>
    ///     Implements XL.string
    /// </summary>
    public class XLangStringType
    {
        /// <summary>
        ///     Core Namespace
        /// </summary>
        private readonly XLCoreNamespace containingNamespace;

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="core">Core Namespace</param>
        public XLangStringType(XLCoreNamespace core)
        {
            containingNamespace = core;
        }


        /// <summary>
        ///     Creates the Console Type for Writing to console.
        /// </summary>
        /// <param name="ns">Core Namespace</param>
        /// <param name="voidType">Void Type ("XL.void")</param>
        /// <param name="stringType">String Type ("XL.string")</param>
        /// <returns>Console Type</returns>
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
                cmdType,
                new XLangFunctionArgument("message", stringType));

            cmdType.SetMembers(new[] {printLnFunc});
            return cmdType;
        }


        /// <summary>
        ///     Implements Console.WriteLine
        /// </summary>
        /// <param name="ns">Core Namespace</param>
        /// <param name="instance">Console Instance(null)</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Function Return(null)</returns>
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

        /// <summary>
        ///     XL.object.ToString() implementation
        /// </summary>
        /// <param name="arg1">Object Instance</param>
        /// <param name="arg2">Arguments</param>
        /// <returns>String representation of the instance.</returns>
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

        /// <summary>
        ///     XL.string.Length Property
        /// </summary>
        /// <param name="instance">String Instance</param>
        /// <returns>String Length</returns>
        private IXLangRuntimeTypeInstance GetStringLengthProperty(IXLangRuntimeTypeInstance instance)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public | XLangBindingQuery.Instance
            );
            return new CSharpTypeInstance(type, instance.GetRaw().ToString().Length);
        }

        /// <summary>
        ///     XL.string.GetLength()
        /// </summary>
        /// <param name="instance">String Instance</param>
        /// <param name="args">Arguments</param>
        /// <returns>String Length</returns>
        private IXLangRuntimeTypeInstance GetStringLength(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public | XLangBindingQuery.Instance
            );
            return new CSharpTypeInstance(type, instance.GetRaw().ToString().Length);
        }

        /// <summary>
        ///     Creates the Runtime Type
        /// </summary>
        /// <param name="objectType">Type "XL.object"</param>
        /// <param name="numberType">Type "XL.number"</param>
        /// <returns>"XL.string" type</returns>
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
                    XLangMemberFlags.Instance | XLangMemberFlags.Public,
                    stringType
                );
            DelegateXLProperty stringLengthProperty =
                new DelegateXLProperty(
                    "Length",
                    GetStringLengthProperty,
                    numberType,
                    XLangMemberFlags.Public | XLangMemberFlags.Instance,
                    stringType
                );
            DelegateXLFunction toString = new DelegateXLFunction(
                "ToString",
                ObjectToString,
                stringType,
                XLangMemberFlags.Public | XLangMemberFlags.Instance,
                objectType
            );

            stringType.SetMembers(new IXLangRuntimeMember[] {stringLengthFunction, stringLengthProperty});
            objectType.InjectMember(toString);
            return stringType;
        }
    }
}
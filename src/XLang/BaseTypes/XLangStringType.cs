using XLang.Core;
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

            cmdType.SetMembers(new[] { printLnFunc });
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
            IXLangRuntimeTypeInstance arg1)
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

        private IXLangRuntimeTypeInstance StringConcat(XLangRuntimeType rt, IXLangRuntimeTypeInstance[] args)
        {
            return new CSharpTypeInstance(rt, args[0].GetRaw().ToString() + args[1].GetRaw());
        }

        private IXLangRuntimeTypeInstance StringAssignConcat(IXLangRuntimeTypeInstance[] args)
        {
            args[0].SetRaw(args[0].Type, args[0].GetRaw().ToString() + args[1].GetRaw());
            return args[0];
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
            return new CSharpTypeInstance(type, (decimal)instance.GetRaw().ToString().Length);
        }

        private IXLangRuntimeTypeInstance SubStrStartCount(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            int start = (int)(decimal)args[0].GetRaw();
            int count = (int)(decimal)args[1].GetRaw();
            return new CSharpTypeInstance(rt, instance.GetRaw().ToString().Substring(start, count));
        }

        private IXLangRuntimeTypeInstance SubStrStart(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            int start = (int)(decimal)args[0].GetRaw();
            return new CSharpTypeInstance(rt, instance.GetRaw().ToString().Substring(start));
        }

        private IXLangRuntimeTypeInstance StrContains(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public | XLangBindingQuery.Instance
                                                               );
            bool contains = instance.GetRaw().ToString().Contains(args[0].GetRaw().ToString());
            return new CSharpTypeInstance(type, (decimal)(contains ? 1 : 0));
        }



        private IXLangRuntimeTypeInstance StrRemoveStartCount(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            int start = (int)(decimal)args[0].GetRaw();
            int count = (int)(decimal)args[1].GetRaw();
            return new CSharpTypeInstance(rt, instance.GetRaw().ToString().Remove(start, count));
        }

        private IXLangRuntimeTypeInstance StrRemoveStart(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            int start = (int)(decimal)args[0].GetRaw();
            return new CSharpTypeInstance(rt, instance.GetRaw().ToString().Remove(start));
        }

        private IXLangRuntimeTypeInstance StrTrim(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance)
        {
            string contains = instance.GetRaw().ToString().Trim();
            return new CSharpTypeInstance(rt, contains);
        }
        private IXLangRuntimeTypeInstance StrTrimStart(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance)
        {
            string contains = instance.GetRaw().ToString().TrimStart();
            return new CSharpTypeInstance(rt, contains);
        }
        private IXLangRuntimeTypeInstance StrTrimEnd(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance)
        {
            string contains = instance.GetRaw().ToString().TrimEnd();
            return new CSharpTypeInstance(rt, contains);
        }
        private IXLangRuntimeTypeInstance StrReplace(XLangRuntimeType rt, IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            string contains = instance.GetRaw().ToString().Replace(args[0].GetRaw().ToString(), args[1].GetRaw().ToString());
            return new CSharpTypeInstance(rt, contains);
        }


        /// <summary>
        ///     XL.string.GetLength()
        /// </summary>
        /// <param name="instance">String Instance</param>
        /// <param name="args">Arguments</param>
        /// <returns>String Length</returns>
        private IXLangRuntimeTypeInstance GetStringLength(
            IXLangRuntimeTypeInstance instance)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public | XLangBindingQuery.Instance
            );
            return new CSharpTypeInstance(type, (decimal)instance.GetRaw().ToString().Length);
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
                    (instance, args) => GetStringLength(instance),
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
                                                                 (instance, args) => ObjectToString(instance),
                                                                 stringType,
                                                                 XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                 objectType
                                                                );

            DelegateXLFunction subStr = new DelegateXLFunction(
                                                               "Substring",
                                                               (instance, args) => SubStrStartCount(stringType, instance, args),
                                                               stringType,
                                                               XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                               stringType,
                                                               new XLangFunctionArgument("start", numberType),
                                                               new XLangFunctionArgument("count", numberType)
                                                              );
            DelegateXLFunction subStrS = new DelegateXLFunction(
                                                                "Substring",
                                                                (instance, args) => SubStrStart(stringType, instance, args),
                                                                stringType,
                                                                XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                stringType,
                                                                new XLangFunctionArgument("start", numberType)
                                                               );
            DelegateXLFunction remStr = new DelegateXLFunction(
                                                               "Remove",
                                                               (instance, args) => StrRemoveStartCount(stringType, instance, args),
                                                               stringType,
                                                               XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                               stringType,
                                                               new XLangFunctionArgument("start", numberType),
                                                               new XLangFunctionArgument("count", numberType)
                                                              );
            DelegateXLFunction remStrS = new DelegateXLFunction(
                                                                "Remove",
                                                                (instance, args) => StrRemoveStart(stringType, instance, args),
                                                                stringType,
                                                                XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                stringType,
                                                                new XLangFunctionArgument("start", numberType)
                                                               );

            DelegateXLFunction containsStr = new DelegateXLFunction(
                                                                "Contains",
                                                                StrContains,
                                                                numberType,
                                                                XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                stringType,
                                                                new XLangFunctionArgument("str", stringType)
                                                               );

            DelegateXLFunction replaceStr = new DelegateXLFunction(
                                                                    "Replace",
                                                                    (instance, args) => StrReplace(stringType, instance, args),
                                                                    stringType,
                                                                    XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                    stringType,
                                                                    new XLangFunctionArgument("old", stringType),
                                                                    new XLangFunctionArgument("newStr", stringType)
                                                                   );

            DelegateXLFunction trimStr = new DelegateXLFunction(
                                                                "Trim",
                                                                (instance, args) => StrTrim(stringType, instance),
                                                                stringType,
                                                                XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                stringType
                                                               );

            DelegateXLFunction trimStrS = new DelegateXLFunction(
                                                                "TrimStart",
                                                                (instance, args) => StrTrimStart(stringType, instance),
                                                                stringType,
                                                                XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                stringType
                                                               );
            DelegateXLFunction trimStrE = new DelegateXLFunction(
                                                                "TrimEnd",
                                                                (instance, args) => StrTrimEnd(stringType, instance),
                                                                stringType,
                                                                XLangMemberFlags.Public | XLangMemberFlags.Instance,
                                                                stringType
                                                               );

            DelegateXLFunction strConcat = new DelegateXLFunction(
                                                                  "Concat",
                                                                  (instance, args) => StringConcat(stringType, args),
                                                                  stringType, XLangMemberFlags.Public | XLangMemberFlags.Static,
                                                                  stringType,
                                                                  new XLangFunctionArgument("a", stringType),
                                                                  new XLangFunctionArgument("b", stringType));

            DelegateXLFunction strOpConcat = new DelegateXLFunction(
                                                                  XLangTokenType.OpPlus.ToString(),
                                                                  (instance, args) => StringConcat(stringType, args),
                                                                  stringType,
                                                                  XLangMemberFlags.Static |
                                                                  XLangMemberFlags.Private |
                                                                  XLangMemberFlags.Operator |
                                                                  XLangMemberFlags.Override,
                                                                  stringType,
                                                                  new XLangFunctionArgument("a", stringType),
                                                                  new XLangFunctionArgument("b", stringType));
            DelegateXLFunction sumAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpSumAssign.ToString(), (instance, args) => StringAssignConcat(args),
                                       stringType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       stringType,
                                       new XLangFunctionArgument("a", stringType),
                                       new XLangFunctionArgument("b", stringType)
                                      );

            stringType.SetMembers(new IXLangRuntimeMember[]
                                  {
                                      stringLengthFunction,
                                      stringLengthProperty,
                                      strConcat,
                                      strOpConcat,
                                      sumAssignFunc,
                                      subStr,
                                      subStrS,
                                      remStr,
                                      remStrS,
                                      containsStr,
                                      replaceStr,
                                      trimStr,
                                      trimStrE,
                                      trimStrS
                                  });
            objectType.InjectMember(toString);
            return stringType;
        }
    }
}
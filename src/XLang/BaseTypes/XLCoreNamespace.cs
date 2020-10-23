using System;
using System.Collections.Generic;

using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;

namespace XLang.BaseTypes
{
    public class XLCoreNamespace : XLangRuntimeNamespace
    {

        public event Action<string> WriteLineImpl = null;

        public void SetWritelineImpl(Action<string> impl) => WriteLineImpl = impl;
        public void InvokeWriteLine(string line) => WriteLineImpl?.Invoke(line);

        private XLCoreNamespace(XLangSettings settings, Action<string> writeLineImpl = null) : base("XL", null, new List<XLangRuntimeType>(), settings)
        {
            WriteLineImpl = writeLineImpl ?? (x => Console.WriteLine("[println]" + x));
        }



        public static XLCoreNamespace CreateNamespace(XLangSettings settings)
        {
            XLCoreNamespace core = new XLCoreNamespace(settings);



            XLangRuntimeType voidType = new XLangRuntimeType(
                                                             "void",
                                                             core,
                                                             null,
                                                             XLangBindingFlags.Instance | XLangBindingFlags.Public
                                                            );



            XLangRuntimeType objectType = new XLangObjectType(core).GetObject();
            XLangRuntimeType arrayType = new XLangRuntimeType(
                                                              "Array",
                                                              core,
                                                              objectType,
                                                              XLangBindingFlags.Instance | XLangBindingFlags.Public
                                                             );

            XLangRuntimeType functionType = new XLangFunctionType(core).GetObject(objectType);
            XLangRuntimeType numberType = new XLangNumberType(core).GetObject(objectType);
            XLangRuntimeType stringType = new XLangStringType(core).GetObject(objectType, numberType);


            voidType.SetMembers(new IXLangRuntimeMember[0]);
            arrayType.SetMembers(new IXLangRuntimeMember[0]);
            core.AddType(numberType);
            core.AddType(stringType);
            core.AddType(voidType);
            core.AddType(objectType);
            core.AddType(arrayType);
            core.AddType(functionType);
            core.AddType(XLangStringType.CreateConsole(core, voidType, stringType));
            return core;
        }

    }
}
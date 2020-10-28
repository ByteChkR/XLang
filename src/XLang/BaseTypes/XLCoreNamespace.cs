using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLang.Core;
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
        private XLCoreNamespace(XLangSettings settings, Action<string> writeLineImpl = null) : base("XL", null,
            new List<XLangRuntimeType>(), settings)
        {
            WriteLineImpl = writeLineImpl ?? (x => Console.WriteLine("[println]" + x));
        }

        public event Action<string> WriteLineImpl;

        public void SetWritelineImpl(Action<string> impl)
        {
            WriteLineImpl = impl;
        }

        public void InvokeWriteLine(string line)
        {
            WriteLineImpl?.Invoke(line);
        }


        public static XLCoreNamespace CreateNamespace(XLangContext context)
        {
            XLCoreNamespace core = new XLCoreNamespace(context.Settings);


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
                XLangBindingFlags.Instance | XLangBindingFlags.Public, type => new CSharpTypeInstance(type, new object[0])
            );

            XLangRuntimeType functionType = new XLangFunctionType(core).GetObject(objectType);
            XLangRuntimeType numberType = new XLangNumberType(core).GetObject(objectType);
            XLangRuntimeType stringType = new XLangStringType(core).GetObject(objectType, numberType);

            DelegateXLProperty lenProp = new DelegateXLProperty("Length", instance => LengthArrProp(context, instance),
                numberType, XLangMemberFlags.Public | XLangMemberFlags.Instance, arrayType);

            DelegateXLFunction elemAccess = new DelegateXLFunction(
                XLangTokenType.OpArrayAccess.ToString(),
                (instance, instances) => ArrAccess(context, instance, instances), objectType,
                XLangMemberFlags.Static |
                XLangMemberFlags.Private |
                XLangMemberFlags.Operator |
                XLangMemberFlags.Override,
                arrayType,
                new XLangFunctionArgument("index", numberType));

            voidType.SetMembers(new IXLangRuntimeMember[0]);
            arrayType.SetMembers(new IXLangRuntimeMember[] { elemAccess, lenProp });
            core.AddType(numberType);
            core.AddType(stringType);
            core.AddType(voidType);
            core.AddType(objectType);
            core.AddType(arrayType);
            core.AddType(functionType);
            core.AddType(XLangStringType.CreateConsole(core, voidType, stringType));
            return core;
        }

        private static IXLangRuntimeTypeInstance LengthArrProp(XLangContext context, IXLangRuntimeTypeInstance instance)
        {
            object obj = instance.GetRaw();
            XLangRuntimeType numType = context.GetType("XL.number");
            if (obj is IEnumerable en)
            {
                return new CSharpTypeInstance(numType, (decimal)en.Cast<object>().Count());
            }
            else return new CSharpTypeInstance(numType, (decimal)0);
        }

        private static IXLangRuntimeTypeInstance ArrAccess(XLangContext context, IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            object obj = instance.GetRaw();
            XLangRuntimeType objType = context.GetType("XL.object");
            if (obj is IEnumerable en)
            {
                int idx = (int)(decimal) args[0].GetRaw();
                return new CSharpTypeInstance(objType, en.OfType<object>().ElementAt(idx));
            }
            else return new CSharpTypeInstance(objType, null);
        }


    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XLang.Core;
using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;


/// <summary>
/// Contains Core Type Implementations
/// </summary>
namespace XLang.BaseTypes
{
    /// <summary>
    ///     Implements the "XL" core namespace
    /// </summary>
    public class XLCoreNamespace : XLangRuntimeNamespace
    {
        /// <summary>
        ///     Private Constructor
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="writeLineImpl">Write Line Implementation</param>
        private XLCoreNamespace(XLangSettings settings, Action<string> writeLineImpl = null) : base("XL", null,
            new List<XLangRuntimeType>(), settings)
        {
            WriteLineImpl = writeLineImpl ?? (x => Console.WriteLine("[println]" + x));
        }

        /// <summary>
        ///     Write Line Implementation
        /// </summary>
        public event Action<string> WriteLineImpl;

        /// <summary>
        ///     Sets the Write Line Event
        /// </summary>
        /// <param name="impl">Implementation</param>
        public void SetWritelineImpl(Action<string> impl)
        {
            WriteLineImpl = impl;
        }

        /// <summary>
        ///     Invokes the Write line Event.
        /// </summary>
        /// <param name="line">Line to Write</param>
        public void InvokeWriteLine(string line)
        {
            WriteLineImpl?.Invoke(line);
        }


        /// <summary>
        ///     Creates the Core Namespace
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <returns>XL Core Namespace ("XL")</returns>
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
                XLangBindingFlags.Instance | XLangBindingFlags.Public,
                type => new CSharpTypeInstance(type, new object[0])
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
            arrayType.SetMembers(new IXLangRuntimeMember[] {elemAccess, lenProp});
            core.AddType(numberType);
            core.AddType(stringType);
            core.AddType(voidType);
            core.AddType(objectType);
            core.AddType(arrayType);
            core.AddType(functionType);
            core.AddType(XLangStringType.CreateConsole(core, voidType, stringType));
            return core;
        }


        /// <summary>
        ///     Implements "XL.Array.Length" property
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="instance">Instance of the Array</param>
        /// <returns>Array Length</returns>
        private static IXLangRuntimeTypeInstance LengthArrProp(XLangContext context, IXLangRuntimeTypeInstance instance)
        {
            object obj = instance.GetRaw();
            XLangRuntimeType numType = context.GetType("XL.number");
            if (obj is IEnumerable en)
            {
                return new CSharpTypeInstance(numType, (decimal) en.Cast<object>().Count());
            }
            return new CSharpTypeInstance(numType, (decimal) 0);
        }

        /// <summary>
        ///     Implements XL.Array[i] array accessor
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="instance">Array Instance</param>
        /// <param name="args">Accessor Arguments</param>
        /// <returns>Element at the specified index.</returns>
        private static IXLangRuntimeTypeInstance ArrAccess(XLangContext context, IXLangRuntimeTypeInstance instance,
            IXLangRuntimeTypeInstance[] args)
        {
            object obj = instance.GetRaw();
            XLangRuntimeType objType = context.GetType("XL.object");
            if (obj is IEnumerable en)
            {
                int idx = (int) (decimal) args[0].GetRaw();
                return new CSharpTypeInstance(objType, en.OfType<object>().ElementAt(idx));
            }
            return new CSharpTypeInstance(objType, null);
        }
    }
}
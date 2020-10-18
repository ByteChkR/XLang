using System.Collections.Generic;

using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Types;

namespace XLang.BaseTypes
{
    public class XLCoreNamespace : XLangRuntimeNamespace
    {

        private XLCoreNamespace(XLangSettings settings) : base("XL", null, new List<XLangRuntimeType>(), settings)
        {
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
            return core;
        }

    }
}
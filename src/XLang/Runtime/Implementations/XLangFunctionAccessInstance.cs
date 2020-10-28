using System;
using XLang.Runtime.Members;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Runtime.Implementations
{
    public class XLangFunctionAccessInstance : IXLangRuntimeTypeInstance
    {
        public readonly XLangRuntimeType FunctionType;
        public readonly IXLangRuntimeTypeInstance Instance;

        public XLangFunctionAccessInstance(
            IXLangRuntimeItem[] member, IXLangRuntimeTypeInstance instance, XLangRuntimeType functionType)
        {
            Instance = instance;
            Member = member;
            FunctionType = functionType;
        }

        public IXLangRuntimeItem[] Member { get; private set; }

        public XLangRuntimeType Type => FunctionType;

        public void AddLocals(XLangRuntimeScope scope)
        {
        }

        public object GetRaw()
        {
            return Member;
        }

        public void SetRaw(XLangRuntimeType type, object value)
        {
            if (value == null)
            {
                return;
            }

            if (type.InheritsFrom(Type))
            {
                Member[0] = (IXLangRuntimeMember) value;
            }
            else
            {
                throw new Exception("Type Mismatch");
            }

        }
    }
}
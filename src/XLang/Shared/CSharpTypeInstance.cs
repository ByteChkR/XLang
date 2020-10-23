using System;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Shared
{
    public class CSharpTypeInstance : IXLangRuntimeTypeInstance
    {
        private object value;

        public CSharpTypeInstance(XLangRuntimeType type, object value)
        {
            Type = type;
            this.value = value;
        }

        public XLangRuntimeType Type { get; }

        public void AddLocals(XLangRuntimeScope scope)
        {
        }

        public object GetRaw()
        {
            return value;
        }

        public void SetRaw(XLangRuntimeType type, object value)
        {
            if (type.InheritsFrom(Type))
            {
                this.value = value;
            }
            else
            {
                throw new Exception("Type Mismatch");
            }
        }
    }
}
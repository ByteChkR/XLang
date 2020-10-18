using System;
using System.Collections.Generic;
using System.Linq;

using XLang.Queries;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

namespace XLang.Runtime.Implementations
{
    public class XLangBaseObject : IXLangRuntimeTypeInstance
    {

        private Dictionary<IXLangRuntimeMember, IXLangRuntimeTypeInstance> instanceVars =
            new Dictionary<IXLangRuntimeMember, IXLangRuntimeTypeInstance>();

        public XLangBaseObject(XLangRuntimeType type)
        {
            Type = type;
            IXLangRuntimeProperty[] props = Type.GetMembers(XLangBindingQuery.Property | XLangBindingQuery.Instance)
                                                .Cast<IXLangRuntimeProperty>().ToArray();
            foreach (IXLangRuntimeProperty prop in props)
            {
                instanceVars[prop] = prop.GetValue(this);
            }
        }

        public XLangRuntimeType Type { get; private set; }

        public void AddLocals(XLangRuntimeScope scope)
        {
            scope.Declare("this", Type).SetValue(this);
            (IXLangRuntimeProperty, IXLangRuntimeTypeInstance)[] memberValues = Type
                                                                                .GetMembers(
                                                                                            XLangBindingQuery.Property |
                                                                                            XLangBindingQuery.Instance
                                                                                           ).Cast<IXLangRuntimeProperty
                                                                                >().Select(x => (x, x.GetValue(this)))
                                                                                .ToArray();
            foreach ((IXLangRuntimeProperty prop, IXLangRuntimeTypeInstance value) in memberValues)
            {
                scope.Declare(prop.Name, prop.PropertyType).SetValue(value);
            }
        }

        public object GetRaw()
        {
            return this;
        }

        public void SetRaw(XLangRuntimeType type, object value)
        {
            if (value == null)
            {
                throw new Exception("look at this");
            }

            if (type.InheritsFrom(Type))
            {
                instanceVars = ((XLangBaseObject)value).instanceVars;
                Type = type;
            }
            else 
            {
                throw new Exception("Type mismatch");
            }
        }

    }
}
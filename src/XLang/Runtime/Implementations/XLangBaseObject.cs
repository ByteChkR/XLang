using System;
using System.Collections.Generic;
using System.Linq;
using XLang.Exceptions;
using XLang.Queries;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

/// <summary>
/// Contains Runtime Item Internal Implementations
/// </summary>
namespace XLang.Runtime.Implementations
{
    /// <summary>
    ///     XLang base Object implementation
    /// </summary>
    public class XLangBaseObject : IXLangRuntimeTypeInstance
    {
        /// <summary>
        ///     Instance Vars
        /// </summary>
        private Dictionary<IXLangRuntimeMember, IXLangRuntimeTypeInstance> instanceVars =
            new Dictionary<IXLangRuntimeMember, IXLangRuntimeTypeInstance>();

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="type">Type of the Instance</param>
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

        /// <summary>
        ///     Type of this Instance
        /// </summary>
        public XLangRuntimeType Type { get; private set; }

        /// <summary>
        ///     Adds all Local Defined Vars inside this Object
        /// </summary>
        /// <param name="scope">The Scope to add to.</param>
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

        /// <summary>
        ///     Gets the Raw Object Value
        /// </summary>
        /// <returns></returns>
        public object GetRaw()
        {
            return this;
        }

        /// <summary>
        ///     Sets the raw object value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetRaw(XLangRuntimeType type, object value)
        {
            if (value == null)
            {
                throw new XLangRuntimeTypeException("look at this");
            }

            if (type.InheritsFrom(Type))
            {
                instanceVars = ((XLangBaseObject) value).instanceVars;
                Type = type;
            }
            else
            {
                throw new XLangRuntimeTypeException("Type mismatch");
            }
        }
    }
}
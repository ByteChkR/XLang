using System;
using XLang.Shared.Enum;

namespace XLang.Runtime.Members
{
    /// <summary>
    ///     Member Type Enum
    /// </summary>
    [Flags]
    public enum XLangMemberType
    {
        Function = XLangBindingQuery.Function,
        Property = XLangBindingQuery.Property,
        Class = XLangBindingQuery.Class,
        Constructor = XLangBindingQuery.Constructor
    }
}
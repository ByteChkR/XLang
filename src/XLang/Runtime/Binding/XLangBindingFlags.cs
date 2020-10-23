using System;
using XLang.Shared.Enum;

namespace XLang.Runtime.Binding
{
    [Flags]
    public enum XLangBindingFlags
    {
        Public = XLangBindingQuery.Public,
        Private = XLangBindingQuery.Private,
        Static = XLangBindingQuery.Static,
        Instance = XLangBindingQuery.Instance,
        Abstract = XLangBindingQuery.Abstract,
        Protected = XLangBindingQuery.Protected
    }
}
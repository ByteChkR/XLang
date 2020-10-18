using System;

using XLang.Shared.Enum;

namespace XLang.Runtime.Binding
{
    [Flags]
    public enum XLangAccessibilityLevel
    {

        Public = XLangBindingQuery.Public,
        Private = XLangBindingQuery.Private,
        Protected = XLangBindingQuery.Protected

    }
}
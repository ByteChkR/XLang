using System;
using XLang.Shared.Enum;

/// <summary>
/// Contains Binding Flag Enums
/// </summary>
namespace XLang.Runtime.Binding
{
    /// <summary>
    ///     Accessibilty Level of Classes.
    /// </summary>
    [Flags]
    public enum XLangAccessibilityLevel
    {
        Public = XLangBindingQuery.Public,
        Private = XLangBindingQuery.Private,
        Protected = XLangBindingQuery.Protected
    }
}
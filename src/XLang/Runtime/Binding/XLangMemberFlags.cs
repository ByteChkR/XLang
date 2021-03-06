﻿using System;
using XLang.Shared.Enum;

namespace XLang.Runtime.Binding
{
    /// <summary>
    ///     Member Flags Enum used to Modify Member Accessibilty and Functionality.
    /// </summary>
    [Flags]
    public enum XLangMemberFlags
    {
        Public = XLangBindingQuery.Public,
        Private = XLangBindingQuery.Private,
        Static = XLangBindingQuery.Static,
        Instance = XLangBindingQuery.Instance,
        Abstract = XLangBindingQuery.Abstract,
        Protected = XLangBindingQuery.Protected,
        Virtual = XLangBindingQuery.Virtual,
        Override = XLangBindingQuery.Override,
        Operator = XLangBindingQuery.Operator,
        Constructor = XLangBindingQuery.Constructor
    }
}
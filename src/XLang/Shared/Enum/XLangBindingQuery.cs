using System;

namespace XLang.Shared.Enum
{
    [Flags]
    public enum XLangBindingQuery
    {

        Public = 1,
        Protected = 2,
        Private = 4,
        Static = 8,
        Instance = 16,
        Abstract = 32,
        Virtual = 64,
        Override = 128,
        Operator = 256,
        Function = 512,
        Property = 1024,
        Exact = 2048,
        Inclusive = 4096,
        MatchType = 8192,
        Class = 16384,
        Constructor = 32768

    }
}
using XLang.Runtime.Types;

namespace XLang.Runtime.Members.Functions
{
    public interface IXLangRuntimeFunctionArgument
    {

        string Name { get; }

        XLangRuntimeType Type { get; }

    }
}
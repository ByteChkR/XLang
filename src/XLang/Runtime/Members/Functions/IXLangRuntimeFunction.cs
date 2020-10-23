using XLang.Runtime.Types;

namespace XLang.Runtime.Members.Functions
{
    public interface IXLangRuntimeFunction : IXLangRuntimeMember
    {
        XLangRuntimeType ReturnType { get; }

        IXLangRuntimeFunctionArgument[] ParameterList { get; }

        IXLangRuntimeTypeInstance Invoke(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] arguments);
    }
}
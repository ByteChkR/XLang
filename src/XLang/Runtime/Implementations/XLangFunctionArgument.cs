using XLang.Runtime.Members.Functions;
using XLang.Runtime.Types;

namespace XLang.Runtime.Implementations
{
    public class XLangFunctionArgument : IXLangRuntimeFunctionArgument
    {

        public XLangFunctionArgument(string name, XLangRuntimeType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public XLangRuntimeType Type { get; }

    }
}
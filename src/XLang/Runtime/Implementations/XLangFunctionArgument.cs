using XLang.Runtime.Members.Functions;
using XLang.Runtime.Types;

namespace XLang.Runtime.Implementations
{
    /// <summary>
    ///     XLang Function Argument Implementation
    /// </summary>
    public class XLangFunctionArgument : IXLangRuntimeFunctionArgument
    {
        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Name of the Function Argument</param>
        /// <param name="type">Return type of the Function Argument</param>
        public XLangFunctionArgument(string name, XLangRuntimeType type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        ///     The Name of the Argument
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The Type of the Argument.
        /// </summary>
        public XLangRuntimeType Type { get; }
    }
}
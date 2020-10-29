using XLang.Runtime.Types;

namespace XLang.Runtime.Members.Functions
{
    /// <summary>
    ///     Function Argument Definition
    /// </summary>
    public interface IXLangRuntimeFunctionArgument
    {
        /// <summary>
        ///     Name of the Argument
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The Type of the Argument.
        /// </summary>
        XLangRuntimeType Type { get; }
    }
}
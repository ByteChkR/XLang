using XLang.Runtime.Types;

/// <summary>
/// Contains Function Runtime Interfaces
/// </summary>
namespace XLang.Runtime.Members.Functions
{
    /// <summary>
    ///     Defines an XL Function.
    /// </summary>
    public interface IXLangRuntimeFunction : IXLangRuntimeMember
    {
        /// <summary>
        ///     The Return type of this Function
        /// </summary>
        XLangRuntimeType ReturnType { get; }

        /// <summary>
        ///     The Parameters for this Function
        /// </summary>
        IXLangRuntimeFunctionArgument[] ParameterList { get; }

        /// <summary>
        ///     Invokes this Function
        /// </summary>
        /// <param name="instance">Instance of the Implementing Class. Null for static function</param>
        /// <param name="arguments">Function Arguments.</param>
        /// <returns>Return value of the function</returns>
        IXLangRuntimeTypeInstance Invoke(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] arguments);
    }
}
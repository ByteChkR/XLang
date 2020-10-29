using XLang.Runtime.Scopes;

/// <summary>
/// Contains Runtime Items that are related to Types
/// </summary>
namespace XLang.Runtime.Types
{
    /// <summary>
    ///     Runtime Type instance Interface Definition
    /// </summary>
    public interface IXLangRuntimeTypeInstance
    {
        /// <summary>
        ///     The Type of this instance
        /// </summary>
        XLangRuntimeType Type { get; }

        /// <summary>
        ///     Adds all Local Vars to this type instance.
        /// </summary>
        /// <param name="scope"></param>
        void AddLocals(XLangRuntimeScope scope);

        /// <summary>
        ///     Returns the Raw Value of this Instance
        /// </summary>
        /// <returns></returns>
        object GetRaw();

        /// <summary>
        ///     Sets the Raw Value of this instance.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        void SetRaw(XLangRuntimeType type, object value);
    }
}
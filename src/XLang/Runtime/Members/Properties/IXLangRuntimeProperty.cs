using XLang.Runtime.Types;

/// <summary>
/// Contains Runtime Property Interfaces
/// </summary>
namespace XLang.Runtime.Members.Properties
{
    /// <summary>
    ///     Defines a XLRuntime Property
    /// </summary>
    public interface IXLangRuntimeProperty : IXLangRuntimeMember
    {
        /// <summary>
        ///     The Type of this Property
        /// </summary>
        XLangRuntimeType PropertyType { get; }

        /// <summary>
        ///     Returns true if the value can be set.
        /// </summary>
        /// <returns></returns>
        bool CanSet();

        /// <summary>
        ///     Returns true if the value can be read.
        /// </summary>
        /// <returns></returns>
        bool CanGet();

        /// <summary>
        ///     Returns the Value of this property.
        /// </summary>
        /// <param name="instance">Instance of the Implementing Class. Null for static properties</param>
        /// <returns></returns>
        IXLangRuntimeTypeInstance GetValue(IXLangRuntimeTypeInstance instance);

        /// <summary>
        ///     Sets the Value of this property
        /// </summary>
        /// <param name="instance">Instance of the Implementing Class. Null for static propert</param>
        /// <param name="value">New value of this property.</param>
        void SetValue(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance value);
    }
}
using XLang.Runtime.Binding;
using XLang.Runtime.Members;

/// <summary>
/// Contains Runtime Scope Logic
/// </summary>
namespace XLang.Runtime.Scopes
{
    /// <summary>
    ///     Defines a Scope Accessible Item
    /// </summary>
    public interface IXLangScopeAccess : IXLangRuntimeItem
    {
        /// <summary>
        ///     Name of the Item
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The Accessibility Level
        /// </summary>
        XLangAccessibilityLevel AccessibilityLevel { get; }

        /// <summary>
        ///     The Binding Flags
        /// </summary>
        XLangBindingFlags BindingFlags { get; }

        /// <summary>
        ///     The Type of this Item
        /// </summary>
        XLangMemberType ItemType { get; }
    }
}
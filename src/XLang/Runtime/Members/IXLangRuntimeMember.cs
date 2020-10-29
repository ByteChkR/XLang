using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

/// <summary>
/// Contains Runtime Member Interfaces and Implementations
/// </summary>
namespace XLang.Runtime.Members
{
    /// <summary>
    ///     Interface that defines the Runtime member
    /// </summary>
    public interface IXLangRuntimeMember : IXLangScopeAccess
    {
        /// <summary>
        ///     The Implementing Class of this Member.
        /// </summary>
        XLangRuntimeType ImplementingClass { get; }
    }
}
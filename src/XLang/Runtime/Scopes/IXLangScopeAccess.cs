using XLang.Runtime.Binding;
using XLang.Runtime.Members;

namespace XLang.Runtime.Scopes
{
    public interface IXLangScopeAccess : IXLangRuntimeItem
    {

        string Name { get; }

        XLangAccessibilityLevel AccessibilityLevel { get; }

        XLangBindingFlags BindingFlags { get; }

        XLangMemberType ItemType { get; }

    }
}
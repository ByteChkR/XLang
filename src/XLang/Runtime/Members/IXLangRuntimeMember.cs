using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Runtime.Members
{
    public interface IXLangRuntimeMember : IXLangScopeAccess
    {
        XLangRuntimeType ImplementingClass { get; }

        XLangMemberType MemberType { get; }
    }
}
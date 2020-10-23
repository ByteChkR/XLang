using XLang.Runtime.Types;

namespace XLang.Runtime.Members.Properties
{
    public interface IXLangRuntimeProperty : IXLangRuntimeMember
    {
        XLangRuntimeType PropertyType { get; }

        bool CanSet();

        bool CanGet();

        IXLangRuntimeTypeInstance GetValue(IXLangRuntimeTypeInstance instance);

        void SetValue(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance value);
    }
}
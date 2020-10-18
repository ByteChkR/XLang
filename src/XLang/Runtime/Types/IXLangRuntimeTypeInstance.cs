using XLang.Runtime.Scopes;

namespace XLang.Runtime.Types
{
    public interface IXLangRuntimeTypeInstance
    {

        XLangRuntimeType Type { get; }

        void AddLocals(XLangRuntimeScope scope);

        object GetRaw();

        void SetRaw(XLangRuntimeType type, object value);

    }
}
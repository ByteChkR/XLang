using System;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Shared
{
    public class DelegateXLProperty : IXLangRuntimeProperty
    {
        private readonly Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance> onGet;

        public DelegateXLProperty(
            string name, Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance> getValue,
            XLangRuntimeType propertyType, XLangMemberFlags flags)
        {
            Name = name;
            PropertyType = propertyType;
            onGet = getValue;
        }


        public XLangMemberFlags BindingFlags { get; }

        public string Name { get; }

        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        public XLangMemberType ItemType => XLangMemberType.Property;

        public XLangRuntimeType ImplementingClass { get; }

        public XLangMemberType MemberType => XLangMemberType.Property;

        public XLangRuntimeType PropertyType { get; }

        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        public bool CanSet()
        {
            return false;
        }

        public bool CanGet()
        {
            return true;
        }

        public IXLangRuntimeTypeInstance GetValue(IXLangRuntimeTypeInstance instance)
        {
            return onGet(instance);
        }

        public void SetValue(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance value)
        {
            throw new XLangRuntimeTypeException($"Property: {ImplementingClass.FullName}.{Name} has no Setter");
        }
    }
}
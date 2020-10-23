using System.Collections.Generic;
using XLang.Parser.Token.Expressions;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Runtime
{
    public class XLangProperty : IXLangRuntimeProperty
    {
        private readonly XLangContext context;

        private readonly XLangExpression InitExpr;

        private readonly Dictionary<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance> InstanceMap =
            new Dictionary<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance>();

        private IXLangRuntimeTypeInstance staticValue;

        public XLangProperty(
            string name, XLangRuntimeType propertyType, XLangRuntimeType implementingClass,
            XLangMemberFlags bindingFlags, XLangContext context, XLangExpression initExpression)
        {
            Name = name;
            PropertyType = propertyType;
            ImplementingClass = implementingClass;
            BindingFlags = bindingFlags;
            this.context = context;
            InitExpr = initExpression;
        }

        public XLangMemberFlags BindingFlags { get; }

        public bool IsStatic => (BindingFlags & XLangMemberFlags.Static) != 0;

        public XLangMemberType ItemType => XLangMemberType.Property;

        public string Name { get; }

        public XLangRuntimeType ImplementingClass { get; }

        public XLangMemberType MemberType => XLangMemberType.Property;

        public XLangRuntimeType PropertyType { get; }

        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        bool IXLangRuntimeProperty.CanSet()
        {
            return CanSet();
        }

        bool IXLangRuntimeProperty.CanGet()
        {
            return CanGet();
        }

        IXLangRuntimeTypeInstance IXLangRuntimeProperty.GetValue(IXLangRuntimeTypeInstance instance)
        {
            return GetValue(instance);
        }

        void IXLangRuntimeProperty.SetValue(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance value)
        {
            SetValue(instance, value);
        }

        public bool CanSet()
        {
            return true;
        }

        public bool CanGet()
        {
            return true;
        }

        public IXLangRuntimeTypeInstance GetValue(IXLangRuntimeTypeInstance instance)
        {
            return GetVal(instance);
        }

        private IXLangRuntimeTypeInstance GetVal(IXLangRuntimeTypeInstance instance)
        {
            XLangRuntimeScope scope = new XLangRuntimeScope(context, this);
            if (IsStatic)
            {
                ImplementingClass.AddStatics(scope);
                return staticValue ??
                       (staticValue = InitExpr?.Process(scope, instance) ?? PropertyType.CreateEmptyBase());
            }

            return InstanceMap.ContainsKey(instance)
                ? InstanceMap[instance]
                : InstanceMap[instance] = InitExpr?.Process(scope, instance) ?? PropertyType.CreateEmptyBase();
        }

        public void SetValue(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance value)
        {
            if (IsStatic)
            {
                staticValue = value;
                return;
            }

            InstanceMap[instance] = value;
        }
    }
}
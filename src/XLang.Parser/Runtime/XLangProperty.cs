using System.Collections.Generic;
using XLang.Parser.Token.Expressions;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Runtime
{
    /// <summary>
    ///     Implements a Runtime Property
    /// </summary>
    public class XLangProperty : IXLangRuntimeProperty
    {
        /// <summary>
        ///     The XL Context
        /// </summary>
        private readonly XLangContext context;

        /// <summary>
        ///     The Initialization Expression of the Property.
        /// </summary>
        private readonly XLangExpression InitExpr;

        /// <summary>
        ///     The Instance Map
        ///     This Map maps the Implementing Class and the property value
        /// </summary>
        private readonly Dictionary<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance> InstanceMap =
            new Dictionary<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance>();

        /// <summary>
        ///     The Static value of the Property
        /// </summary>
        private IXLangRuntimeTypeInstance staticValue;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Property Name</param>
        /// <param name="propertyType">Property Type</param>
        /// <param name="implementingClass">The Implementing Class of the Property</param>
        /// <param name="bindingFlags">Property Binding Flags</param>
        /// <param name="context">XL Context</param>
        /// <param name="initExpression">Initialization Expression</param>
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

        /// <summary>
        ///     The Property Binding Flags
        /// </summary>
        public XLangMemberFlags BindingFlags { get; }

        /// <summary>
        ///     True if the Binding Flags contains XLangMemberFlags.Static
        /// </summary>
        public bool IsStatic => (BindingFlags & XLangMemberFlags.Static) != 0;

        /// <summary>
        ///     The Item Type (Property)
        /// </summary>
        public XLangMemberType ItemType => XLangMemberType.Property;

        /// <summary>
        ///     The Property Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The Implementing Class
        /// </summary>
        public XLangRuntimeType ImplementingClass { get; }

        /// <summary>
        ///     The Type of the Property.
        /// </summary>
        public XLangRuntimeType PropertyType { get; }

        /// <summary>
        ///     Accessibilty Level of the Property
        /// </summary>
        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        /// <summary>
        ///     The Binding Flags
        /// </summary>
        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        /// <summary>
        ///     Returns true if the Property can be set.
        /// </summary>
        /// <returns></returns>
        bool IXLangRuntimeProperty.CanSet()
        {
            return CanSet();
        }

        /// <summary>
        ///     Returns true if the property can be read.
        /// </summary>
        /// <returns></returns>
        bool IXLangRuntimeProperty.CanGet()
        {
            return CanGet();
        }

        /// <summary>
        ///     Returns the Value of the Property
        /// </summary>
        /// <param name="instance">Implementing Class Instance. Null if static</param>
        /// <returns></returns>
        IXLangRuntimeTypeInstance IXLangRuntimeProperty.GetValue(IXLangRuntimeTypeInstance instance)
        {
            return GetValue(instance);
        }

        /// <summary>
        ///     Sets the Value of the Property
        /// </summary>
        /// <param name="instance">Implementing Class Instance. Null if static</param>
        /// <param name="value">new property value</param>
        void IXLangRuntimeProperty.SetValue(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance value)
        {
            SetValue(instance, value);
        }

        /// <summary>
        ///     returns true if the Value can be set
        /// </summary>
        /// <returns></returns>
        public bool CanSet()
        {
            return true;
        }

        /// <summary>
        ///     Returns true if the value can be read.
        /// </summary>
        /// <returns></returns>
        public bool CanGet()
        {
            return true;
        }

        /// <summary>
        ///     Returns the value of the property
        /// </summary>
        /// <param name="instance">Implementing Class Instance. Null if static</param>
        /// <returns></returns>
        public IXLangRuntimeTypeInstance GetValue(IXLangRuntimeTypeInstance instance)
        {
            return GetVal(instance);
        }

        /// <summary>
        ///     Returns the Value of the Property
        /// </summary>
        /// <param name="instance">Implementing Class Instance. Null if static</param>
        /// <returns></returns>
        private IXLangRuntimeTypeInstance GetVal(IXLangRuntimeTypeInstance instance)
        {
            XLangRuntimeScope scope = new XLangRuntimeScope(context, this);
            if (IsStatic)
            {

                return staticValue ??
                       (staticValue = InitExpr?.Process(scope, instance) ?? PropertyType.CreateEmptyBase());
            }

            return InstanceMap.ContainsKey(instance)
                ? InstanceMap[instance]
                : InstanceMap[instance] = InitExpr?.Process(scope, instance) ?? PropertyType.CreateEmptyBase();
        }

        /// <summary>
        ///     Sets the Value of the Property
        /// </summary>
        /// <param name="instance">Implementing Class Instance. Null if static</param>
        /// <param name="value">New Property Value</param>
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
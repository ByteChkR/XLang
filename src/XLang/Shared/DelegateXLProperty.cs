using System;
using XLang.Exceptions;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Shared
{
    /// <summary>
    ///     Property Implementation that allows implementing properties with delegates in c#
    /// </summary>
    public class DelegateXLProperty : IXLangRuntimeProperty
    {
        /// <summary>
        ///     Get Delegate
        /// </summary>
        private readonly Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance> onGet;

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="name">Name of the Property</param>
        /// <param name="getValue">Get Value delegate</param>
        /// <param name="propertyType">The Type of the property</param>
        /// <param name="flags">Binding Flags for this Property</param>
        /// <param name="implementingClass">The Implementing class of this property.</param>
        public DelegateXLProperty(
            string name, Func<IXLangRuntimeTypeInstance, IXLangRuntimeTypeInstance> getValue,
            XLangRuntimeType propertyType, XLangMemberFlags flags, XLangRuntimeType implementingClass)
        {
            BindingFlags = flags;
            ImplementingClass = implementingClass;
            Name = name;
            PropertyType = propertyType;
            onGet = getValue;
        }

        /// <summary>
        ///     Binding Flags for this property
        /// </summary>
        public XLangMemberFlags BindingFlags { get; }

        /// <summary>
        ///     Name of this Property
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The Accessibilty Level of this Property
        /// </summary>
        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) ((XLangBindingFlags) BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        /// <summary>
        ///     Item Type (Property)
        /// </summary>
        public XLangMemberType ItemType => XLangMemberType.Property;

        /// <summary>
        ///     The Implementing class of this Property
        /// </summary>
        public XLangRuntimeType ImplementingClass { get; }


        /// <summary>
        ///     The Property Type.
        /// </summary>
        public XLangRuntimeType PropertyType { get; }

        /// <summary>
        ///     Binding Flags of this Scoped Item
        /// </summary>
        XLangBindingFlags IXLangScopeAccess.BindingFlags => (XLangBindingFlags) BindingFlags;

        /// <summary>
        ///     Returns True if the Property can be Set
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSet()
        {
            return false;
        }

        /// <summary>
        ///     Returns False if the property value can be read.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanGet()
        {
            return true;
        }

        /// <summary>
        ///     Returns the Value of this Property
        /// </summary>
        /// <param name="instance">The Instance of the implementing class. Null for static</param>
        /// <returns>Property Value</returns>
        public IXLangRuntimeTypeInstance GetValue(IXLangRuntimeTypeInstance instance)
        {
            return onGet(instance);
        }

        /// <summary>
        ///     Sets the Value of this Property
        /// </summary>
        /// <param name="instance">The Instance of the implementing class. Null for static</param>
        /// <param name="value">New property Value</param>
        public void SetValue(IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance value)
        {
            throw new XLangRuntimeTypeException($"Property: {ImplementingClass.FullName}.{Name} has no Setter");
        }

        /// <summary>
        ///     Returns the . concatenated Fully Qualified name of this type and property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ImplementingClass.FullName + "." + Name;
        }
    }
}
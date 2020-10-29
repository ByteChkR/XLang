using System;
using System.Collections.Generic;
using System.Linq;
using XLang.Queries;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Shared.Enum;

namespace XLang.Runtime.Types
{
    /// <summary>
    ///     XL Type Implementation
    /// </summary>
    public class XLangRuntimeType : IXLangScopeAccess
    {
        /// <summary>
        ///     Function that is used to create an empty base type.
        /// </summary>
        private readonly Func<XLangRuntimeType, IXLangRuntimeTypeInstance> createBase;

        /// <summary>
        ///     Members of this type
        /// </summary>
        private IXLangRuntimeMember[] members;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="nameSpace">Containing Namespace</param>
        /// <param name="baseType">Base Type ( Can be null for no base type )</param>
        /// <param name="bindingFlags">Binding Flags of this type</param>
        /// <param name="createBase">Create Base Function used to create empty type instances of this type.</param>
        public XLangRuntimeType(
            string name, XLangRuntimeNamespace nameSpace, XLangRuntimeType baseType, XLangBindingFlags bindingFlags,
            Func<XLangRuntimeType, IXLangRuntimeTypeInstance> createBase = null)
        {
            this.createBase = createBase ?? (x => new XLangBaseObject(x));
            Name = name;
            Namespace = nameSpace;
            BaseType = baseType;
            ;
            BindingFlags = bindingFlags;
        }

        /// <summary>
        ///     Containing Namespace
        /// </summary>
        public XLangRuntimeNamespace Namespace { get; }

        /// <summary>
        ///     Base Type of this Type
        /// </summary>
        public XLangRuntimeType BaseType { get; }

        /// <summary>
        ///     Fully qualified name of this Type
        /// </summary>
        public string FullName => Namespace.FullName + "." + Name;

        /// <summary>
        ///     The Item Type of this Item (Class)
        /// </summary>
        public XLangMemberType ItemType => XLangMemberType.Class;

        /// <summary>
        ///     Type Name
        /// </summary>
        public string Name { get; }


        /// <summary>
        ///     Accessibilty Level of this Type
        /// </summary>
        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) (BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        /// <summary>
        ///     Type Binding Flags
        /// </summary>
        public XLangBindingFlags BindingFlags { get; }

        /// <summary>
        ///     Sets the members of this type.
        /// </summary>
        /// <param name="members">New Member Collection</param>
        public void SetMembers(IXLangRuntimeMember[] members)
        {
            this.members = members;
        }

        /// <summary>
        ///     Returns true if the current type is inheriting the specified type OR the specified type IS the current type.
        /// </summary>
        /// <param name="type">Type to check against</param>
        /// <returns>True if the specified type can be "cast" into this type</returns>
        public bool InheritsFrom(XLangRuntimeType type)
        {
            return type == this || BaseType != null && BaseType.InheritsFrom(type);
        }

        /// <summary>
        ///     Creates an Empty Base Instance for this Type.
        /// </summary>
        /// <returns>Empty Type Instance</returns>
        public virtual IXLangRuntimeTypeInstance CreateEmptyBase()
        {
            return createBase(this);
        }

        /// <summary>
        ///     Searches this type for a Member with the specified name and Query
        /// </summary>
        /// <param name="name">Member Name</param>
        /// <param name="query">Member Binding Flags</param>
        /// <returns>True if member exists</returns>
        public bool HasMember(string name, XLangBindingQuery query)
        {
            return this.GetMembers(name, query).Length != 0;
        }

        /// <summary>
        ///     Searches this type for a Member with the specified Query
        /// </summary>
        /// <param name="query">Member Binding Flags</param>
        /// <returns>True if member exists</returns>
        public bool HasMember(XLangBindingQuery query)
        {
            return this.GetMembers(query).Length != 0;
        }

        /// <summary>
        ///     Returns all Members of the type, including inherited types.
        /// </summary>
        /// <returns>Array of Members</returns>
        public IXLangRuntimeMember[] GetAllMembers()
        {
            return BuildMemberList();
        }

        /// <summary>
        ///     Returns all Members of the type, including inherited types.
        /// </summary>
        /// <returns>Array of Members</returns>
        private IXLangRuntimeMember[] BuildMemberList()
        {
            if (BaseType == null)
            {
                return members;
            }

            List<IXLangRuntimeMember> ret = new List<IXLangRuntimeMember>(members);
            ret.AddRange(
                BaseType.GetAllMembers().Where(
                    x => ret.All(
                        y => x.Name != y.Name
                    )
                )
            );
            return ret.ToArray();
        }

        /// <summary>
        ///     Returns the Member with the specified name and query
        /// </summary>
        /// <param name="name">Member Name</param>
        /// <param name="query">Member Binding Flags</param>
        /// <returns>Result</returns>
        public IXLangRuntimeMember GetMember(string name, XLangBindingQuery query)
        {
            return this.GetMembers(name, query).FirstOrDefault();
        }

        /// <summary>
        ///     Returns all members with the specified name.
        /// </summary>
        /// <param name="name">Member Name</param>
        /// <returns>Array of Members</returns>
        public IXLangRuntimeMember[] GetMembers(string name)
        {
            return GetAllMembers().Where(x => x.Name == name).ToArray();
        }

        /// <summary>
        ///     Returns the first Member with the specified name
        /// </summary>
        /// <param name="name">Member Name</param>
        /// <returns>Member</returns>
        public IXLangRuntimeMember GetMember(string name)
        {
            return GetAllMembers().FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        ///     Returns the first Member that fits the Binding Flag Query
        /// </summary>
        /// <param name="query">Binding Flags</param>
        /// <returns>Member</returns>
        public IXLangRuntimeMember GetMember(XLangBindingQuery query)
        {
            return this.GetMembers(query).FirstOrDefault();
        }

        /// <summary>
        ///     Adds all Static properties to the specified scope
        /// </summary>
        /// <param name="scope">The scope</param>
        public void AddStatics(XLangRuntimeScope scope)
        {
            foreach (IXLangRuntimeMember xLangRuntimeMember in this.GetMembers(
                XLangBindingQuery.Static |
                XLangBindingQuery.Property
            ))
            {
                IXLangRuntimeProperty prop = xLangRuntimeMember as IXLangRuntimeProperty;
                scope.Declare(xLangRuntimeMember.Name, prop.PropertyType).SetValue(prop.GetValue(null));
            }
        }

        /// <summary>
        ///     Injects a Member into this type.
        /// </summary>
        /// <param name="member">Member to Inject</param>
        internal void InjectMember(IXLangRuntimeMember member)
        {
            members = members.Concat(new[] {member}).ToArray();
        }

        /// <summary>
        ///     To String Implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullName;
        }
    }
}
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
    public class XLangRuntimeType : IXLangScopeAccess
    {
        private readonly Func<XLangRuntimeType, IXLangRuntimeTypeInstance> createBase;

        private IXLangRuntimeMember[] members;

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

        public XLangRuntimeNamespace Namespace { get; }

        public XLangRuntimeType BaseType { get; }

        public string FullName => Namespace.FullName + "." + Name;

        public XLangMemberType ItemType => XLangMemberType.Class;

        public string Name { get; }

        public XLangAccessibilityLevel AccessibilityLevel =>
            (XLangAccessibilityLevel) (BindingFlags &
                                       (XLangBindingFlags.Public |
                                        XLangBindingFlags.Private |
                                        XLangBindingFlags.Protected));

        public XLangBindingFlags BindingFlags { get; }

        public void SetMembers(IXLangRuntimeMember[] members)
        {
            this.members = members;
        }

        public bool InheritsFrom(XLangRuntimeType type)
        {
            return type == this || BaseType != null && BaseType.InheritsFrom(type);
        }

        public virtual IXLangRuntimeTypeInstance CreateEmptyBase()
        {
            return createBase(this);
        }

        public bool HasMember(string name, XLangBindingQuery query)
        {
            return this.GetMembers(name, query).Length != 0;
        }

        public bool HasMember(XLangBindingQuery query)
        {
            return this.GetMembers(query).Length != 0;
        }

        public IXLangRuntimeMember[] GetAllMembers()
        {
            return BuildMemberList();
        }

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

        public IXLangRuntimeMember GetMember(string name, XLangBindingQuery query)
        {
            return this.GetMembers(name, query).FirstOrDefault();
        }

        public IXLangRuntimeMember GetMember(string name)
        {
            return members.FirstOrDefault(x => x.Name == name);
        }

        public IXLangRuntimeMember GetMember(XLangBindingQuery query)
        {
            return this.GetMembers(query).FirstOrDefault();
        }

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

        internal void InjectMember(IXLangRuntimeMember member)
        {
            members = members.Concat(new[] {member}).ToArray();
        }
    }
}
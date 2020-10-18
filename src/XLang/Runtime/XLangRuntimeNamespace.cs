using System.Collections.Generic;
using System.Linq;

using XLang.Core;
using XLang.Queries;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.Runtime
{
    public class XLangRuntimeNamespace : IXLangRuntimeItem
    {

        private readonly List<XLangRuntimeNamespace> children;
        public readonly string Name;

        private readonly XLangRuntimeNamespace parent;
        private readonly XLangSettings settings;

        public XLangRuntimeNamespace(
            string name, XLangRuntimeNamespace parent, List<XLangRuntimeType> types, XLangSettings settings)
        {
            Name = name;
            children = new List<XLangRuntimeNamespace>();
            this.parent = parent;
            Types = types;
            this.settings = settings;
        }

        protected virtual List<XLangRuntimeType> Types { get; }

        public IReadOnlyList<XLangRuntimeNamespace> Children => children.AsReadOnly();

        public IReadOnlyList<XLangRuntimeType> DefinedTypes => Types.AsReadOnly();

        public string FullName =>
            parent == null ? Name : parent.Name + settings.ReverseReservedSymbols[XLangTokenType.OpDot] + Name;

        public virtual bool HasType(string name, XLangBindingQuery query)
        {
            return this.GetTypes(name, query).Length != 0;
        }

        public virtual bool HasType(XLangBindingQuery query)
        {
            return this.GetTypes(query).Length != 0;
        }

        public virtual XLangRuntimeType GetType(string name, XLangBindingQuery query)
        {
            return this.GetTypes(name, query).FirstOrDefault();
        }

        public virtual XLangRuntimeType GetType(XLangBindingQuery query)
        {
            return this.GetTypes(query).FirstOrDefault();
        }

        public virtual XLangRuntimeType[] GetAllTypes()
        {
            return Types.ToArray();
        }

        public virtual void AddType(XLangRuntimeType typeDef)
        {
            if (DefinedTypes.Any(x => x.Name == typeDef.Name))
            {
                throw new XLangRuntimeTypeException(
                                                    $"A Type with the name '{typeDef.Name}' already exists in namespace '{FullName}'."
                                                   );
            }

            Types.Add(typeDef);
        }

        public XLangRuntimeNamespace[] GetAllNamespacesRecursive()
        {
            List<XLangRuntimeNamespace> types = new List<XLangRuntimeNamespace> { this };
            foreach (XLangRuntimeNamespace xLangRuntimeNamespace in Children)
            {
                types.AddRange(xLangRuntimeNamespace.GetAllNamespacesRecursive());
            }

            return types.ToArray();
        }

    }
}
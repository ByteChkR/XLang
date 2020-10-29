using System.Collections.Generic;
using System.Linq;
using XLang.Core;
using XLang.Exceptions;
using XLang.Queries;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.Runtime
{
    /// <summary>
    ///     XL namespace implementation
    /// </summary>
    public class XLangRuntimeNamespace : IXLangRuntimeItem
    {
        /// <summary>
        ///     Child Namespaces.
        /// </summary>
        private readonly List<XLangRuntimeNamespace> children;

        /// <summary>
        ///     Namespace Name
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     The Parent namespace. Null if root namespace.
        /// </summary>
        private readonly XLangRuntimeNamespace parent;

        /// <summary>
        ///     Settings that are used in the context this namespace is in.
        /// </summary>
        private readonly XLangSettings settings;

        /// <summary>
        ///     The defined using directives inside this namespace
        /// </summary>
        private readonly List<string> usingDirectives = new List<string> {"XL"};

        /// <summary>
        ///     Creates a new Namespace
        /// </summary>
        /// <param name="name">Namespace name</param>
        /// <param name="parent">Parent Namespace</param>
        /// <param name="types">Defined Types</param>
        /// <param name="settings">XL Settings</param>
        public XLangRuntimeNamespace(
            string name, XLangRuntimeNamespace parent, List<XLangRuntimeType> types, XLangSettings settings)
        {
            Name = name;
            children = new List<XLangRuntimeNamespace>();
            this.parent = parent;
            parent?.AddChildNamespace(this);
            Types = types;
            this.settings = settings;
        }

        /// <summary>
        ///     Defined Types in this namespace
        /// </summary>
        protected virtual List<XLangRuntimeType> Types { get; }

        /// <summary>
        ///     Child Namespaces
        /// </summary>
        public IReadOnlyList<XLangRuntimeNamespace> Children => children.AsReadOnly();

        /// <summary>
        ///     Defined Type in this namespace
        /// </summary>
        public IReadOnlyList<XLangRuntimeType> DefinedTypes => Types.AsReadOnly();

        /// <summary>
        ///     The Full name of this Namespace
        /// </summary>
        public string FullName =>
            parent == null ? Name : parent.Name + settings.ReverseReservedSymbols[XLangTokenType.OpDot] + Name;

        /// <summary>
        ///     Returns true if the type does exist with the specified bindings
        /// </summary>
        /// <param name="name">Name of the Type</param>
        /// <param name="query">Binding Flags</param>
        /// <returns></returns>
        public virtual bool HasType(string name, XLangBindingQuery query)
        {
            return this.GetTypes(name, query).Length != 0;
        }

        /// <summary>
        ///     Adds a Using Directive to this namespace.
        /// </summary>
        /// <param name="nsName">Name of the Namespace</param>
        public void AddUsing(string nsName)
        {
            if (!usingDirectives.Contains(nsName))
            {
                usingDirectives.Add(nsName);
            }
        }

        /// <summary>
        ///     Returns true if a type exists that satisfies the query
        /// </summary>
        /// <param name="query">Binding Query</param>
        /// <returns></returns>
        public virtual bool HasType(XLangBindingQuery query)
        {
            return this.GetTypes(query).Length != 0;
        }

        /// <summary>
        ///     Returns a Type with the specified name that fits the binding flags
        /// </summary>
        /// <param name="name">Name of the type</param>
        /// <param name="query">Binding Flags</param>
        /// <returns>Null if not found. Result if found.</returns>
        public virtual XLangRuntimeType GetType(string name, XLangBindingQuery query)
        {
            return this.GetTypes(name, query).FirstOrDefault();
        }

        /// <summary>
        ///     Returns a Type that fits the Binding Flags
        /// </summary>
        /// <param name="query">Binding Flags</param>
        /// <returns>Result</returns>
        public virtual XLangRuntimeType GetType(XLangBindingQuery query)
        {
            return this.GetTypes(query).FirstOrDefault();
        }

        /// <summary>
        ///     Returns all Types defined inside this namespace
        /// </summary>
        /// <returns>All Types</returns>
        public virtual XLangRuntimeType[] GetAllTypes()
        {
            return Types.ToArray();
        }

        /// <summary>
        ///     Adds a Type to this namespace.
        /// </summary>
        /// <param name="typeDef">Type to be added.</param>
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

        /// <summary>
        ///     Returns All visible namespaces.
        ///     (All Usings and self)
        /// </summary>
        /// <param name="context">Context of this Namespace</param>
        /// <returns>Array of visible namespaces.</returns>
        public XLangRuntimeNamespace[] GetVisibleNamespaces(XLangContext context)
        {
            return context.GetNamespaces().SelectMany(x => x.GetAllNamespacesRecursive())
                .Where(x => x == this || usingDirectives.Contains(x.FullName)).ToArray();
        }

        /// <summary>
        ///     Returns all child Namespaces below this namespace
        ///     Including this namespace
        /// </summary>
        /// <returns>Array of Namespaces</returns>
        public XLangRuntimeNamespace[] GetAllNamespacesRecursive()
        {
            List<XLangRuntimeNamespace> types = new List<XLangRuntimeNamespace> {this};
            foreach (XLangRuntimeNamespace xLangRuntimeNamespace in Children)
            {
                types.AddRange(xLangRuntimeNamespace.GetAllNamespacesRecursive());
            }

            return types.ToArray();
        }


        /// <summary>
        ///     Adds a Child Namespace to this namespace.
        /// </summary>
        /// <param name="ns">Namespace to add.</param>
        public void AddChildNamespace(XLangRuntimeNamespace ns)
        {
            children.Add(ns);
        }
    }
}
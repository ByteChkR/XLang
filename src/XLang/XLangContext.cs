using System;
using System.Collections.Generic;
using System.Linq;
using XLang.BaseTypes;
using XLang.Core;
using XLang.Exceptions;
using XLang.Queries;
using XLang.Runtime;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang
{
    /// <summary>
    ///     Context of a XL Script Execution.
    /// </summary>
    public class XLangContext
    {
        /// <summary>
        ///     All Namespaces that are imported/referenced by the current context.
        /// </summary>
        private readonly List<XLangRuntimeNamespace> allNamespaces = new List<XLangRuntimeNamespace>();
        /// <summary>
        ///     Default Imports as specified in constructor
        /// </summary>
        private readonly List<string> defaultImports;

        /// <summary>
        ///     The Default Namespace (Default name: DEFAULT)
        /// </summary>
        public readonly XLangRuntimeNamespace DefaultNamespace;

        /// <summary>
        ///     The XL Settings that are used in this context.
        /// </summary>
        public readonly XLangSettings Settings;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="settings">Settings Instance</param>
        /// <param name="defaultImports">The Automatically imported namespaces.</param>
        public XLangContext(XLangSettings settings, params string[] defaultImports)
        {
            this.defaultImports = defaultImports.ToList();
            Settings = settings;
            DefaultNamespace = new XLangRuntimeNamespace("DEFAULT", null, new List<XLangRuntimeType>(), settings);
            LoadNamespace(DefaultNamespace);
            XLangRuntimeNamespace coreNs = XLCoreNamespace.CreateNamespace(this);
            LoadNamespace(coreNs);

        }

        /// <summary>
        ///     Automatically implicitly imported namespaces
        /// </summary>
        public IReadOnlyCollection<string> DefaultImports => defaultImports.AsReadOnly();

        /// <summary>
        ///     Loads a Type into the default namespace in this context.
        /// </summary>
        /// <param name="type">Type to load.</param>
        public void LoadType(XLangRuntimeType type)
        {
            LoadType(DefaultNamespace, type);
        }

        /// <summary>
        ///     Loads a Type into this context under the specified namespace
        /// </summary>
        /// <param name="nameSpace">The target namespace of the Type</param>
        /// <param name="type">Type to load.</param>
        public void LoadType(XLangRuntimeNamespace nameSpace, XLangRuntimeType type)
        {
            nameSpace.AddType(type);
        }

        /// <summary>
        ///     Loads a Namespace into the current context.
        /// </summary>
        /// <param name="nameSpace">Namespace to load.</param>
        public void LoadNamespace(XLangRuntimeNamespace nameSpace)
        {
            if (!allNamespaces.Contains(nameSpace))
            {
                nameSpace.AddUsing(nameSpace.FullName);
                allNamespaces.Add(nameSpace);
            }
        }

        /// <summary>
        ///     Returns all visible types inside the specified namespaces.
        /// </summary>
        /// <param name="visibleNamespaces">Namespaces to be searched</param>
        /// <returns>Visible Types.</returns>
        public List<XLangRuntimeType> GetVisibleTypes(params XLangRuntimeNamespace[] visibleNamespaces)
        {
            List<XLangRuntimeType> ret = new List<XLangRuntimeType>();
            IEnumerable<XLangRuntimeNamespace> nss =
                visibleNamespaces.Concat(allNamespaces.Where(x => DefaultImports.Contains(x.FullName)));
            foreach (XLangRuntimeNamespace ns in nss)
            {
                ret.AddRange(ns.DefinedTypes);
            }

            ret.AddRange(DefaultNamespace.DefinedTypes);

            return ret;
        }


        /// <summary>
        ///     Returns a type by fully qualified name (Namespace.TypeName)
        /// </summary>
        /// <param name="fullyQualifiedType">Qualified Type Name</param>
        /// <returns>Type with the specified name</returns>
        public XLangRuntimeType GetType(string fullyQualifiedType)
        {
            int idx = fullyQualifiedType.LastIndexOf('.');
            string ns = fullyQualifiedType.Remove(idx, fullyQualifiedType.Length - idx);
            string name = fullyQualifiedType.Remove(0, idx + 1);
            if (TryGet(ns, out XLangRuntimeNamespace nSpace))
            {
                return nSpace.DefinedTypes.FirstOrDefault(x => x.Name == name);
            }

            return null;
        }

        /// <summary>
        ///     Tries to return the namespace with the selected name.
        /// </summary>
        /// <param name="name">Name of the searched namespace</param>
        /// <param name="result">Result of the Search. Null if not found</param>
        /// <returns>Returns true if the namespace has been found.</returns>
        public bool TryGet(string name, out XLangRuntimeNamespace result)
        {
            XLangRuntimeNamespace closest = null;
            foreach (XLangRuntimeNamespace root in GetNamespaces())
            {
                closest = VisitRecursive(
                    root,
                    ns =>
                    {
                        string nsn = ns.FullName;
                        return name.StartsWith(nsn + ".");
                    },
                    ns =>
                    {
                        string nsn = ns.FullName;
                        return name == nsn ||
                               ns.Children.Count == 0 &&
                               name.StartsWith(nsn + ".");
                    }
                );
                if (closest != null)
                {
                    break;
                }
            }


            result = closest;

            return closest != null && closest.FullName == name;
        }

        /// <summary>
        ///     Returns a namespace with the specified name
        ///     If the namespace does not exist, it is created.
        /// </summary>
        /// <param name="name">Name of the Namespace</param>
        /// <returns>Namespace with the correct name</returns>
        public XLangRuntimeNamespace CreateOrGet(string name)
        {
            XLangRuntimeNamespace closest = null;
            if (DefaultNamespace.Name == name)
            {
                return DefaultNamespace;
            }
            foreach (XLangRuntimeNamespace root in allNamespaces)
            {
                closest = VisitRecursive(
                    root,
                    ns =>
                    {
                        string nsn = ns.FullName;
                        return name.StartsWith(nsn + ".");
                    },
                    ns =>
                    {
                        string nsn = ns.FullName;
                        return name == nsn ||
                               ns.Children.Count == 0 &&
                               name.StartsWith(nsn + ".");
                    }
                );
                if (closest != null)
                {
                    break;
                }
            }


            if (closest != null && closest.FullName == name)
            {
                return closest;
            }

            string startName = closest?.FullName ?? "";

            string[] parts = null;
            if (startName == "")
            {
                parts = name.Split('.');
            }
            else
            {
                parts = name.Remove(0, (startName + ".").Length).Split('.');
            }

            XLangRuntimeNamespace current = closest;
            for (int i = 0; i < parts.Length; i++)
            {
                XLangRuntimeNamespace next =
                    new XLangRuntimeNamespace(parts[i], current, new List<XLangRuntimeType>(), Settings);
                if (current == null)
                {
                    LoadNamespace(next);
                }
                current = next;
            }

            return current;
        }

        /// <summary>
        ///     Recursive Search Function that does search namespaces based on "enter" and "accept" functions.
        /// </summary>
        /// <param name="current">The current namespace that is beeing searched</param>
        /// <param name="enter">Returns true if the search should recurse into the passed namespace</param>
        /// <param name="accept">Returns true if the search should terminate when a namespace is found.</param>
        /// <returns>The Result of the Search</returns>
        private XLangRuntimeNamespace VisitRecursive(
            XLangRuntimeNamespace current, Func<XLangRuntimeNamespace, bool> enter,
            Func<XLangRuntimeNamespace, bool> accept)
        {
            if (accept(current))
            {
                return current;
            }

            foreach (XLangRuntimeNamespace child in current.Children)
            {
                if (enter(child))
                {
                    XLangRuntimeNamespace token = VisitRecursive(child, enter, accept);
                    if (token != null)
                    {
                        return current;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns All loaded namespaces in this context.
        /// </summary>
        /// <returns>Enumerable of XLRuntimeNamespace</returns>
        public IEnumerable<XLangRuntimeNamespace> GetNamespaces()
        {
            return allNamespaces;
        }

        /// <summary>
        ///     Returns the Unary operator for the specified type and token.
        /// </summary>
        /// <param name="leftType">Type to operate on</param>
        /// <param name="type">Token Type that defines the Operation</param>
        /// <returns>The function implementation</returns>
        public IXLangRuntimeFunction GetUnaryOperatorImplementation(
            XLangRuntimeType leftType, XLangTokenType type)
        {
            if (TryGetUnaryOperatorImplementation(leftType, type, out IXLangRuntimeFunction result))
            {
                return result;
            }

            throw new XLangRuntimeTypeException(
                $"No operator implementation for operator: '{type}' on type '{leftType.FullName}'"
            );
        }


        /// <summary>
        ///     Returns the Unary operator for the specified type and token.
        /// </summary>
        /// <param name="leftType">Type to operate on</param>
        /// <param name="type">Token Type that defines the Operation</param>
        /// <param name="operatorImpl">Function implementation if found-</param>
        /// <returns>Returns true if the implementation is defined.</returns>
        public bool TryGetUnaryOperatorImplementation(
            XLangRuntimeType leftType,
            XLangTokenType type, out IXLangRuntimeFunction operatorImpl)
        {
            IEnumerable<IXLangScopeAccess> items = leftType.GetMembers(
                type.ToString(),
                XLangBindingQuery.Override |
                XLangBindingQuery.Operator |
                XLangBindingQuery.Static
            );
            operatorImpl = items.Cast<IXLangRuntimeFunction>()
                .FirstOrDefault(
                    x => x.ParameterList.Length == 1 &&
                         x.ParameterList.First().Type == leftType
                );
            return operatorImpl != null;
        }


        /// <summary>
        ///     Returns the Binary operator for the specified types and token.
        /// </summary>
        /// <param name="leftType">Left Type to operate on</param>
        /// <param name="rightType">Right Type to operate on</param>
        /// <param name="type">Token Type that defines the Operation</param>
        /// <returns>Returns true if the implementation is defined.</returns>
        public IXLangRuntimeFunction GetBinaryOperatorImplementation(
            XLangRuntimeType leftType, XLangRuntimeType rightType, XLangTokenType type)
        {
            if (TryGetBinaryOperatorImplementation(leftType, rightType, type, out IXLangRuntimeFunction result))
            {
                return result;
            }

            throw new XLangRuntimeTypeException(
                $"No operator implementation for operator: '{type}' on types '{leftType.FullName}',  '{rightType.FullName}'"
            );
        }

        /// <summary>
        ///     Returns the Binary operator for the specified type and token.
        /// </summary>
        /// <param name="leftType">Left Type to operate on</param>
        /// <param name="rightType">Right Type to operate on</param>
        /// <param name="type">Token Type that defines the Operation</param>
        /// <param name="operatorImpl">Function implementation if found-</param>
        /// <returns>Returns true if the implementation is defined.</returns>
        public bool TryGetBinaryOperatorImplementation(
            XLangRuntimeType leftType, XLangRuntimeType rightType,
            XLangTokenType type, out IXLangRuntimeFunction operatorImpl)
        {
            operatorImpl = leftType?.GetMembers(
                    type.ToString(),
                    XLangBindingQuery.Override |
                    XLangBindingQuery.Operator |
                    XLangBindingQuery.Static
                ).Cast<IXLangRuntimeFunction>()
                .FirstOrDefault(
                    x => x.ParameterList.Length == 2 &&
                         leftType.InheritsFrom(x.ParameterList.First().Type) &&
                         rightType.InheritsFrom(x.ParameterList.Skip(1).First().Type)
                );
            return operatorImpl != null;
        }
    }
}
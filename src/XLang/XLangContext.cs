using System;
using System.Collections.Generic;
using System.Linq;
using XLang.BaseTypes;
using XLang.Core;
using XLang.Queries;
using XLang.Runtime;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang
{
    public class XLangContext
    {
        private readonly List<string> defaultImports;
        public readonly XLangRuntimeNamespace DefaultNamespace;

        private readonly List<XLangRuntimeNamespace> loadedNamespaces = new List<XLangRuntimeNamespace>();

        public readonly XLangSettings Settings;

        public XLangContext(XLangSettings settings, params string[] defaultImports)
        {
            this.defaultImports = defaultImports.ToList();
            Settings = settings;
            DefaultNamespace = new XLangRuntimeNamespace("DEFAULT", null, new List<XLangRuntimeType>(), settings);
            LoadNamespace(XLCoreNamespace.CreateNamespace(settings));
        }

        public IReadOnlyCollection<string> DefaultImports => defaultImports.AsReadOnly();

        public void LoadType(XLangRuntimeType type)
        {
            LoadType(DefaultNamespace, type);
        }

        public void LoadType(XLangRuntimeNamespace nameSpace, XLangRuntimeType type)
        {
            nameSpace.AddType(type);
        }

        public void LoadNamespace(XLangRuntimeNamespace nameSpace)
        {
            loadedNamespaces.Add(nameSpace);
        }

        public List<XLangRuntimeType> GetVisibleTypes(params XLangRuntimeNamespace[] visibleNamespaces)
        {
            List<XLangRuntimeType> ret = new List<XLangRuntimeType>();
            IEnumerable<XLangRuntimeNamespace> nss =
                visibleNamespaces.Concat(loadedNamespaces.Where(x => DefaultImports.Contains(x.FullName)));
            foreach (XLangRuntimeNamespace ns in nss)
            {
                ret.AddRange(ns.DefinedTypes);
            }

            ret.AddRange(DefaultNamespace.DefinedTypes);

            return ret;
        }

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

        public XLangRuntimeNamespace CreateOrGet(string name)
        {
            XLangRuntimeNamespace closest = null;
            foreach (XLangRuntimeNamespace root in loadedNamespaces)
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

            string[] parts = name.Remove(0, (startName + ".").Length).Split('.');

            XLangRuntimeNamespace current = closest;
            for (int i = 0; i < parts.Length; i++)
            {
                XLangRuntimeNamespace next =
                    new XLangRuntimeNamespace(parts[i], current, new List<XLangRuntimeType>(), Settings);
                current = next;
            }

            return current;
        }

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

        public IEnumerable<XLangRuntimeNamespace> GetNamespaces()
        {
            return loadedNamespaces.Concat(new[] {DefaultNamespace});
        }

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
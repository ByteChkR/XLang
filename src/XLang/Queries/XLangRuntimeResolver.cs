using System;
using System.Linq;
using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

namespace XLang.Queries
{
    public static class XLangRuntimeResolver
    {
        public static IXLangRuntimeMember ResolveDynamicItem(XLangContext context, string name, XLangRuntimeType type)
        {
            string[] parts = name.Split('.');
            int current = 1;

            XLangRuntimeType currentItem = type;
            IXLangRuntimeMember currentMember = null;
            do
            {
                IXLangRuntimeMember member = currentItem.GetMember(
                    parts[current],
                    XLangBindingQuery.Public |
                    XLangBindingQuery.Private |
                    XLangBindingQuery.Instance |
                    XLangBindingQuery.Static |
                    XLangBindingQuery.Inclusive
                );


                if (member is IXLangRuntimeProperty prop)
                {
                    currentItem = prop.PropertyType;
                    currentMember = member;
                }
                else if (member is IXLangRuntimeFunction func)
                {
                    currentItem = func.ReturnType;
                    currentMember = member;
                }

                current++;
            } while (current < parts.Length);

            return currentMember;
        }

        private static XLangRuntimeNamespace GetNamespace(XLangContext context, XLangRuntimeNamespace start,
            string name)
        {
            if (start == null)
            {
                return context.GetNamespaces().FirstOrDefault(x => x.Name == name);
            }

            return start.Children.FirstOrDefault(x => x.Name == name);
        }

        private static XLangRuntimeType GetType(XLangRuntimeScope scope, XLangRuntimeNamespace start, string name,
            XLangRuntimeType caller)
        {
            if (start == null)
            {
                return caller.Namespace.GetVisibleNamespaces(scope.Context).Distinct().SelectMany(x => x.DefinedTypes)
                    .FirstOrDefault(x => x.Name == name);
                //return scope.GetAllVisible().Concat(new[] {caller.Namespace}).Distinct().SelectMany(x => x.DefinedTypes)
                //    .FirstOrDefault(x => x.Name == name);
            }

            return start.DefinedTypes.FirstOrDefault(x => x.Name == name);
        }

        public static IXLangRuntimeItem[] ResolveItem(XLangRuntimeScope scope, string name, IXLangRuntimeItem start,
            XLangRuntimeType caller)
        {
            if (start == null)
            {
                XLangRuntimeType type = GetType(scope, null, name, caller);

                if (type != null)
                {
                    if ((type.BindingFlags & XLangBindingFlags.Private) != 0)
                    {
                        if (caller == type)
                        {
                            return new []{ type };
                        }
                        throw new Exception($"Type '{type}' is not accessible from caller '{caller}'");
                    }

                    return new []{ type };
                }

                XLangRuntimeNamespace ns = GetNamespace(scope.Context, null, name);
                if (ns != null)
                {
                    return new []{ ns };
                }


                IXLangRuntimeMember[] member = caller.GetMembers(
                    name,
                    XLangBindingQuery.Static |
                    XLangBindingQuery.Private |
                    XLangBindingQuery.Public |
                    XLangBindingQuery.Protected |
                    XLangBindingQuery.Function |
                    XLangBindingQuery.Property |
                    XLangBindingQuery.Inclusive
                );
                if (member != null && member.Length!=0)
                {
                    return member;
                }

                throw new Exception("Can not find Type or namespace: " + name);
            }

            if (start is XLangRuntimeNamespace nSpace)
            {
                XLangRuntimeType type = GetType(scope, nSpace, name, caller);
                if (type != null)
                {
                    return new []{ type };
                }

                XLangRuntimeNamespace ns = GetNamespace(scope.Context, nSpace, name);
                if (ns != null)
                {
                    return new []{ ns };
                }

                throw new Exception("Can not find Type or namespace: " + name);
            }

            if (start is XLangRuntimeType rType)
            {
                XLangBindingQuery query = rType == caller
                    ? XLangBindingQuery.Private |
                      XLangBindingQuery.Public |
                      XLangBindingQuery.Protected |
                      XLangBindingQuery.Property |
                      XLangBindingQuery.Function |
                      XLangBindingQuery.Inclusive
                    : caller.InheritsFrom(rType)
                        ? XLangBindingQuery.Protected |
                          XLangBindingQuery.Public |
                          XLangBindingQuery.Property |
                          XLangBindingQuery.Function |
                          XLangBindingQuery.Inclusive
                        : XLangBindingQuery.Public |
                          XLangBindingQuery.Property |
                          XLangBindingQuery.Function |
                          XLangBindingQuery.Inclusive;
                IXLangRuntimeMember[] member = rType.GetMembers(name, query);
                if (member != null)
                {
                    return member;
                }
                throw new Exception(
                    $"Can not find Property or Function: {name} or it is not accessible from this scope"
                );
            }

            if (start is IXLangRuntimeFunction func)
            {
                return ResolveItem(scope, name, func.ReturnType, caller);
            }

            if (start is IXLangRuntimeProperty prop)
            {
                return ResolveItem(scope, name, prop.PropertyType, caller);
            }
            throw new Exception(
                $"Invalid Input: {name}"
            );
        }


        public static IXLangScopeAccess ResolveStaticItem(
            XLangContext context, string name, IXLangScopeAccess start, IXLangRuntimeTypeInstance instance,
            params XLangRuntimeNamespace[] visibleNamespaces)
        {
            string[] parts = name.Split('.');
            int current = 0;
            IXLangScopeAccess currentItem = start;

            (XLangRuntimeNamespace ns, Exception ex) = GetClosestNamespace(context, parts);

            if (start != null)
            {
                current++;
            }
            else if (ex != null)
            {
                if (instance != null && parts.Length == 1)
                {
                    currentItem = instance.Type.GetMember(name);
                }

                if (currentItem == null)
                {
                    currentItem = context.GetVisibleTypes(visibleNamespaces)
                        .FirstOrDefault(x => x.Name == parts.First());
                }

                if (currentItem == null)
                {
                    throw ex;
                }

                current++;
            }

            while (current < parts.Length)
            {
                if (currentItem == null)
                {
                    currentItem = ns.GetType(
                        parts[current],
                        XLangBindingQuery.Public |
                        XLangBindingQuery.Private |
                        XLangBindingQuery.Instance |
                        XLangBindingQuery.Static |
                        XLangBindingQuery.Inclusive
                    );
                }
                else if (currentItem is XLangRuntimeType type)
                {
                    currentItem = type.GetMember(
                        parts[current],
                        XLangBindingQuery.Public |
                        XLangBindingQuery.Private |
                        XLangBindingQuery.Instance |
                        XLangBindingQuery.Static |
                        XLangBindingQuery.Inclusive
                    );
                }
                else if (currentItem is IXLangRuntimeFunction func)
                {
                    currentItem = func.ReturnType;
                }
                else if (currentItem is IXLangRuntimeProperty prop)
                {
                    currentItem = prop.PropertyType;
                }
                else
                {
                    throw new Exception("F");
                }

                current++;
            }

            return currentItem;
        }


        private static (IXLangRuntimeMember, Exception) GetMember(XLangContext context, string[] parts)
        {
            string[] typeParts = parts.Take(parts.Length - 1).ToArray();

            (XLangRuntimeType type, Exception ex) = GetType(context, typeParts);
            if (ex != null)
            {
                return (null, ex);
            }

            return (type.GetMember(parts.Last()), null);
        }

        private static (XLangRuntimeType, Exception) GetType(XLangContext context, string[] parts)
        {
            (XLangRuntimeNamespace nameSpace, Exception ex) = GetClosestNamespace(context, parts);

            if (ex != null)
            {
                return (null, ex);
            }

            string[] nsParts = nameSpace.FullName.Split('.');

            if (nsParts.Length == parts.Length)
            {
                return (null, new Exception("The Word does not refer to a type"));
            }

            if (nsParts.Length < parts.Length - 1)
            {
                return (null, new Exception($"Could not find Namespace '{nameSpace.FullName}.{parts.LastOrDefault()}"));
            }

            XLangRuntimeType type = nameSpace.GetAllTypes().FirstOrDefault(x => x.Name == parts.Last());

            return (type,
                type == null
                    ? new Exception(
                        $"Could not find type '{parts.LastOrDefault()}' in namespace '{nameSpace.FullName}'"
                    )
                    : null);
        }

        private static (XLangRuntimeNamespace, Exception) GetClosestNamespace(XLangContext context, string[] parts)
        {
            XLangRuntimeNamespace current =
                context.GetNamespaces().FirstOrDefault(x => x.Name == parts.FirstOrDefault());

            if (current == null)
            {
                return (null, new Exception($"Could not find Namespace: '{parts.FirstOrDefault()}'"));
            }

            if (parts.Length == 1)
            {
                return (current, null);
            }

            XLangRuntimeNamespace[] nss = context.GetNamespaces().ToArray();
            for (int i = 0; i < parts.Length; i++)
            {
                for (int j = 0; j < nss.Length; j++)
                {
                    if (parts[i] == nss[j].Name)
                    {
                        current = nss[j];
                        nss = current.Children.ToArray();
                        break;
                    }
                }
            }

            return (current, null);
        }
    }
}
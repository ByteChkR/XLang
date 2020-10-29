using System.Collections.Generic;
using XLang.Runtime;
using XLang.Runtime.Members;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

/// <summary>
/// Contains Static Query APIs
/// </summary>
namespace XLang.Queries
{
    /// <summary>
    ///     XLang Binding Resolver
    ///     Resolves IXLangScopeAccess items.
    /// </summary>
    public static class XLangBindingResolver
    {
        /// <summary>
        ///     Returns the Subset of Items that fit the binding Query and name.
        /// </summary>
        /// <typeparam name="T">Type of the Item</typeparam>
        /// <param name="collection">Search Collection</param>
        /// <param name="name">Name of the Target ITems</param>
        /// <param name="query">The Search Query.</param>
        /// <returns>Result Subset.</returns>
        private static T[] Query<T>(IEnumerable<T> collection, string name, XLangBindingQuery query)
            where T : IXLangScopeAccess
        {
            XLangBindingQuery exactMask = XLangBindingQuery.Property |
                                          XLangBindingQuery.Function |
                                          XLangBindingQuery.Exact |
                                          XLangBindingQuery.Inclusive |
                                          XLangBindingQuery.MatchType |
                                          XLangBindingQuery.Class |
                                          XLangBindingQuery.Constructor;

            XLangBindingQuery matchMode =
                query & (XLangBindingQuery.Exact | XLangBindingQuery.Inclusive | XLangBindingQuery.MatchType);
            XLangBindingQuery mType =
                query &
                (XLangBindingQuery.Function |
                 XLangBindingQuery.Property |
                 XLangBindingQuery.Class |
                 XLangBindingQuery.Constructor);


            XLangBindingQuery queryType = query & ~exactMask;

            bool IsTarget(T member)
            {
                if (mType == 0)
                {
                    return true;
                }

                return (mType & (XLangBindingQuery) member.ItemType) != 0;
            }

            List<T> ret = new List<T>();
            foreach (T member in collection)
            {
                if (!IsTarget(member) || name != null && name != member.Name)
                {
                    continue;
                }

                if (matchMode == XLangBindingQuery.Exact)
                {
                    if ((XLangBindingQuery) member.BindingFlags == queryType)
                    {
                        ret.Add(member);
                    }
                }
                else if (matchMode == XLangBindingQuery.Inclusive)
                {
                    if (((XLangBindingQuery) member.BindingFlags & queryType) != 0)
                    {
                        ret.Add(member);
                    }
                }
                else if (matchMode == XLangBindingQuery.MatchType)
                {
                    if (((XLangBindingQuery) member.BindingFlags & queryType) ==
                        (XLangBindingQuery) member.BindingFlags)
                    {
                        ret.Add(member);
                    }
                }
                else
                {
                    if (((XLangBindingQuery) member.BindingFlags & queryType) == queryType)
                    {
                        ret.Add(member);
                    }
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        ///     Returns all Members that fit the Search Query.
        /// </summary>
        /// <param name="type">This Type</param>
        /// <param name="query">Search Query.</param>
        /// <returns>Result Subset.</returns>
        public static IXLangRuntimeMember[] GetMembers(this XLangRuntimeType type, XLangBindingQuery query)
        {
            return GetMembers(type, null, query);
        }

        /// <summary>
        ///     Returns all Members that fit the name and Search Query.
        /// </summary>
        /// <param name="type">This Type</param>
        /// <param name="name">Name of the Member</param>
        /// <param name="query">Search Query.</param>
        /// <returns>Result Subset.</returns>
        public static IXLangRuntimeMember[] GetMembers(this XLangRuntimeType type, string name, XLangBindingQuery query)
        {
            return Query(type.GetAllMembers(), name, query);
        }

        /// <summary>
        ///     Returns all Types inside this namespace that fit the search query.
        /// </summary>
        /// <param name="nameSpace">This Namespace</param>
        /// <param name="query">Search Query</param>
        /// <returns>Result Subset.</returns>
        public static XLangRuntimeType[] GetTypes(this XLangRuntimeNamespace nameSpace, XLangBindingQuery query)
        {
            return GetTypes(nameSpace, null, query);
        }

        /// <summary>
        /// </summary>
        /// <param name="nameSpace">Namespace to be Searched</param>
        /// <param name="name">Search Name</param>
        /// <param name="query">Search Query</param>
        /// <returns>Result Subset.</returns>
        public static XLangRuntimeType[] GetTypes(
            this XLangRuntimeNamespace nameSpace, string name, XLangBindingQuery query)
        {
            return Query(nameSpace.GetAllTypes(), name, query);
        }
    }
}
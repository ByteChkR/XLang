using System.Collections.Generic;

using XLang.Runtime;
using XLang.Runtime.Members;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

namespace XLang.Queries
{
    public static class XLangBindingResolver
    {

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

        public static IXLangRuntimeMember[] GetMembers(this XLangRuntimeType type, XLangBindingQuery query)
        {
            return GetMembers(type, null, query);
        }

        public static IXLangRuntimeMember[] GetMembers(this XLangRuntimeType type, string name, XLangBindingQuery query)
        {
            return Query(type.GetAllMembers(), name, query);
        }

        public static XLangRuntimeType[] GetTypes(this XLangRuntimeNamespace nameSpace, XLangBindingQuery query)
        {
            return GetTypes(nameSpace, null, query);
        }

        public static XLangRuntimeType[] GetTypes(
            this XLangRuntimeNamespace nameSpace, string name, XLangBindingQuery query)
        {
            return Query(nameSpace.GetAllTypes(), name, query);
        }

    }
}
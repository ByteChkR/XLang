using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.CSharp
{
    public static class CSharpClassTunnel
    {
        public static void LoadTunnel(XLangContext context)
        {
            XLangRuntimeNamespace tns = context.CreateOrGet("CSharp");
            XLangRuntimeType voidType = context.GetType("XL.void");
            XLangRuntimeType strType = context.GetType("XL.string");
            XLangRuntimeType tunnelType = new XLangRuntimeType("Tunnel", tns, null, XLangBindingFlags.Static | XLangBindingFlags.Public, type => null);
            DelegateXLFunction loadType = new DelegateXLFunction("LoadType",
                (instance, instances) => FuncLoadType(context, instance, instances), voidType,
                XLangMemberFlags.Public | XLangMemberFlags.Static, tunnelType,
                new XLangFunctionArgument("typeName", strType), new XLangFunctionArgument("targetNs", strType));
            DelegateXLFunction loadNs = new DelegateXLFunction("LoadNamespace",
                (instance, instances) => FuncLoadNamespace(context, instance, instances), voidType,
                XLangMemberFlags.Public | XLangMemberFlags.Static, tunnelType,
                new XLangFunctionArgument("targetNs", strType));
            DelegateXLFunction loadAll = new DelegateXLFunction("LoadAll",
                (instance, instances) => FuncLoadAll(context, instance, instances), voidType,
                XLangMemberFlags.Public | XLangMemberFlags.Static, tunnelType,
                new XLangFunctionArgument("targetNs", strType));
            tunnelType.SetMembers(new []{loadAll, loadNs, loadType});
            tns.AddType(tunnelType);
        }


        private static IXLangRuntimeTypeInstance FuncLoadType(XLangContext context, IXLangRuntimeTypeInstance instance,
            IXLangRuntimeTypeInstance[] args)
        {
            Type t = Type.GetType(args[0].GetRaw().ToString());
            XLangBindingQuery q = XLangBindingQuery.Public;
            if (t.IsAbstract && t.IsSealed)
                q |= XLangBindingQuery.Static;
            GetType(context, t,
                context.CreateOrGet(args[1].GetRaw().ToString()), q);
            return null;
        }

        private static IXLangRuntimeTypeInstance FuncLoadNamespace(XLangContext context, IXLangRuntimeTypeInstance instance,
            IXLangRuntimeTypeInstance[] args)
        {
            Type[] ts = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.Namespace == args[0].GetRaw().ToString()).ToArray();
            XLangRuntimeNamespace ns = context.CreateOrGet(args[0].GetRaw().ToString());
            foreach (Type t in ts)
            {
                XLangBindingQuery q = XLangBindingQuery.Public;
                if (t.IsAbstract && t.IsSealed)
                    q |= XLangBindingQuery.Static;
                GetType(context, t, ns, q);
            }

            return null;
        }

        private static IXLangRuntimeTypeInstance FuncLoadAll(XLangContext context, IXLangRuntimeTypeInstance instance,
            IXLangRuntimeTypeInstance[] args)
        {
            Type[] ts = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToArray();
            XLangRuntimeNamespace ns = context.CreateOrGet(args[0].GetRaw().ToString());
            foreach (Type t in ts)
            {
                XLangBindingQuery q = XLangBindingQuery.Public;
                if (t.IsAbstract && t.IsSealed)
                    q |= XLangBindingQuery.Static;
                GetType(context, t, ns, q);
            }

            return null;
        }

        private static bool IsNumericType(this Type o)
        {
            switch (Type.GetTypeCode(o))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }

        private static void AddStaticClass(XLangContext context, Type type, XLangRuntimeNamespace targetNs)
        {
            GetType(context, type, targetNs);
        }

        private static void AddInstanceClass(XLangContext context, Type type, XLangRuntimeNamespace targetNs)
        {
            GetType(context, type, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Instance);
        }


        private static XLangRuntimeType GetType(XLangContext context, Type type, XLangRuntimeNamespace targetNs, XLangBindingQuery flags = XLangBindingQuery.Public | XLangBindingQuery.Static)
        {
            if (type.IsArray) return null;
            if (type == typeof(object)) return context.GetType("XL.object");
            if (type == typeof(string)) return context.GetType("XL.string");
            if (type.IsNumericType()) return context.GetType("XL.number");
            if (targetNs.HasType(type.Name, XLangBindingQuery.Public | XLangBindingQuery.Class)) return targetNs.GetType(type.Name, flags);

            XLangRuntimeType t = null;
            t = new XLangRuntimeType(type.Name, targetNs, null, (XLangBindingFlags)flags, null);
            targetNs.AddType(t);
            List<IXLangRuntimeMember> members = GetStaticMembers(t, context, type, targetNs);
            members.AddRange(GetInstanceMembers(t, context, type, targetNs));
            t.SetMembers(members.ToArray());
            return t;
        }

        private static List<IXLangRuntimeMember> GetStaticMembers(XLangRuntimeType implClass, XLangContext context, Type t, XLangRuntimeNamespace targetNs)
        {
            List<IXLangRuntimeMember> ret = new List<IXLangRuntimeMember>();
            PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Static);
            ret.AddRange(ps.Select(x => ProcessProperty(implClass, context, x, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Static)));
            MethodInfo[] ms = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
            ret.AddRange(ms.Select(x => ProcessMethod(implClass, context, x, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Static)));
            ret.RemoveAll(x => x == null);

            return ret;
        }

        private static List<IXLangRuntimeMember> GetInstanceMembers(XLangRuntimeType implClass, XLangContext context, Type t, XLangRuntimeNamespace targetNs)
        {
            List<IXLangRuntimeMember> ret = new List<IXLangRuntimeMember>();
            PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            ret.AddRange(ps.Select(x => ProcessProperty(implClass, context, x, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Instance)));
            MethodInfo[] ms = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            ret.AddRange(ms.Select(x => ProcessMethod(implClass, context, x, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Instance)));
            ret.RemoveAll(x => x == null);
            return ret;
        }

        private static IXLangRuntimeMember ProcessProperty(XLangRuntimeType implClass, XLangContext context, PropertyInfo pi, XLangRuntimeNamespace targetNs, XLangBindingQuery flags)
        {
            XLangRuntimeType type = GetType(context, pi.PropertyType, targetNs, flags & ~XLangBindingQuery.Static);
            if (type == null) return null;
            return new DelegateXLProperty(pi.Name, instance => GetInstance(context, pi, instance, targetNs), type, (XLangMemberFlags)flags, implClass);
        }

        private static IXLangRuntimeMember ProcessMethod(XLangRuntimeType implClass, XLangContext context, MethodInfo mi, XLangRuntimeNamespace targetNs, XLangBindingQuery flags)
        {
            XLangRuntimeType type = GetType(context, mi.ReturnType, targetNs, flags & ~XLangBindingQuery.Static);
            if (type == null) return null;
            return new DelegateXLFunction(mi.Name, (x, y) => InvokeFunc(context, mi, targetNs, x, y), type, (XLangMemberFlags)flags, implClass);
        }

        private static IXLangRuntimeTypeInstance InvokeFunc(XLangContext context, MethodInfo mi, XLangRuntimeNamespace targetNs, IXLangRuntimeTypeInstance arg1, IXLangRuntimeTypeInstance[] arg2)
        {
            XLangRuntimeType t = GetType(context, mi.ReturnType, targetNs, XLangBindingQuery.Public);
            return new CSharpTypeInstance(t, mi.Invoke(arg1?.GetRaw(), arg2.Select(x => x.GetRaw()).ToArray()));
        }

        private static IXLangRuntimeTypeInstance GetInstance(XLangContext context, PropertyInfo pi, IXLangRuntimeTypeInstance instance, XLangRuntimeNamespace targetNs)
        {
            XLangRuntimeType t = GetType(context, pi.PropertyType, targetNs, XLangBindingQuery.Public);
            return new CSharpTypeInstance(t, pi.GetValue(instance.GetRaw()));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;


/// <summary>
/// Contains CSharp Interop Logic
/// </summary>
namespace XLang.CSharp
{
    /// <summary>
    ///     Implements a C# Class Tunnel Service that allows for C# Types and Namespaces to be exposed inside the XL VM
    /// </summary>
    public static class CSharpClassTunnel
    {
        /// <summary>
        ///     Loads the Tunnel System into the Specified Context.
        /// </summary>
        /// <param name="context">Specified Context.</param>
        public static void LoadTunnel(XLangContext context)
        {
            XLangRuntimeNamespace tns = context.CreateOrGet("CSharp");
            XLangRuntimeType voidType = context.GetType("XL.void");
            XLangRuntimeType strType = context.GetType("XL.string");
            XLangRuntimeType tunnelType = new XLangRuntimeType("Tunnel", tns, null,
                XLangBindingFlags.Static | XLangBindingFlags.Public, type => null);
            DelegateXLFunction loadType = new DelegateXLFunction("LoadType",
                (instance, instances) => FuncLoadType(context, instance, instances), voidType,
                XLangMemberFlags.Public | XLangMemberFlags.Static, tunnelType,
                new XLangFunctionArgument("typeName", strType), new XLangFunctionArgument("targetNs", strType));
            DelegateXLFunction loadNs = new DelegateXLFunction("LoadNamespace",
                (instance, instances) => FuncLoadNamespace(context, instances), voidType,
                XLangMemberFlags.Public | XLangMemberFlags.Static, tunnelType,
                new XLangFunctionArgument("targetNs", strType));
            DelegateXLFunction loadAll = new DelegateXLFunction("LoadAll",
                (instance, instances) => FuncLoadAll(context, instances), voidType,
                XLangMemberFlags.Public | XLangMemberFlags.Static, tunnelType,
                new XLangFunctionArgument("targetNs", strType));
            tunnelType.SetMembers(new[] {loadAll, loadNs, loadType});
            tns.AddType(tunnelType);
        }

        /// <summary>
        ///     Returns a C# Type by Qualified Name
        /// </summary>
        /// <param name="qualifiedName">Name</param>
        /// <returns>Type that was found</returns>
        private static Type GetType(string qualifiedName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.FullName == qualifiedName);
        }

        /// <summary>
        ///     Function that implements Tunnel.LoadType(TypeName, TargetNamespace)
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="instance">Instance of the Implementing Class (this function is static)</param>
        /// <param name="args">Function arguments (0: Type Name 1: Target namespace.</param>
        /// <returns>The Return instance.(null)</returns>
        private static IXLangRuntimeTypeInstance FuncLoadType(XLangContext context, IXLangRuntimeTypeInstance instance,
            IXLangRuntimeTypeInstance[] args)
        {
            Type t = GetType(args[0].GetRaw().ToString());

            XLangBindingQuery q = XLangBindingQuery.Public;
            if (t.IsAbstract && t.IsSealed)
            {
                q |= XLangBindingQuery.Static;
            }
            GetType(context, t,
                context.CreateOrGet(args[1].GetRaw().ToString()), q);
            return null;
        }

        /// <summary>
        ///     Function that implements Tunnel.LoadNamespace(TargetNamespace)
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="instance">Instance of the Implementing Class (this function is static)</param>
        /// <param name="args">Function arguments (0: Target namespace.</param>
        /// <returns>The Return instance.(null)</returns>
        private static IXLangRuntimeTypeInstance FuncLoadNamespace(XLangContext context,
            IXLangRuntimeTypeInstance[] args)
        {
            Type[] ts = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.Namespace == args[0].GetRaw().ToString()).ToArray();
            XLangRuntimeNamespace ns = context.CreateOrGet(args[0].GetRaw().ToString());
            foreach (Type t in ts)
            {
                XLangBindingQuery q = XLangBindingQuery.Public;
                if (t.IsAbstract && t.IsSealed)
                {
                    q |= XLangBindingQuery.Static;
                }
                GetType(context, t, ns, q);
            }

            return null;
        }

        /// <summary>
        ///     Loads all Namespaces from all available Assemblies.
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="instance">Instance of the Implementing Class</param>
        /// <param name="args">Function Arguments</param>
        /// <returns>Return Value (null)</returns>
        private static IXLangRuntimeTypeInstance FuncLoadAll(XLangContext context,
            IXLangRuntimeTypeInstance[] args)
        {
            Type[] ts = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToArray();
            XLangRuntimeNamespace ns = context.CreateOrGet(args[0].GetRaw().ToString());
            foreach (Type t in ts)
            {
                XLangBindingQuery q = XLangBindingQuery.Public;
                if (t.IsAbstract && t.IsSealed)
                {
                    q |= XLangBindingQuery.Static;
                }
                GetType(context, t, ns, q);
            }

            return null;
        }

        /// <summary>
        ///     Returns true if the Type is a Numeric Type
        /// </summary>
        /// <param name="o">Type to Check</param>
        /// <returns>True if the Type is a number.</returns>
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


        /// <summary>
        ///     Gets or Creates a Type Implementation based on a C# Type
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="type">C# Type</param>
        /// <param name="targetNs">Target Namespace</param>
        /// <param name="flags">Flags of the Resulting type(used if it is beeing created)</param>
        /// <returns>Type that has been found or created.</returns>
        private static XLangRuntimeType GetType(XLangContext context, Type type, XLangRuntimeNamespace targetNs,
            XLangBindingQuery flags = XLangBindingQuery.Public | XLangBindingQuery.Static)
        {
            if (type.IsArray)
            {
                return context.GetType("XL.Array");
            }
            if (type == typeof(object))
            {
                return context.GetType("XL.object");
            }
            if (type == typeof(string))
            {
                return context.GetType("XL.string");
            }
            if (type.IsNumericType())
            {
                return context.GetType("XL.number");
            }
            if (targetNs.HasType(type.Name, XLangBindingQuery.Public | XLangBindingQuery.Class))
            {
                return targetNs.GetType(type.Name, flags);
            }

            XLangRuntimeType t = null;
            t = new XLangRuntimeType(type.Name, targetNs, null, (XLangBindingFlags) flags);
            targetNs.AddType(t);
            List<IXLangRuntimeMember> members = GetStaticMembers(t, context, type, targetNs);
            members.AddRange(GetInstanceMembers(t, context, type, targetNs));
            t.SetMembers(members.ToArray());
            return t;
        }

        /// <summary>
        ///     Returns all Static Members of the C# Type
        /// </summary>
        /// <param name="implClass">The Implementing Class</param>
        /// <param name="context">Context of Execution</param>
        /// <param name="t">C# Type</param>
        /// <param name="targetNs">The target Namespace</param>
        /// <returns>Runtime Members created from the Specified type.</returns>
        private static List<IXLangRuntimeMember> GetStaticMembers(XLangRuntimeType implClass, XLangContext context,
            Type t, XLangRuntimeNamespace targetNs)
        {
            List<IXLangRuntimeMember> ret = new List<IXLangRuntimeMember>();
            PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Static);
            ret.AddRange(ps.Select(x =>
                ProcessProperty(implClass, context, x, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Static)));
            MethodInfo[] ms = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
            ret.AddRange(ms.Select(x =>
                ProcessMethod(implClass, context, x, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Static)));
            ret.RemoveAll(x => x == null);

            return ret;
        }

        /// <summary>
        ///     Returns all Instance Members of the C# Type
        /// </summary>
        /// <param name="implClass">The Implementing Class</param>
        /// <param name="context">Context of Execution</param>
        /// <param name="t">C# Type</param>
        /// <param name="targetNs">The target Namespace</param>
        /// <returns>Runtime Members created from the Specified type.</returns>
        private static List<IXLangRuntimeMember> GetInstanceMembers(XLangRuntimeType implClass, XLangContext context,
            Type t, XLangRuntimeNamespace targetNs)
        {
            List<IXLangRuntimeMember> ret = new List<IXLangRuntimeMember>();
            PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            ret.AddRange(ps.Select(x => ProcessProperty(implClass, context, x, targetNs,
                XLangBindingQuery.Public | XLangBindingQuery.Instance)));
            MethodInfo[] ms = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            ret.AddRange(ms.Select(x =>
                ProcessMethod(implClass, context, x, targetNs, XLangBindingQuery.Public | XLangBindingQuery.Instance)));
            ret.RemoveAll(x => x == null);
            return ret;
        }

        /// <summary>
        ///     Processes a Property of a C# Type
        /// </summary>
        /// <param name="implClass">The Implementing Class</param>
        /// <param name="context">The Context of Execution</param>
        /// <param name="pi">Property Info</param>
        /// <param name="targetNs">Target namespace</param>
        /// <param name="flags">Flags of the Type</param>
        /// <returns>Generated Property.</returns>
        private static IXLangRuntimeMember ProcessProperty(XLangRuntimeType implClass, XLangContext context,
            PropertyInfo pi, XLangRuntimeNamespace targetNs, XLangBindingQuery flags)
        {
            XLangRuntimeType type = GetType(context, pi.PropertyType, targetNs, flags & ~XLangBindingQuery.Static);
            if (type == null)
            {
                return null;
            }
            return new DelegateXLProperty(pi.Name, instance => GetInstance(context, pi, instance, targetNs), type,
                (XLangMemberFlags) flags, implClass);
        }


        /// <summary>
        ///     Processes a Function of a C# Type
        /// </summary>
        /// <param name="implClass">The Implementing Class</param>
        /// <param name="context">The Context of Execution</param>
        /// <param name="mi">Method Info</param>
        /// <param name="targetNs">Target namespace</param>
        /// <param name="flags">Flags of the Type</param>
        /// <returns>Generated Method.</returns
        private static IXLangRuntimeMember ProcessMethod(XLangRuntimeType implClass, XLangContext context,
            MethodInfo mi, XLangRuntimeNamespace targetNs, XLangBindingQuery flags)
        {
            XLangRuntimeType type = GetType(context, mi.ReturnType, targetNs, flags & ~XLangBindingQuery.Static);
            if (type == null)
            {
                return null;
            }
            return new DelegateXLFunction(mi.Name, (x, y) => InvokeFunc(context, mi, targetNs, x, y), type,
                (XLangMemberFlags) flags, implClass, ProcessArgs(mi, context, targetNs, flags));
        }

        /// <summary>
        ///     Returns the Processed Function Arguments.
        /// </summary>
        /// <param name="context">The Context of Execution</param>
        /// <param name="mi">Method Info</param>
        /// <param name="targetNs">Target namespace</param>
        /// <param name="flags">Flags of the Type</param>
        /// <returns>Processed Arguments.</returns>
        private static IXLangRuntimeFunctionArgument[] ProcessArgs(MethodInfo mi, XLangContext context,
            XLangRuntimeNamespace targetNs, XLangBindingQuery flags)
        {
            return mi.GetParameters().Select(x =>
                new XLangFunctionArgument(x.Name, GetType(context, x.ParameterType, targetNs, flags))).ToArray();
        }

        /// <summary>
        ///     Invokes a C# Function
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="mi">Method Info</param>
        /// <param name="targetNs">Target Namespace</param>
        /// <param name="arg1">Instance of the implementing class of the Function</param>
        /// <param name="arg2"></param>
        /// <returns>Return value</returns>
        private static IXLangRuntimeTypeInstance InvokeFunc(XLangContext context, MethodInfo mi,
            XLangRuntimeNamespace targetNs, IXLangRuntimeTypeInstance arg1, IXLangRuntimeTypeInstance[] arg2)
        {
            XLangRuntimeType t = GetType(context, mi.ReturnType, targetNs, XLangBindingQuery.Public);
            return new CSharpTypeInstance(t, mi.Invoke(arg1?.GetRaw(), arg2.Select(x => x.GetRaw()).ToArray()));
        }

        /// <summary>
        ///     Returns the Instance of a Specified Property
        /// </summary>
        /// <param name="context">Context of Execution</param>
        /// <param name="pi">Property Info</param>
        /// <param name="instance">Instance of the Implementing Class</param>
        /// <param name="targetNs">Target Namespace.</param>
        /// <returns></returns>
        private static IXLangRuntimeTypeInstance GetInstance(XLangContext context, PropertyInfo pi,
            IXLangRuntimeTypeInstance instance, XLangRuntimeNamespace targetNs)
        {
            XLangRuntimeType t = GetType(context, pi.PropertyType, targetNs, XLangBindingQuery.Public);
            return new CSharpTypeInstance(t, pi.GetValue(instance.GetRaw()));
        }
    }
}
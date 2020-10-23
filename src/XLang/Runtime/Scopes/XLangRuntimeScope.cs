using System;
using System.Collections.Generic;
using System.Linq;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;

namespace XLang.Runtime.Scopes
{
    public class XLangRuntimeScope
    {
        [Flags]
        public enum ScopeFlags
        {
            Return = 1,
            Continue = 2,
            Break = 4
        }

        public readonly XLangContext Context;

        private readonly List<XLangRuntimeScopedVar> localVars = new List<XLangRuntimeScopedVar>();
        private readonly IXLangRuntimeMember owner;
        private readonly List<string> visibleList = new List<string>();

        public XLangRuntimeScope(XLangContext context, IXLangRuntimeMember owner)
        {
            this.owner = owner;
            Context = context;
            visibleList.AddRange(context.DefaultImports);
            Continue = true;
        }

        public XLangRuntimeType OwnerType => owner.ImplementingClass;

        public bool Continue { get; }

        public ScopeFlags Flags { get; private set; }

        public IXLangRuntimeTypeInstance Return { get; private set; }

        public XLangRuntimeScopedVar Declare(string varName, XLangRuntimeType varType)
        {
            if (ResolveVar(varName) != null)
            {
                throw new XLangRuntimeTypeException($"Redefinition of var '{varName}'");
            }

            XLangRuntimeScopedVar v = new XLangRuntimeScopedVar(varName, varType);
            localVars.Add(v);
            return v;
        }

        public XLangRuntimeScopedVar ResolveVar(string name)
        {
            return localVars.FirstOrDefault(x => x.Name == name);
        }

        public XLangRuntimeNamespace[] GetAllVisible()
        {
            return Context.GetNamespaces().SelectMany(x => x.GetAllNamespacesRecursive())
                .Where(x => visibleList.Contains(x.FullName)).ToArray();
        }

        public void Error(ScopeFlags errorMask)
        {
            if (Check(errorMask))
            {
                throw new Exception($"Flag: '{errorMask}' is not valid for this scope");
            }
        }

        public void ClearFlag(ScopeFlags flag)
        {
            Flags &= ~flag;
        }

        public void SetFlag(ScopeFlags flag)
        {
            Flags |= flag;
        }

        public void SetReturn(IXLangRuntimeTypeInstance returnValue)
        {
            Return = returnValue;
            SetFlag(ScopeFlags.Return);
        }

        public bool Check(ScopeFlags checkMask)
        {
            return (Flags & checkMask) != 0;
        }

        public class XLangRuntimeScopedVar
        {
            public readonly string Name;
            private readonly XLangRuntimeType Types;
            private IXLangRuntimeTypeInstance Value;

            public XLangRuntimeScopedVar(string name, XLangRuntimeType types)
            {
                Name = name;
                Types = types;
            }

            public void SetValue(IXLangRuntimeTypeInstance value)
            {
                Value = value;
            }

            public IXLangRuntimeTypeInstance GetValue()
            {
                return Value ?? (Value = Types.CreateEmptyBase());
            }
        }
    }
}
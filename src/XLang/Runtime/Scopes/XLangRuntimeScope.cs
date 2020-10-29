using System;
using System.Collections.Generic;
using System.Linq;
using XLang.Exceptions;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;

namespace XLang.Runtime.Scopes
{
    /// <summary>
    ///     XL Runtime Scope that defines local scoped properties and instances.
    /// </summary>
    public class XLangRuntimeScope
    {
        /// <summary>
        ///     The Scope flags that are used to implement control flow changing statements.
        /// </summary>
        [Flags]
        public enum ScopeFlags
        {
            Return = 1,
            Continue = 2,
            Break = 4
        }

        /// <summary>
        ///     Context that this scope is in.
        /// </summary>
        public readonly XLangContext Context;

        /// <summary>
        ///     The Local Variables that are defined.
        /// </summary>
        private readonly List<XLangRuntimeScopedVar> localVars = new List<XLangRuntimeScopedVar>();

        /// <summary>
        ///     The Owner of this Scope
        /// </summary>
        private readonly IXLangRuntimeMember owner;

        /// <summary>
        ///     Visible Variables.
        /// </summary>
        private readonly List<string> visibleList = new List<string>();

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="context">Context of this Scope</param>
        /// <param name="owner">Owner Member</param>
        public XLangRuntimeScope(XLangContext context, IXLangRuntimeMember owner)
        {
            this.owner = owner;
            Context = context;
            visibleList.AddRange(context.DefaultImports);
            Continue = true;
        }

        /// <summary>
        ///     The Type of the Owner
        /// </summary>
        public XLangRuntimeType OwnerType => owner.ImplementingClass;

        /// <summary>
        ///     If true the continue key was set but not yet processed.
        /// </summary>
        public bool Continue { get; }

        /// <summary>
        ///     The Flags of this scope
        /// </summary>
        public ScopeFlags Flags { get; private set; }

        /// <summary>
        ///     The Return value of the scope.
        /// </summary>
        public IXLangRuntimeTypeInstance Return { get; private set; }

        /// <summary>
        ///     Declares a Variable
        /// </summary>
        /// <param name="varName">The Name of the Variable</param>
        /// <param name="varType">Type of the Variable</param>
        /// <returns>The Scoped Variable</returns>
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

        /// <summary>
        ///     Resolves a Variable by name
        /// </summary>
        /// <param name="name">Name of the Variable</param>
        /// <returns>Resolved Variable</returns>
        public XLangRuntimeScopedVar ResolveVar(string name)
        {
            return localVars.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        ///     Returns all Visible Namespaces inside this Scope
        /// </summary>
        /// <returns>Visible Namespaces</returns>
        public XLangRuntimeNamespace[] GetAllVisible()
        {
            return Context.GetNamespaces().SelectMany(x => x.GetAllNamespacesRecursive())
                .Where(x => visibleList.Contains(x.FullName)).ToArray();
        }

        /// <summary>
        ///     Throws an Exception if the flags are not correctly set.
        /// </summary>
        /// <param name="errorMask">The flags that need to be unset.</param>
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

        /// <summary>
        ///     Sets the Specified Flag
        /// </summary>
        /// <param name="flag">Flag to set.</param>
        public void SetFlag(ScopeFlags flag)
        {
            Flags |= flag;
        }

        /// <summary>
        ///     Sets the Return value of this scope.
        /// </summary>
        /// <param name="returnValue">The Return value</param>
        public void SetReturn(IXLangRuntimeTypeInstance returnValue)
        {
            Return = returnValue;
            SetFlag(ScopeFlags.Return);
        }

        /// <summary>
        ///     Checks the current flags if any of the flags are set that are also set in the checkMask
        /// </summary>
        /// <param name="checkMask">The Flags to Check</param>
        /// <returns>True if at least one flag is set</returns>
        public bool Check(ScopeFlags checkMask)
        {
            return (Flags & checkMask) != 0;
        }

        /// <summary>
        ///     Scoped Variable Implementation
        /// </summary>
        public class XLangRuntimeScopedVar
        {
            /// <summary>
            ///     The Name of the Variable
            /// </summary>
            public readonly string Name;

            /// <summary>
            ///     The Type of this Scoped Variable
            /// </summary>
            private readonly XLangRuntimeType Type;

            /// <summary>
            ///     The Value of this Scoped Variable
            /// </summary>
            private IXLangRuntimeTypeInstance Value;

            /// <summary>
            ///     Public Constructor.
            /// </summary>
            /// <param name="name">Name of the Variable</param>
            /// <param name="type">The type of the Variable</param>
            public XLangRuntimeScopedVar(string name, XLangRuntimeType type)
            {
                Name = name;
                Type = type;
            }

            /// <summary>
            ///     Sets the Value of this Scoped Var
            /// </summary>
            /// <param name="value">The new Value</param>
            public void SetValue(IXLangRuntimeTypeInstance value)
            {
                Value = value;
            }

            /// <summary>
            ///     Returns the current value or an empty type instance.
            /// </summary>
            /// <returns>Instance of this var.</returns>
            public IXLangRuntimeTypeInstance GetValue()
            {
                return Value ?? (Value = Type.CreateEmptyBase());
            }
        }
    }
}
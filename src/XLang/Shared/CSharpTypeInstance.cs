using System;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

/// <summary>
/// Contains Shared Logic and XLRuntimeItem implementations for Interfacing with the XL VM
/// </summary>
namespace XLang.Shared
{
    /// <summary>
    ///     Simple C# Runtime Type Instance Implementation
    /// </summary>
    public class CSharpTypeInstance : IXLangRuntimeTypeInstance
    {
        /// <summary>
        ///     value of this type instance
        /// </summary>
        private object value;

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="type">Type of this instance</param>
        /// <param name="value">Instance Value</param>
        public CSharpTypeInstance(XLangRuntimeType type, object value)
        {
            Type = type;
            this.value = value;
        }

        /// <summary>
        ///     Type of this Instance
        /// </summary>
        public XLangRuntimeType Type { get; }

        /// <summary>
        ///     Adds Local defined types of this Instance
        /// </summary>
        /// <param name="scope">The Scope to add to.</param>
        public void AddLocals(XLangRuntimeScope scope)
        {
        }

        /// <summary>
        ///     returns the Raw Value of this Type Instance
        /// </summary>
        /// <returns>The Instance Value</returns>
        public object GetRaw()
        {
            return value;
        }

        /// <summary>
        ///     Sets the Raw Value of this Type Instance
        ///     Does throw Exception if type does not inherit from Instance Type
        /// </summary>
        /// <param name="type">New Type this Value</param>
        /// <param name="value">Value instance</param>
        public void SetRaw(XLangRuntimeType type, object value)
        {
            if (type.InheritsFrom(Type))
            {
                this.value = value;
            }
            else
            {
                throw new Exception("Type Mismatch");
            }
        }
    }
}
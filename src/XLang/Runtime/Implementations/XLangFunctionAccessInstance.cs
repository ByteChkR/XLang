using XLang.Exceptions;
using XLang.Runtime.Members;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Runtime.Implementations
{
    /// <summary>
    ///     Function Type Implementation
    /// </summary>
    public class XLangFunctionAccessInstance : IXLangRuntimeTypeInstance
    {
        /// <summary>
        ///     The Instance of the implemented class of this function
        /// </summary>
        public readonly IXLangRuntimeTypeInstance Instance;

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="member">Members (functions) with the same name</param>
        /// <param name="instance">Instance of the Implementing Class, Null for static</param>
        /// <param name="functionType">The Function Type</param>
        public XLangFunctionAccessInstance(
            IXLangRuntimeItem[] member, IXLangRuntimeTypeInstance instance, XLangRuntimeType functionType)
        {
            Instance = instance;
            Member = member;
            Type = functionType;
        }

        /// <summary>
        ///     The Members that will get invoked
        ///     Those members have the same name but different arguments.
        /// </summary>
        public IXLangRuntimeItem[] Member { get; }

        /// <summary>
        ///     The Type of this Function Access Instance
        /// </summary>
        public XLangRuntimeType Type { get; }

        /// <summary>
        ///     Adds all Locally Defined variables inside this type
        /// </summary>
        /// <param name="scope"></param>
        public void AddLocals(XLangRuntimeScope scope)
        {
            //Functions do not have local variables
        }

        /// <summary>
        ///     Gets the Raw Value
        /// </summary>
        /// <returns></returns>
        public object GetRaw()
        {
            return Member;
        }

        /// <summary>
        ///     Sets the Raw Value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetRaw(XLangRuntimeType type, object value)
        {
            if (value == null)
            {
                return;
            }

            if (type.InheritsFrom(Type))
            {
                Member[0] = (IXLangRuntimeMember) value;
            }
            else
            {
                throw new XLangRuntimeTypeException("Type Mismatch");
            }

        }
    }
}
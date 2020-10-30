using XLang.Core;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.BaseTypes
{
    /// <summary>
    ///     Implements "XL.object"
    /// </summary>
    public class XLangObjectType
    {
        private readonly XLCoreNamespace containingNamespace;

        public XLangObjectType(XLCoreNamespace core)
        {
            containingNamespace = core;
        }

        /// <summary>
        ///     Creates the Runtime Type
        /// </summary>
        /// <returns>"XL.object" type</returns>
        public XLangRuntimeType GetObject()
        {
            XLangRuntimeType objectType = new XLangRuntimeType(
                "object",
                containingNamespace,
                null,
                XLangBindingFlags.Instance | XLangBindingFlags.Public
            );
            DelegateXLFunction eqFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpEquality.ToString(),
                    (instance, args) => EquValue(args),
                    objectType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    objectType,
                    new XLangFunctionArgument("a", objectType),
                    new XLangFunctionArgument("b", objectType)
                );
            DelegateXLFunction cmpFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpComparison.ToString(),
                    CmpValue,
                    objectType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    objectType,
                    new XLangFunctionArgument("a", objectType),
                    new XLangFunctionArgument("b", objectType)
                );


            objectType.SetMembers(new[] {eqFunc, cmpFunc});

            return objectType;
        }

        #region Operator Implementations

        private IXLangRuntimeTypeInstance EquValue(IXLangRuntimeTypeInstance[] args)
        {
            args[0].SetRaw(args[1].Type, args[1].GetRaw());
            return args[0];
        }

        private IXLangRuntimeTypeInstance CmpValue(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );

            return new CSharpTypeInstance(
                type,
                (decimal) args[0].GetRaw() == (decimal) args[1].GetRaw()
                    ? (decimal) 1
                    : (decimal) 0
            );
        }

        #endregion
    }
}
﻿using System.ComponentModel;

using XLang.Core;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Types;
using XLang.Shared;
using XLang.Shared.Enum;

namespace XLang.BaseTypes
{
    /// <summary>
    ///     Implements "XL.number"
    /// </summary>
    public class XLangNumberType
    {
        /// <summary>
        ///     The Containing Namespace
        /// </summary>
        private readonly XLCoreNamespace containingNamespace;

        /// <summary>
        ///     Public constructor
        /// </summary>
        /// <param name="core">Core Namespace</param>
        public XLangNumberType(XLCoreNamespace core)
        {
            containingNamespace = core;
        }

        /// <summary>
        ///     Creates the Runtime Type
        /// </summary>
        /// <param name="objectType">Type "XL.object"</param>
        /// <returns>"XL.number" type</returns>
        public XLangRuntimeType GetObject(XLangRuntimeType objectType)
        {
            XLangRuntimeType numberType = new XLangRuntimeType(
                "number",
                containingNamespace,
                objectType,
                XLangBindingFlags.Public | XLangBindingFlags.Instance,
                x => new CSharpTypeInstance(x, (decimal)0)
            );
            DelegateXLFunction mulNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpAsterisk.ToString(),
                    MulNum,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );

            DelegateXLFunction gtNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpGreaterThan.ToString(),
                    GreaterThan,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction ltNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpLessThan.ToString(),
                    LessThan,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction geNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpGreaterOrEqual.ToString(),
                    GreaterOrEqual,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction leNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpLessOrEqual.ToString(),
                    LessOrEqual,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction addNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpPlus.ToString(),
                    PlusNum,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction modNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpPercent.ToString(),
                    (instance, args) => ModNum(args),
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction subNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpMinus.ToString(),
                    SubNum,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction divNumFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpFwdSlash.ToString(),
                    DivNum,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );

            DelegateXLFunction notFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpBang.ToString(),
                    NotValue,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType)
                );
            DelegateXLFunction unPlusFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpPlus.ToString(),
                                       UnPlusNum,
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType)
                                      );
            DelegateXLFunction unIncFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpUnaryIncrement.ToString(), (instance, args) => UnaryInc(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType)
                                      );

            DelegateXLFunction unDecFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpUnaryDecrement.ToString(), (instance, args) => UnaryDec(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType)
                                      );

            DelegateXLFunction unMinusFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpMinus.ToString(),
                    UnMinusNum,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType)
                );
            DelegateXLFunction andFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpAnd.ToString(),
                                       AndValue,
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );

            DelegateXLFunction sumAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpSumAssign.ToString(), (instance, args) => SumAssignNum(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );

            DelegateXLFunction difAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpDifAssign.ToString(), (instance, args) => DifAssignNum(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );

            DelegateXLFunction prodAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpProdAssign.ToString(), (instance, args) => ProdAssignNum(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );
            DelegateXLFunction quotAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpQuotAssign.ToString(), (instance, args) => QuotAssignNum(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );

            DelegateXLFunction remAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpRemAssign.ToString(), (instance, args) => RemAssignNum(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );
            DelegateXLFunction orAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpOrAssign.ToString(), (instance, args) => OrAssignValue(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );
            DelegateXLFunction andAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpAndAssign.ToString(), (instance, args) => AndAssignValue(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );
            DelegateXLFunction xorAssignFunc =
                new DelegateXLFunction(
                                       XLangTokenType.OpXOrAssign.ToString(), (instance, args) => XOrAssignValue(args),
                                       numberType,
                                       XLangMemberFlags.Static |
                                       XLangMemberFlags.Private |
                                       XLangMemberFlags.Operator |
                                       XLangMemberFlags.Override,
                                       numberType,
                                       new XLangFunctionArgument("a", numberType),
                                       new XLangFunctionArgument("b", numberType)
                                      );

            DelegateXLFunction orFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpPipe.ToString(),
                    OrValue,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );

            DelegateXLFunction xorFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpCap.ToString(),
                    XOrValue,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );

            DelegateXLFunction logicOrFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpLogicalOr.ToString(),
                    LogicalOrValue,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction logicAndFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpLogicalAnd.ToString(),
                    LogicalAndValue,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType),
                    new XLangFunctionArgument("b", numberType)
                );
            DelegateXLFunction flipFunc =
                new DelegateXLFunction(
                    XLangTokenType.OpTilde.ToString(),
                    FlipBitsValue,
                    numberType,
                    XLangMemberFlags.Static |
                    XLangMemberFlags.Private |
                    XLangMemberFlags.Operator |
                    XLangMemberFlags.Override,
                    numberType,
                    new XLangFunctionArgument("a", numberType)
                );


            numberType.SetMembers(
                new IXLangRuntimeMember[]
                {
                    unMinusFunc,
                    unPlusFunc,
                    mulNumFunc,
                    addNumFunc,
                    subNumFunc,
                    divNumFunc,
                    andFunc,
                    orFunc,
                    xorFunc,
                    flipFunc,
                    notFunc,
                    leNumFunc,
                    ltNumFunc,
                    geNumFunc,
                    gtNumFunc,
                    modNumFunc,
                    logicAndFunc,
                    logicOrFunc,
                    unIncFunc,
                    unDecFunc,
                    sumAssignFunc,
                    difAssignFunc,
                    prodAssignFunc,
                    quotAssignFunc,
                    remAssignFunc,
                    andAssignFunc,
                    orAssignFunc,
                    xorAssignFunc
                }
            );
            return numberType;
        }

        #region Number Operators

        private IXLangRuntimeTypeInstance MulNum(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );

            return new CSharpTypeInstance(type, (decimal)args[0].GetRaw() * (decimal)args[1].GetRaw());
        }

        private IXLangRuntimeTypeInstance PlusNum(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );

            return new CSharpTypeInstance(type, (decimal)args[0].GetRaw() + (decimal)args[1].GetRaw());
        }

        private IXLangRuntimeTypeInstance UnaryInc(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );

            args[0].SetRaw(type, (decimal)args[0].GetRaw() + 1);
            return args[0];
        }

        private IXLangRuntimeTypeInstance UnaryDec(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );

            args[0].SetRaw(type, (decimal)args[0].GetRaw() - 1);
            return args[0];
        }



        private IXLangRuntimeTypeInstance ModNum(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );

            return new CSharpTypeInstance(type, (decimal)args[0].GetRaw() % (decimal)args[1].GetRaw());
        }

        private IXLangRuntimeTypeInstance SumAssignNum(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );
            args[0].SetRaw(type, (decimal)args[0].GetRaw() + (decimal)args[1].GetRaw());
            return args[0];
        }

        private IXLangRuntimeTypeInstance DifAssignNum(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );
            args[0].SetRaw(type, (decimal)args[0].GetRaw() - (decimal)args[1].GetRaw());
            return args[0];
        }

        private IXLangRuntimeTypeInstance ProdAssignNum(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );
            args[0].SetRaw(type, (decimal)args[0].GetRaw() * (decimal)args[1].GetRaw());
            return args[0];
        }


        private IXLangRuntimeTypeInstance QuotAssignNum(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );
            args[0].SetRaw(type, (decimal)args[0].GetRaw() / (decimal)args[1].GetRaw());
            return args[0];
        }

        private IXLangRuntimeTypeInstance RemAssignNum(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );
            args[0].SetRaw(type, (decimal)args[0].GetRaw() % (decimal)args[1].GetRaw());
            return args[0];
        }

        private IXLangRuntimeTypeInstance OrAssignValue(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );

            args[0].SetRaw(
                           type,
                           (decimal)((int)(decimal)args[0].GetRaw() |
                                     (int)(decimal)args[1].GetRaw())
                          );
            return args[0];
        }

        private IXLangRuntimeTypeInstance AndAssignValue(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );

            args[0].SetRaw(
                           type,
                           (decimal)((int)(decimal)args[0].GetRaw() &
                                     (int)(decimal)args[1].GetRaw())
                          );
            return args[0];
        }

        private IXLangRuntimeTypeInstance XOrAssignValue(IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                                                                "number",
                                                                XLangBindingQuery.Public |
                                                                XLangBindingQuery.Instance |
                                                                XLangBindingQuery.Inclusive
                                                               );

            args[0].SetRaw(
                           type,
                           (decimal)((int)(decimal)args[0].GetRaw() ^
                                     (int)(decimal)args[1].GetRaw())
                          );
            return args[0];
        }


        private IXLangRuntimeTypeInstance UnPlusNum(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );

            return new CSharpTypeInstance(type, (decimal)args[0].GetRaw());
        }

        private IXLangRuntimeTypeInstance UnMinusNum(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );

            return new CSharpTypeInstance(type, -(decimal)args[0].GetRaw());
        }


        private IXLangRuntimeTypeInstance DivNum(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );

            return new CSharpTypeInstance(type, (decimal)args[0].GetRaw() / (decimal)args[1].GetRaw());
        }


        private IXLangRuntimeTypeInstance SubNum(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );

            return new CSharpTypeInstance(type, (decimal)args[0].GetRaw() - (decimal)args[1].GetRaw());
        }

        private IXLangRuntimeTypeInstance NotValue(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );
            decimal v = (decimal)args[0].GetRaw();
            return new CSharpTypeInstance(type, v == 0 ? (decimal)1 : (decimal)0);
        }

        private IXLangRuntimeTypeInstance LessThan(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );
            decimal a = (decimal)args[0].GetRaw();
            decimal b = (decimal)args[1].GetRaw();
            return new CSharpTypeInstance(type, a < b ? (decimal)1 : (decimal)0);
        }

        private IXLangRuntimeTypeInstance GreaterThan(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );
            decimal a = (decimal)args[0].GetRaw();
            decimal b = (decimal)args[1].GetRaw();
            return new CSharpTypeInstance(type, a > b ? (decimal)1 : (decimal)0);
        }


        private IXLangRuntimeTypeInstance LessOrEqual(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );
            decimal a = (decimal)args[0].GetRaw();
            decimal b = (decimal)args[1].GetRaw();
            return new CSharpTypeInstance(type, a <= b ? (decimal)1 : (decimal)0);
        }

        private IXLangRuntimeTypeInstance GreaterOrEqual(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );
            decimal a = (decimal)args[0].GetRaw();
            decimal b = (decimal)args[1].GetRaw();
            return new CSharpTypeInstance(type, a >= b ? (decimal)1 : (decimal)0);
        }


        private IXLangRuntimeTypeInstance AndValue(
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
                (decimal)((int)(decimal)args[0].GetRaw() &
                           (int)(decimal)args[1].GetRaw())
            );
        }

        private IXLangRuntimeTypeInstance OrValue(
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
                (decimal)((int)(decimal)args[0].GetRaw() |
                           (int)(decimal)args[1].GetRaw())
            );
        }

        private IXLangRuntimeTypeInstance LogicalOrValue(
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
                (decimal)((int)(decimal)args[0].GetRaw() != 0 || (int)(decimal)args[1].GetRaw() != 0 ? 0 : 1)
            );
        }

        private IXLangRuntimeTypeInstance LogicalAndValue(
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
                (decimal)((int)(decimal)args[0].GetRaw() != 0 && (int)(decimal)args[1].GetRaw() != 0 ? 0 : 1)
            );
        }

        private IXLangRuntimeTypeInstance XOrValue(
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
                (decimal)((int)(decimal)args[0].GetRaw() ^
                           (int)(decimal)args[1].GetRaw())
            );
        }

        private IXLangRuntimeTypeInstance FlipBitsValue(
            IXLangRuntimeTypeInstance instance, IXLangRuntimeTypeInstance[] args)
        {
            XLangRuntimeType type = containingNamespace.GetType(
                "number",
                XLangBindingQuery.Public |
                XLangBindingQuery.Instance |
                XLangBindingQuery.Inclusive
            );

            return new CSharpTypeInstance(type, (decimal)~(int)(decimal)args[0].GetRaw());
        }

        #endregion
    }
}
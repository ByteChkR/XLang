using System.Collections.Generic;
using XLang.Core;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

/// <summary>
/// Contains XLangExpressionParser Token Implementations
/// </summary>
namespace XLang.Parser.Token.Expressions
{
    /// <summary>
    ///     Implements the base of any XLangExpression implementation
    /// </summary>
    public abstract class XLangExpression : IXLangToken
    {
        /// <summary>
        ///     The XL Context
        /// </summary>
        protected readonly XLangContext Context;

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        protected XLangExpression(XLangContext context, int sourceIndex)
        {
            Context = context;
        }

        /// <summary>
        ///     Start index in source
        /// </summary>
        public int SourceIndex { get; }

        /// <summary>
        ///     The Token Type (OpExpression)
        /// </summary>
        public virtual XLangTokenType Type => XLangTokenType.OpExpression;

        /// <summary>
        ///     Returns the Child Tokens of this token
        /// </summary>
        /// <returns></returns>
        public abstract List<IXLangToken> GetChildren();

        /// <summary>
        ///     Returns the String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public abstract string GetValue();

        /// <summary>
        ///     Processes this Expression
        /// </summary>
        /// <param name="scope">Execution Scope</param>
        /// <param name="instance">The Expression Type Instance</param>
        /// <returns></returns>
        public abstract IXLangRuntimeTypeInstance Process(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance);
    }
}
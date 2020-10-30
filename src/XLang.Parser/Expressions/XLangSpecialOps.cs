using System;
using System.Collections.Generic;
using System.Linq;
using XLang.Core;
using XLang.Parser.Reader;
using XLang.Parser.Token;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators.Special;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Expressions
{
    /// <summary>
    ///     Implements Special Expressions that do require some custom parsing steps
    /// </summary>
    public static class XLangSpecialOps
    {
        #region For Parser

        /// <summary>
        ///     Parses a For Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public static XLangExpression ReadFor(XLangExpressionParser parser)
        {
            IXLangToken ft = parser.CurrentToken;
            parser.Eat(XLangTokenType.OpFor);
            parser.Eat(XLangTokenType.OpBracketOpen);
            XLangExpression vDecl = parser.ParseExpr(0);
            parser.Eat(XLangTokenType.OpSemicolon);
            XLangExpression condition = parser.ParseExpr(0);
            parser.Eat(XLangTokenType.OpSemicolon);
            XLangExpression vInc = parser.ParseExpr(0);
            parser.Eat(XLangTokenType.OpBracketClose);

            XLangExpression token = null;
            List<XLangExpression> block;
            if (parser.CurrentToken.Type != XLangTokenType.OpBlockToken)
            {
                block = new List<XLangExpression> {parser.ParseExpr(0)};
            }
            else
            {
                block = XLangExpressionParser
                    .Create(parser.Context, new XLangExpressionReader(parser.CurrentToken.GetChildren())).Parse()
                    .ToList();
            }
            token = new XLangForOp(parser.Context, vDecl, condition, vInc,
                (x, y) => RunMulti(block, x, y), ft.SourceIndex);


            return token;
        }

        #endregion


        #region While Parser

        /// <summary>
        ///     Parses a While Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public static XLangExpression ReadWhile(XLangExpressionParser parser)
        {
            IXLangToken wT = parser.CurrentToken;
            parser.Eat(XLangTokenType.OpWhile);
            parser.Eat(XLangTokenType.OpBracketOpen);
            XLangExpression condition = parser.ParseExpr(0);
            parser.Eat(XLangTokenType.OpBracketClose);

            XLangExpression token = null;
            List<XLangExpression> block;
            if (parser.CurrentToken.Type != XLangTokenType.OpBlockToken)
            {
                block = new List<XLangExpression> {parser.ParseExpr(0)};
            }
            else
            {
                block = XLangExpressionParser
                    .Create(parser.Context, new XLangExpressionReader(parser.CurrentToken.GetChildren())).Parse()
                    .ToList();
            }
            token = new XLangWhileOp(parser.Context, condition, RunMulti, wT.SourceIndex);

            void RunMulti(XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
            {
                foreach (XLangExpression xLangExpression in block)
                {
                    xLangExpression.Process(scope, instance);
                    if (scope.Check(XLangRuntimeScope.ScopeFlags.Continue))
                    {
                        scope.ClearFlag(XLangRuntimeScope.ScopeFlags.Continue);
                        return;
                    }
                    if (scope.Check(XLangRuntimeScope.ScopeFlags.Break | XLangRuntimeScope.ScopeFlags.Return))
                    {
                        return;
                    }
                }
            }

            return token;
        }

        #endregion


        #region Utility

        private static void RunMulti(
            IEnumerable<XLangExpression> block, XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance,
            bool clearContinue = true)
        {
            foreach (XLangExpression xLangExpression in block)
            {
                xLangExpression.Process(scope, instance);
                if (scope.Check(XLangRuntimeScope.ScopeFlags.Continue))
                {
                    if (clearContinue)
                    {
                        scope.ClearFlag(XLangRuntimeScope.ScopeFlags.Continue);
                    }
                    return;
                }
                if (scope.Check(XLangRuntimeScope.ScopeFlags.Break | XLangRuntimeScope.ScopeFlags.Return))
                {
                    return;
                }
            }
        }

        #endregion


        #region If Parser

        /// <summary>
        ///     Parses an If Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Condition Expression and Expression Block</returns>
        private static (XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>) ReadIfStatement(
            XLangExpressionParser parser)
        {
            parser.Eat(XLangTokenType.OpIf);
            parser.Eat(XLangTokenType.OpBracketOpen);
            XLangExpression condition = parser.ParseExpr(0);
            parser.Eat(XLangTokenType.OpBracketClose);

            List<XLangExpression> content = ReadIfBlockContent(parser);
            return (condition, (scope, instance) => RunMulti(content, scope, instance, false));
        }

        /// <summary>
        ///     Parses an If Expression block from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression Block</returns>
        private static List<XLangExpression> ReadIfBlockContent(XLangExpressionParser parser)
        {
            if (parser.CurrentToken.Type != XLangTokenType.OpBlockToken)
            {
                XLangExpression expr = parser.ParseExpr(0);
                parser.Eat(XLangTokenType.OpSemicolon);
                return new List<XLangExpression> {expr};
            }

            IXLangToken token = parser.CurrentToken;
            parser.Eat(XLangTokenType.OpBlockToken);

            return XLangExpressionParser.Create(parser.Context, new XLangExpressionReader(token.GetChildren()))
                .Parse().ToList();
        }


        /// <summary>
        ///     Parses an If Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed If Expression</returns>
        public static XLangExpression ReadIf(XLangExpressionParser parser)
        {
            List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)> conditions =
                new List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)>();
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> elseBranch = null;
            IXLangToken it = parser.CurrentToken;
            conditions.Add(ReadIfStatement(parser));

            while (parser.CurrentToken.Type == XLangTokenType.OpElse)
            {
                parser.Eat(XLangTokenType.OpElse);
                if (parser.CurrentToken.Type == XLangTokenType.OpIf)
                {
                    conditions.Add(ReadIfStatement(parser));
                }
                else
                {
                    List<XLangExpression> content = ReadIfBlockContent(parser);
                    elseBranch = (scope, instance) => RunMulti(content, scope, instance, false);
                    break;
                }
            }


            return new XLangIfOp(parser.Context, conditions, elseBranch, it.SourceIndex);
        }

        #endregion
    }
}
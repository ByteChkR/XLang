using System;
using System.Collections.Generic;
using System.Linq;

using XLang.Core;
using XLang.Parser.Reader;
using XLang.Parser.Token;
using XLang.Parser.Token.Combined;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operands;
using XLang.Parser.Token.Expressions.Operators;
using XLang.Parser.Token.Expressions.Operators.Special;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Expressions
{
    public class XLangExpressionValueCreator : AXLangExpressionValueCreator
    {

        #region Specials

        private XLangExpression ReadIf(XLangExpressionParser parser)
        {
            List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)> conditions =
                new List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)>();
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> elseBranch = null;
            ReadBlock();

            while (parser.CurrentToken.Type == XLangTokenType.OpIf)
            {
                ReadBlock();
            }

            if (parser.CurrentToken.Type != XLangTokenType.OpSemicolon)
            {
                List<XLangExpression> content = ReadBlockContent();
                elseBranch = (scope, instance) => RunMulti(content, scope, instance, false);
            }

            void ReadBlock()
            {
                parser.Eat(XLangTokenType.OpIf);
                parser.Eat(XLangTokenType.OpBracketOpen);
                XLangExpression condition = parser.ParseExpr(0);
                parser.Eat(XLangTokenType.OpBracketClose);

                List<XLangExpression> content = ReadBlockContent();
                conditions.Add((condition, (scope, instance) => RunMulti(content, scope, instance, false)));
                if (parser.CurrentToken.Type == XLangTokenType.OpElse)
                {
                    parser.Eat(XLangTokenType.OpElse);
                }
            }

            List<XLangExpression> ReadBlockContent()
            {
                if (parser.CurrentToken.Type != XLangTokenType.OpBlockToken)
                {
                    XLangExpression expr = parser.ParseExpr(0);
                    return new List<XLangExpression> { expr };
                }

                IXLangToken token = parser.CurrentToken;
                parser.Eat(XLangTokenType.OpBlockToken);

                return XLangExpressionParser.Create(parser.Context, new XLangExpressionReader(token.GetChildren()))
                                            .Parse().ToList();
                //return token.GetChildren().Select(
                //                                  x => new XLangExpressionParser(
                //                                                                 context,
                //                                                                 new XLangExpressionReader(
                //                                                                                           x.GetChildren()
                //                                                                                          )
                //                                                                ).Parse()
                //                                 ).ToList();
            }



            return new XLangIfOp(parser.Context, XLangTokenType.OpIf, conditions, elseBranch);
        }


        private XLangExpression ReadFor(XLangExpressionParser parser)
        {
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
                block = new List<XLangExpression> { parser.ParseExpr(0) };
            }
            else
            {
                block = XLangExpressionParser.Create(parser.Context, new XLangExpressionReader(parser.CurrentToken.GetChildren())).Parse().ToList();
            }
            token = new XLangForOp(parser.Context, vDecl, condition, vInc, XLangTokenType.OpFor, (x, y) => RunMulti(block, x, y));


            return token;
        }

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
                        scope.ClearFlag(XLangRuntimeScope.ScopeFlags.Continue);
                    return;
                }
                if (scope.Check(XLangRuntimeScope.ScopeFlags.Break | XLangRuntimeScope.ScopeFlags.Return))
                {
                    return;
                }
            }
        }

        private XLangExpression ReadWhile(XLangExpressionParser parser)
        {
            parser.Eat(XLangTokenType.OpWhile);
            parser.Eat(XLangTokenType.OpBracketOpen);
            XLangExpression condition = parser.ParseExpr(0);
            parser.Eat(XLangTokenType.OpBracketClose);

            XLangExpression token = null;
            List<XLangExpression> block;
            if (parser.CurrentToken.Type != XLangTokenType.OpBlockToken)
            {
                block = new List<XLangExpression> { parser.ParseExpr(0) };
            }
            else
            {
                block = XLangExpressionParser.Create(parser.Context, new XLangExpressionReader(parser.CurrentToken.GetChildren())).Parse().ToList();

            }
            token = new XLangWhileOp(parser.Context, condition, XLangTokenType.OpWhile, RunMulti);
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


        public override XLangExpression CreateValue(XLangExpressionParser parser)
        {
            if (parser.CurrentToken.Type == XLangTokenType.OpNew)
            {
                parser.Eat(parser.CurrentToken.Type);
                XLangExpression token = new XLangUnaryOp(parser.Context, parser.ParseExpr(0), XLangTokenType.OpNew);
                return token;
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpReturn)
            {
                parser.Eat(XLangTokenType.OpReturn);
                if (parser.CurrentToken.Type == XLangTokenType.OpSemicolon)
                {
                    return new XLangReturnOp(parser.Context, null);
                }
                return new XLangReturnOp(parser.Context, parser.ParseExpr(0));
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpContinue)
            {
                parser.Eat(XLangTokenType.OpContinue);
                return new XLangContinueOp(parser.Context);
            }
            if (parser.CurrentToken.Type == XLangTokenType.OpBreak)
            {
                parser.Eat(XLangTokenType.OpBreak);
                return new XLangBreakOp(parser.Context);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpIf)
            {
                return ReadIf(parser);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpFor)
            {
                return ReadFor(parser);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpWhile)
            {
                return ReadWhile(parser);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpBracketOpen)
            {
                parser.Eat(XLangTokenType.OpBracketOpen);
                XLangExpression token = parser.ParseExpr(0);
                parser.Eat(XLangTokenType.OpBracketClose);
                return token;
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpThis ||
                parser.CurrentToken.Type == XLangTokenType.OpBase)
            {
                XLangExpression token = new XLangVarOperand(parser.Context, parser.CurrentToken);
                parser.Eat(parser.CurrentToken.Type);

                return token;
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpWord)
            {
                IXLangToken item = parser.CurrentToken;
                XLangExpression token = null;
                parser.Eat(parser.CurrentToken.Type);
                if (parser.CurrentToken.Type == XLangTokenType.OpWord)
                {
                    token = new XLangVarDefOperand(
                                                   parser.Context,
                                                   new VariableDefinitionToken(
                                                                               parser.CurrentToken,
                                                                               item,
                                                                               new IXLangToken[0],
                                                                               new IXLangToken[0],
                                                                               null
                                                                              )
                                                  );
                    parser.Eat(parser.CurrentToken.Type);
                }
                else
                {
                    token = new XLangVarOperand(parser.Context, item);
                }

                return token;
            }


            if (parser.CurrentToken.Type == XLangTokenType.OpVariableDefinition)
            {
                XLangExpression token = new XLangVarDefOperand(parser.Context, (VariableDefinitionToken)parser.CurrentToken);
                parser.Eat(XLangTokenType.OpVariableDefinition);
                return token;
            }
            if (parser.CurrentToken.Type == XLangTokenType.OpNumber ||
                parser.CurrentToken.Type == XLangTokenType.OpStringLiteral)
            {
                XLangExpression token = new XLangValueOperand(parser.Context, parser.CurrentToken);
                parser.Eat(parser.CurrentToken.Type);
                return token;
            }


            throw new Exception("Invalid Token: " + parser.CurrentToken.Type);
        }

    }
}
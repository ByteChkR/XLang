using System;
using System.Collections.Generic;
using System.Linq;

using XLang.Core;
using XLang.Parser.Reader;
using XLang.Parser.Shared;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;
using XLang.Parser.Token.Combined;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operands;
using XLang.Parser.Token.Expressions.Operators;
using XLang.Parser.Token.Expressions.Operators.Special;
using XLang.Runtime.Scopes;
using XLang.Runtime.Types;

namespace XLang.Parser.Expressions
{
    public class XLangExpressionParser
    {

        private readonly XLangContext context;
        private readonly XLangExpressionReader reader;

        public XLangExpressionParser(XLangContext context, XLangExpressionReader reader)
        {
            this.context = context;
            this.reader = reader;
            CurrentToken = reader.GetNext();
        }

        public IXLangToken CurrentToken { get; private set; }

        public XLangExpression[] Parse()
        {
            if (CurrentToken.Type == XLangTokenType.EOF) return new XLangExpression[0];
            List<XLangExpression> ret = new List<XLangExpression> { Expr() };
            while (CurrentToken.Type == XLangTokenType.OpSemicolon)
            {
                Eat(XLangTokenType.OpSemicolon);
                if (CurrentToken.Type == XLangTokenType.EOF) break;
                ret.Add(Expr());
            }
            return ret.ToArray();
        }

        private void Eat(XLangTokenType type)
        {
            if (CurrentToken.Type == type)
            {
                CurrentToken = reader.GetNext();
            }
            else
            {
                throw new XLangTokenReadException(reader.tokens, type, CurrentToken.Type, CurrentToken.StartIndex);
            }
        }

        private XLangExpression DotConcat()
        {
            XLangExpression node = Factor();

            string x = "";
            int startId = -1;
            while (CurrentToken.Type == XLangTokenType.OpDot)
            {
                startId = CurrentToken.StartIndex;
                if (x != "")
                {
                    x += ".";
                }

                if (node is XLangVarOperand vop)
                {
                    x += vop.Value.GetValue();
                }
                else if (node is XLangVarDefOperand vdop)
                {
                    x += vdop.Value.GetValue();
                }
                else
                {
                    continue;
                }

                Eat(XLangTokenType.OpDot);
                node = Expr();
            }

            if (x != "")
            {
                x += ".";
                if (node is XLangVarOperand vop)
                {
                    x += vop.Value.GetValue();
                }
                else if (node is XLangVarDefOperand vdop)
                {
                    x += vdop.Value.GetValue();
                }

                node = new XLangVarOperand(context, new TextToken(XLangTokenType.OpWord, x, startId));
            }



            return node;
        }

        private XLangExpression Term()
        {
            XLangExpression node = DotConcat();


            while (CurrentToken.Type == XLangTokenType.OpAsterisk ||
                   CurrentToken.Type == XLangTokenType.OpFwdSlash ||
                   CurrentToken.Type == XLangTokenType.OpBracketOpen ||
                   CurrentToken.Type == XLangTokenType.OpIndexerBracketOpen ||
                   CurrentToken.Type == XLangTokenType.OpAnd ||
                   CurrentToken.Type == XLangTokenType.OpPipe ||
                   CurrentToken.Type == XLangTokenType.OpEquality && reader.PeekNext().Type == XLangTokenType.OpEquality ||
                   CurrentToken.Type == XLangTokenType.OpLessThan ||
                   CurrentToken.Type == XLangTokenType.OpGreaterThan)

            {
                IXLangToken token = CurrentToken;
                if (token.Type == XLangTokenType.OpAsterisk ||
                    token.Type == XLangTokenType.OpFwdSlash ||
                    token.Type == XLangTokenType.OpAnd ||
                    token.Type == XLangTokenType.OpPipe
                )
                {
                    Eat(token.Type);
                    node = new XLangBinaryOp(context, node, token.Type, DotConcat());
                }
                else if (token.Type == XLangTokenType.OpEquality && reader.PeekNext().Type == XLangTokenType.OpEquality)
                {
                    Eat(XLangTokenType.OpEquality);
                    Eat(XLangTokenType.OpEquality);
                    node = new XLangBinaryOp(context, node, XLangTokenType.OpComparison, Expr());
                }
                else if (token.Type == XLangTokenType.OpLessThan)
                {
                    Eat(token.Type);
                    if (CurrentToken.Type == XLangTokenType.OpEquality)
                    {
                        Eat(XLangTokenType.OpEquality);
                        node = new XLangBinaryOp(context, node, XLangTokenType.OpLessOrEqual, Expr());
                    }
                    else
                    {
                        node = new XLangBinaryOp(context, node, token.Type, Expr());
                    }
                }
                else if (token.Type == XLangTokenType.OpGreaterThan)
                {
                    Eat(token.Type);
                    if (CurrentToken.Type == XLangTokenType.OpEquality)
                    {
                        Eat(XLangTokenType.OpEquality);
                        node = new XLangBinaryOp(context, node, XLangTokenType.OpGreaterOrEqual, Expr());
                    }
                    else
                    {
                        node = new XLangBinaryOp(context, node, token.Type, Expr());
                    }
                }
                else if (token.Type == XLangTokenType.OpFwdSlash)
                {
                    Eat(XLangTokenType.OpFwdSlash);
                    node = new XLangBinaryOp(context, node, token.Type, DotConcat());
                }
                else if (CurrentToken.Type == XLangTokenType.OpBracketOpen)
                {
                    Eat(XLangTokenType.OpBracketOpen);
                    List<XLangExpression> parameterList = new List<XLangExpression>();
                    bool comma = false;
                    while (CurrentToken.Type != XLangTokenType.OpBracketClose)
                    {
                        if (comma)
                        {
                            Eat(XLangTokenType.OpComma);
                            comma = false;
                        }
                        else
                        {
                            parameterList.Add(Expr());
                            comma = true;
                        }
                    }

                    Eat(XLangTokenType.OpBracketClose);

                    node = new XLangInvocationOp(context, node, parameterList.ToArray());
                }
                else if (token.Type == XLangTokenType.OpIndexerBracketOpen)
                {
                    Eat(XLangTokenType.OpIndexerBracketOpen);
                    List<XLangExpression> parameterList = new List<XLangExpression>();
                    bool comma = false;
                    while (CurrentToken.Type != XLangTokenType.OpIndexerBracketClose)
                    {
                        if (comma)
                        {
                            Eat(XLangTokenType.OpComma);
                        }
                        else
                        {
                            parameterList.Add(Expr());
                            comma = true;
                        }
                    }

                    Eat(XLangTokenType.OpIndexerBracketClose);
                    node = new XLangArrayAccessorOp(context, node, parameterList);
                }
            }

            return node;
        }

        private XLangExpression ReadIf()
        {
            List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)> conditions =
                    new List<(XLangExpression, Action<XLangRuntimeScope, IXLangRuntimeTypeInstance>)>();
            Action<XLangRuntimeScope, IXLangRuntimeTypeInstance> elseBranch = null;
            ReadBlock();

            while (CurrentToken.Type == XLangTokenType.OpIf)
            {
                ReadBlock();
            }

            if (CurrentToken.Type != XLangTokenType.OpSemicolon)
            {
                List<XLangExpression> content = ReadBlockContent();
                elseBranch = (scope, instance) => RunMulti(content, scope, instance);
            }

            void ReadBlock()
            {
                Eat(XLangTokenType.OpIf);
                Eat(XLangTokenType.OpBracketOpen);
                XLangExpression condition = Expr();
                Eat(XLangTokenType.OpBracketClose);

                List<XLangExpression> content = ReadBlockContent();
                conditions.Add((condition, (scope, instance) => RunMulti(content, scope, instance)));
                if (CurrentToken.Type == XLangTokenType.OpElse)
                {
                    Eat(XLangTokenType.OpElse);
                }
            }

            List<XLangExpression> ReadBlockContent()
            {
                if (CurrentToken.Type != XLangTokenType.OpBlockToken)
                {
                    XLangExpression expr = Expr();
                    return new List<XLangExpression> { expr };

                }

                IXLangToken token = CurrentToken;
                Eat(XLangTokenType.OpBlockToken);

                return new XLangExpressionParser(context, new XLangExpressionReader(token.GetChildren()))
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



            return new XLangIfOp(context, XLangTokenType.OpIf, conditions, elseBranch);
        }


        public XLangExpression ReadFor()
        {
            Eat(XLangTokenType.OpFor);
            Eat(XLangTokenType.OpBracketOpen);
            XLangExpression vDecl = Expr();
            Eat(XLangTokenType.OpSemicolon);
            XLangExpression condition = Expr();
            Eat(XLangTokenType.OpSemicolon);
            XLangExpression vInc = Expr();
            Eat(XLangTokenType.OpBracketClose);

            XLangExpression token = null;
            List<XLangExpression> block;
            if (CurrentToken.Type != XLangTokenType.OpBlockToken)
            {
                block = new List<XLangExpression> { Expr() };
            }
            else
            {
                block = new XLangExpressionParser(context, new XLangExpressionReader(CurrentToken.GetChildren())).Parse().ToList();
            }
            token = new XLangForOp(context, vDecl, condition, vInc, XLangTokenType.OpFor, (x, y) => RunMulti(block, x, y));


            return token;
        }

        private static void RunMulti(IEnumerable<XLangExpression> block, XLangRuntimeScope scope, IXLangRuntimeTypeInstance instance)
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

        public XLangExpression ReadWhile()
        {
            Eat(XLangTokenType.OpWhile);
            Eat(XLangTokenType.OpBracketOpen);
            XLangExpression condition = Expr();
            Eat(XLangTokenType.OpBracketClose);

            XLangExpression token = null;
            List<XLangExpression> block;
            if (CurrentToken.Type != XLangTokenType.OpBlockToken)
            {
                block = new List<XLangExpression> { Expr() };
            }
            else
            {
                block = new XLangExpressionParser(context, new XLangExpressionReader(CurrentToken.GetChildren())).Parse().ToList();
            }
            token = new XLangWhileOp(context, condition, XLangTokenType.OpWhile, RunMulti);
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

        private XLangExpression Factor()
        {
            if (CurrentToken.Type == XLangTokenType.OpWord ||
                CurrentToken.Type == XLangTokenType.OpThis ||
                CurrentToken.Type == XLangTokenType.OpBase)
            {
                IXLangToken item = CurrentToken;
                XLangExpression token = new XLangVarOperand(context, CurrentToken);
                Eat(CurrentToken.Type);
                if (CurrentToken.Type == XLangTokenType.OpWord)
                {
                    token = new XLangVarDefOperand(
                                                   context,
                                                   new VariableDefinitionToken(
                                                                               CurrentToken,
                                                                               item,
                                                                               new IXLangToken[0],
                                                                               new IXLangToken[0],
                                                                               null
                                                                              )
                                                  );
                    Eat(CurrentToken.Type);
                }
                return token;
            }



            if (CurrentToken.Type == XLangTokenType.OpVariableDefinition)
            {
                XLangExpression token = new XLangVarDefOperand(context, (VariableDefinitionToken)CurrentToken);
                Eat(XLangTokenType.OpVariableDefinition);
                return token;
            }

            if (CurrentToken.Type == XLangTokenType.OpPlus ||
                CurrentToken.Type == XLangTokenType.OpTilde ||
                CurrentToken.Type == XLangTokenType.OpBang ||
                CurrentToken.Type == XLangTokenType.OpMinus ||
                CurrentToken.Type == XLangTokenType.OpNew)
            {
                Eat(CurrentToken.Type);
                XLangExpression token = new XLangUnaryOp(context, Factor(), CurrentToken.Type);
                return token;
            }

            if (CurrentToken.Type == XLangTokenType.OpReturn)
            {
                Eat(XLangTokenType.OpReturn);
                if (CurrentToken.Type != XLangTokenType.OpSemicolon)
                    return new XLangReturnOp(context, Factor());
                return new XLangReturnOp(context, null);
            }

            if (CurrentToken.Type == XLangTokenType.OpContinue)
            {
                Eat(XLangTokenType.OpContinue);
                XLangExpression token = new XLangContinueOp(context);
                return token;
            }
            if (CurrentToken.Type == XLangTokenType.OpBreak)
            {
                Eat(XLangTokenType.OpBreak);
                XLangExpression token = new XLangBreakOp(context);
                return token;
            }

            if (CurrentToken.Type == XLangTokenType.OpNumber ||
                CurrentToken.Type == XLangTokenType.OpStringLiteral)
            {
                XLangExpression token = new XLangValueOperand(context, CurrentToken);
                Eat(CurrentToken.Type);
                return token;
            }

            if (CurrentToken.Type == XLangTokenType.OpBracketOpen)
            {
                Eat(XLangTokenType.OpBracketOpen);
                XLangExpression token = Expr();
                Eat(XLangTokenType.OpBracketClose);
                return token;
            }

            if (CurrentToken.Type == XLangTokenType.OpWhile)
            {
                return ReadWhile();
            }

            if (CurrentToken.Type == XLangTokenType.OpFor)
            {
                return ReadFor();
            }

            if (CurrentToken.Type == XLangTokenType.OpIf)
            {
                return ReadIf();
            }

            throw new Exception();
        }

        private XLangExpression Expr()
        {
            XLangExpression node = Term();
            while (CurrentToken.Type == XLangTokenType.OpPlus ||
                   CurrentToken.Type == XLangTokenType.OpMinus ||
                   CurrentToken.Type == XLangTokenType.OpEquality)
            {
                IXLangToken token = CurrentToken;
                if (token.Type == XLangTokenType.OpMinus)
                {
                    Eat(XLangTokenType.OpMinus);
                    node = new XLangBinaryOp(context, node, token.Type, Term());
                }
                else if (token.Type == XLangTokenType.OpPlus)
                {
                    Eat(XLangTokenType.OpPlus);
                    node = new XLangBinaryOp(context, node, token.Type, Term());
                }
                else if (token.Type == XLangTokenType.OpEquality)
                {
                    Eat(XLangTokenType.OpEquality);

                    node = new XLangBinaryOp(context, node, token.Type, Expr());

                }
            }

            return node;
        }

    }
}
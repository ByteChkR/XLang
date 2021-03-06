﻿using XLang.Core;
using XLang.Exceptions;
using XLang.Parser.Token;
using XLang.Parser.Token.Combined;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operands;
using XLang.Parser.Token.Expressions.Operators;
using XLang.Parser.Token.Expressions.Operators.Special;

namespace XLang.Parser.Expressions
{
    /// <summary>
    ///     Implements AXLangExpressionValueCreator
    /// </summary>
    public class XLangExpressionValueCreator : AXLangExpressionValueCreator
    {
        /// <summary>
        ///     Creates a Value based on the Current State of the Expression Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
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
                IXLangToken rt = parser.CurrentToken;
                parser.Eat(XLangTokenType.OpReturn);
                if (parser.CurrentToken.Type == XLangTokenType.OpSemicolon)
                {
                    return new XLangReturnOp(parser.Context, null, rt.SourceIndex);
                }
                return new XLangReturnOp(parser.Context, parser.ParseExpr(0), rt.SourceIndex);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpContinue)
            {
                IXLangToken ct = parser.CurrentToken;
                parser.Eat(XLangTokenType.OpContinue);
                return new XLangContinueOp(parser.Context, ct.SourceIndex);
            }
            if (parser.CurrentToken.Type == XLangTokenType.OpBreak)
            {
                IXLangToken bt = parser.CurrentToken;
                parser.Eat(XLangTokenType.OpBreak);
                return new XLangBreakOp(parser.Context, bt.SourceIndex);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpIf)
            {
                return XLangSpecialOps.ReadIf(parser);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpFor)
            {
                return XLangSpecialOps.ReadFor(parser);
            }

            if (parser.CurrentToken.Type == XLangTokenType.OpWhile)
            {
                return XLangSpecialOps.ReadWhile(parser);
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
                XLangExpression token = new XLangVarOperand(parser.Context, parser.CurrentToken,
                    parser.CurrentToken.SourceIndex);
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
                    token = new XLangVarOperand(parser.Context, item, item.SourceIndex);
                }

                return token;
            }


            if (parser.CurrentToken.Type == XLangTokenType.OpVariableDefinition)
            {
                XLangExpression token =
                    new XLangVarDefOperand(parser.Context, (VariableDefinitionToken) parser.CurrentToken);
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


            throw new XLangTokenParseException("Invalid Token: " + parser.CurrentToken.Type);
        }
    }
}
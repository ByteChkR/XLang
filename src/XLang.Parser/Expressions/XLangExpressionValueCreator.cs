using System;
using XLang.Core;
using XLang.Parser.Token;
using XLang.Parser.Token.Combined;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operands;
using XLang.Parser.Token.Expressions.Operators;
using XLang.Parser.Token.Expressions.Operators.Special;

namespace XLang.Parser.Expressions
{
    public class XLangExpressionValueCreator : AXLangExpressionValueCreator
    {
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


            throw new Exception("Invalid Token: " + parser.CurrentToken.Type);
        }

        #region Specials

        #endregion
    }
}
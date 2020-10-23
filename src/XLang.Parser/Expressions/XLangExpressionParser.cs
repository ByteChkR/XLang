using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using XLang.Core;
using XLang.Parser.Reader;
using XLang.Parser.Shared;
using XLang.Parser.Token;
using XLang.Parser.Token.Expressions;

namespace XLang.Parser.Expressions
{
    public class XLangExpressionParser
    {

        public readonly XLangContext Context;
        public readonly XLangExpressionReader Reader;
        public readonly XLangExpressionValueCreator ValueCreator;
        public readonly XLangExpressionOperatorCollection OpCollection;

        public static XLangExpressionParser Create(XLangContext context, XLangExpressionReader reader)
        {
            XLangExpressionOperator[] operators = new XLangExpressionOperator[]
                                                  {
                                                      new XLangArraySubscriptOperator(),
                                                      new XLangAssignmentOperators(),
                                                      new XLangBitAndOperators(),
                                                      new XLangBitOrOperators(),
                                                      new XLangBitXOrOperators(),
                                                      new XLangInEqualityOperators(),
                                                      new XLangEqualityOperators(),
                                                      new XLangInvocationSelectorOperator(),
                                                      new XLangLogicalAndOperators(),
                                                      new XLangLogicalOrOperators(),
                                                      new XLangMemberSelectorOperator(),
                                                      new XLangMulDivModOperators(),
                                                      new XLangPlusMinusOperators(),
                                                      new XLangRelationOperators(),
                                                      new XLangUnaryOperators(),

                                                  };
            XLangExpressionValueCreator valueCreator = new XLangExpressionValueCreator();
            return new XLangExpressionParser(context, reader, valueCreator, operators);

        }

        public XLangExpressionParser(XLangContext context, XLangExpressionReader reader, XLangExpressionValueCreator valueCreator, XLangExpressionOperator[] operators)
        {
            OpCollection = new XLangExpressionOperatorCollection(operators);
            ValueCreator = valueCreator;
            Context = context;
            Reader = reader;
            CurrentToken = reader.GetNext();
        }

        public IXLangToken CurrentToken { get; private set; }

        public XLangExpression[] Parse()
        {
            if (CurrentToken.Type == XLangTokenType.EOF) return new XLangExpression[0];
            List<XLangExpression> ret = new List<XLangExpression> { ParseExpr(OpCollection.Lowest) };
            while (CurrentToken.Type == XLangTokenType.OpSemicolon || CurrentToken.Type == XLangTokenType.OpBlockToken || CurrentToken.Type == XLangTokenType.OpWord)
            {
                if (CurrentToken.Type == XLangTokenType.OpSemicolon || CurrentToken.Type == XLangTokenType.OpBlockToken)
                    Eat(CurrentToken.Type);
                if (CurrentToken.Type == XLangTokenType.EOF) break;
                ret.Add(ParseExpr(OpCollection.Lowest));
            }
            return ret.ToArray();
        }

        public void Eat(XLangTokenType type)
        {
            if (CurrentToken.Type == type)
            {
                CurrentToken = Reader.GetNext();
            }
            else
            {
                throw new XLangTokenReadException(Reader.tokens, type, CurrentToken.Type, CurrentToken.StartIndex);
            }
        }


        public XLangExpression ParseExpr(int startAt)
        {
            XLangExpression node = ValueCreator.CreateValue(this);

            for (int i = startAt; i <= OpCollection.Highest; i++)
            {
                if (!OpCollection.HasLevel(i)) continue;

                List<XLangExpressionOperator> ops = OpCollection.GetLevel(i);

                XLangExpressionOperator current = null;
                while ((current = ops.FirstOrDefault(x => x.CanCreate(this, node))) != null)
                {
                    node = current.Create(this, node);
                    i = startAt;
                }
            }

            return node;
        }

    }
}
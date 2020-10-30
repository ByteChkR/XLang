using System.Collections.Generic;
using System.Linq;
using XLang.Core;
using XLang.Parser.Exceptions;
using XLang.Parser.Expressions.Operators;
using XLang.Parser.Reader;
using XLang.Parser.Shared;
using XLang.Parser.Token;
using XLang.Parser.Token.Expressions;

namespace XLang.Parser.Expressions
{
    /// <summary>
    ///     Parses XLangExpressions from a Token Stream
    /// </summary>
    public class XLangExpressionParser
    {
        /// <summary>
        ///     XL Context
        /// </summary>
        public readonly XLangContext Context;

        /// <summary>
        ///     Operator Collection
        /// </summary>
        public readonly XLangExpressionOperatorCollection OpCollection;

        /// <summary>
        ///     Token Reader
        /// </summary>
        public readonly XLangExpressionReader Reader;

        /// <summary>
        ///     Value Creator
        /// </summary>
        public readonly AXLangExpressionValueCreator ValueCreator;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="reader">Token Reader</param>
        /// <param name="valueCreator">XLExpression Value Creator</param>
        /// <param name="operators">Operator Collection</param>
        public XLangExpressionParser(XLangContext context, XLangExpressionReader reader,
            AXLangExpressionValueCreator valueCreator, XLangExpressionOperator[] operators)
        {
            OpCollection = new XLangExpressionOperatorCollection(operators);
            ValueCreator = valueCreator;
            Context = context;
            Reader = reader;
            CurrentToken = reader.GetNext();
        }

        /// <summary>
        ///     The Current Token
        /// </summary>
        public IXLangToken CurrentToken { get; private set; }

        /// <summary>
        ///     Creates a XLExpressionParser instance
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="reader">Expression Reader</param>
        /// <returns></returns>
        public static XLangExpressionParser Create(XLangContext context, XLangExpressionReader reader)
        {
            XLangExpressionOperator[] operators =
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
                new XLangUnaryOperators()

            };
            XLangExpressionValueCreator valueCreator = new XLangExpressionValueCreator();
            return new XLangExpressionParser(context, reader, valueCreator, operators);

        }

        /// <summary>
        ///     Parses the Expressions inside the Token Reader Stream
        /// </summary>
        /// <returns></returns>
        public XLangExpression[] Parse()
        {
            if (CurrentToken.Type == XLangTokenType.EOF)
            {
                return new XLangExpression[0];
            }
            List<XLangExpression> ret = new List<XLangExpression> {ParseExpr(OpCollection.Lowest)};
            while (CurrentToken.Type != XLangTokenType.EOF)
            {
                if (CurrentToken.Type == XLangTokenType.OpSemicolon || CurrentToken.Type == XLangTokenType.OpBlockToken)
                {
                    Eat(CurrentToken.Type);
                }
                if (CurrentToken.Type == XLangTokenType.EOF)
                {
                    break;
                }
                ret.Add(ParseExpr(OpCollection.Lowest));
            }
            return ret.ToArray();
        }

        /// <summary>
        ///     Consumes the Specified Token
        ///     Throws an Error if a different token was found
        /// </summary>
        /// <param name="type">Expected Token Type</param>
        public void Eat(XLangTokenType type)
        {
            if (CurrentToken.Type == type)
            {
                CurrentToken = Reader.GetNext();
            }
            else
            {
                throw new XLangTokenReadException(Reader.tokens, type, CurrentToken.Type, CurrentToken.SourceIndex);
            }
        }


        /// <summary>
        ///     Parses the Expression starting at the specified Operator Precedence
        /// </summary>
        /// <param name="startAt">Operator Precedence</param>
        /// <returns>Expression at the Specified Index</returns>
        public XLangExpression ParseExpr(int startAt)
        {
            XLangExpression node = ValueCreator.CreateValue(this);

            for (int i = startAt; i <= OpCollection.Highest; i++)
            {
                if (!OpCollection.HasLevel(i))
                {
                    continue;
                }

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
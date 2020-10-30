using System.Collections.Generic;
using NUnit.Framework;
using XLang.Core;
using XLang.Parser.Exceptions;
using XLang.Parser.Shared;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;
using XLang.Parser.Token.Combined;

namespace XLang.Parser.Tests
{
    public class XLParsingToolTests
    {
        private static readonly List<IXLangToken> tokens = new List<IXLangToken>
        {
            new ClassDefinitionToken(
                new TextToken(XLangTokenType.OpClass, "class", 0),
                new TextToken(XLangTokenType.OpWord, "C", 1),
                new TextToken(XLangTokenType.OpWord, "object", 1),
                new IXLangToken[0],
                new IXLangToken[0]),
            new TextToken(XLangTokenType.OpSemicolon, ";", 0),
            new TextToken(XLangTokenType.OpFunctionDefinition, ";", 0),
            new TextToken(XLangTokenType.OpSemicolon, ";", 0),
            new TextToken(XLangTokenType.OpClassDefinition, ";", 0)
        };

        [Test]
        public void ReadAny()
        {
            IXLangToken cld = XLangParsingTools.ReadAny(tokens, 0);
            Assert.True(cld.Type == XLangTokenType.OpClassDefinition);
            Assert.Throws<XLangTokenReadException>(() => XLangParsingTools.ReadAny(tokens, tokens.Count));
        }

        [Test]
        public void ReadOneOfAny()
        {
            IXLangToken cld = XLangParsingTools.ReadOneOfAny(tokens, 0, new[] {XLangTokenType.OpClassDefinition});
            Assert.True(cld.Type == XLangTokenType.OpClassDefinition);
            Assert.Throws<XLangTokenReadException>(() =>
                XLangParsingTools.ReadOneOfAny(tokens, 0, new[] {XLangTokenType.OpSemicolon}));
        }

        [Test]
        public void ReadOneOrManyOf()
        {
            Assert.True(XLangParsingTools.ReadOneOrManyOf(tokens, 1, 1, new[] {XLangTokenType.OpSemicolon}).Length ==
                        1);
            Assert.Throws<XLangTokenReadException>(() =>
                XLangParsingTools.ReadOneOrManyOf(tokens, 1, 1, new[] {XLangTokenType.OpClassDefinition}));
        }

        [Test]
        public void ReadOne()
        {
            Assert.True(XLangParsingTools.ReadOne(tokens, 0, XLangTokenType.OpClassDefinition).Type ==
                        XLangTokenType.OpClassDefinition);
            Assert.Throws<XLangTokenReadException>(() =>
                XLangParsingTools.ReadOne(tokens, 0, XLangTokenType.OpSemicolon));
        }

        [Test]
        public void ReadNoneOrMany()
        {
            Assert.True(XLangParsingTools.ReadNoneOrMany(tokens, 0, 1, XLangTokenType.OpClassDefinition).Length == 1);
            Assert.True(XLangParsingTools.ReadNoneOrMany(tokens, 0, 1, XLangTokenType.OpSemicolon).Length == 0);
        }

        [Test]
        public void ReadOneOrMany()
        {
            Assert.True(XLangParsingTools.ReadOneOrMany(tokens, 0, 1, XLangTokenType.OpClassDefinition).Length == 1);
            Assert.Throws<XLangTokenReadException>(() =>
                XLangParsingTools.ReadOneOrMany(tokens, 0, 1, XLangTokenType.OpSemicolon));
        }
    }
}
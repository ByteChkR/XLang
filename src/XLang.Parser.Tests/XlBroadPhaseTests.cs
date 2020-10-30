using System.Collections.Generic;
using NUnit.Framework;
using XLang.Parser.Base;
using XLang.Parser.Exceptions;
using XLang.Parser.Reader;
using XLang.Parser.Token;

namespace XLang.Parser.Tests
{
    public class XlBroadPhaseTests
    {
        private static readonly string WrongStringLiteralCode = "\"StringLiteral without ending quote\n";
        private static readonly string StringLiteralCode = "\"StringLiteral without ending quote\"\n";

        private static readonly string CommentCode = @"
public static class Program
{
public void Main()
{
    //Comment \!$!%(=&=/_:;_; with special chars trailing comment begin tag//
}

}//TestComment with no newline at end";

        [Test]
        public void StringParsing()
        {
            XLangSettings s = new XLangSettings();
            XLangBaseReader r = new XLangBaseReader(s, StringLiteralCode);
            List<IXLangToken> l = r.ReadToEnd();
            XLangBroadParser.ElevateOneLineString(s, l);

            Assert.True(l.Count == 2);

            r = new XLangBaseReader(s, WrongStringLiteralCode);
            l = r.ReadToEnd();

            Assert.Throws<XLangTokenReadException>(() => XLangBroadParser.ElevateOneLineString(s, l));
        }

        [Test]
        public void CommentParsing()
        {
            XLangSettings s = new XLangSettings();
            XLangBaseReader r = new XLangBaseReader(s, CommentCode);
            List<IXLangToken> l = r.ReadToEnd();
            XLangBroadParser.ElevateOneLineComment(s, l);

        }
    }
}
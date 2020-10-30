using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using XLang.BaseTypes;
using XLang.Core;
using XLang.CSharp;
using XLang.Parser.Base;
using XLang.Parser.Exceptions;
using XLang.Parser.Reader;
using XLang.Parser.Shared;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;
using XLang.Parser.Token.Combined;
using XLang.Runtime;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Types;

namespace XLang.Parser.Tests
{
    public class XLBroadPhaseTests
    {
        private static string WrongStringLiteralCode = "\"StringLiteral without ending quote\n";
        private static string StringLiteralCode = "\"StringLiteral without ending quote\"\n";

        private static string CommentCode = @"
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

    public class XLParsingToolTests
    {
        private static List<IXLangToken> tokens = new List<IXLangToken>
        {
            new ClassDefinitionToken(
                new TextToken(XLangTokenType.OpClass, "class", 0),
                new TextToken(XLangTokenType.OpWord, "C",1),
                new TextToken(XLangTokenType.OpWord, "object", 1),
                new IXLangToken[0],
                new IXLangToken[0]),
            new TextToken(XLangTokenType.OpSemicolon, ";", 0),
            new TextToken(XLangTokenType.OpFunctionDefinition, ";", 0),
            new TextToken(XLangTokenType.OpSemicolon, ";", 0),
            new TextToken(XLangTokenType.OpClassDefinition, ";", 0),
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
            IXLangToken cld = XLangParsingTools.ReadOneOfAny(tokens, 0, new[] { XLangTokenType.OpClassDefinition });
            Assert.True(cld.Type == XLangTokenType.OpClassDefinition);
            Assert.Throws<XLangTokenReadException>(() =>
                XLangParsingTools.ReadOneOfAny(tokens, 0, new[] { XLangTokenType.OpSemicolon }));
        }

        [Test]
        public void ReadOneOrManyOf()
        {
            Assert.True(XLangParsingTools.ReadOneOrManyOf(tokens, 1, 1, new[] { XLangTokenType.OpSemicolon }).Length ==
                        1);
            Assert.Throws<XLangTokenReadException>(() =>
                XLangParsingTools.ReadOneOrManyOf(tokens, 1, 1, new[] { XLangTokenType.OpClassDefinition }));
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


    public class XLTests
    {
        private static string[] GetExampleFiles()
        {
            return Directory.GetFiles("../../../../../examples", "*.xl", SearchOption.AllDirectories)
                .Select(Path.GetFullPath).ToArray();
        }

        [Test]
        [TestCaseSource("GetExampleFiles")]
        public void LexerTest(string file)
        {
            XLangSettings settings = new XLangSettings();
            string source = File.ReadAllText(file);
            XLangBaseReader lx = new XLangBaseReader(settings, source);
            List<IXLangToken> tokenStream = new List<IXLangToken>();
            IXLangToken token = null;
            while ((token = lx.Advance()).Type != XLangTokenType.EOF)
            {
                tokenStream.Add(token);
            }
        }

        [Test]
        [TestCaseSource("GetExampleFiles")]
        public void ParseTest(string file)
        {
            XLangParser parser = new XLangParser();
            parser.Parse(File.ReadAllText(file));
        }


        [Test]
        [TestCaseSource("GetExampleFiles")]
        public void ExecutionTest(string file)
        {
            StringBuilder sb = new StringBuilder();
            XLangContext c = new XLangContext(new XLangSettings(), "XL");

            Assert.True(c.TryGet("XL", out XLangRuntimeNamespace cNs) && cNs is XLCoreNamespace);
            XLCoreNamespace coreNs = (XLCoreNamespace)cNs;

            coreNs.SetWritelineImpl(x => sb.AppendLine(x));

            CSharpClassTunnel.LoadTunnel(c);

            XLangParser parser = new XLangParser(c);
            parser.Parse(File.ReadAllText(file));

            IXLangRuntimeFunction entry = c.GetType("DEFAULT.Program")?.GetMember("Main") as IXLangRuntimeFunction;

            Assert.True(entry != null);

            entry.Invoke(null, new IXLangRuntimeTypeInstance[entry.ParameterList.Length]);


            Assert.Pass(sb.ToString());
        }
    }
}
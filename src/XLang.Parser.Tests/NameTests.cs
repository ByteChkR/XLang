using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using XLang.Core;
using XLang.Parser.Reader;
using XLang.Parser.Token;

namespace XLang.Parser.Tests
{
    public class NameTests
    {

        private static string[] GetExampleFiles()
        {
            return Directory.GetFiles(".\\tests", "*.xl", SearchOption.AllDirectories);
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

    }
}
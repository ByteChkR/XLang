using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using XLang.BaseTypes;
using XLang.Core;
using XLang.Parser.Reader;
using XLang.Parser.Token;
using XLang.Runtime;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Types;

namespace XLang.Parser.Tests
{
    public class XLTests
    {
        private static string[] GetExampleFiles()
        {
            return Directory.GetFiles("..\\..\\..\\..\\..\\examples", "*.xl", SearchOption.AllDirectories)
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
            XLCoreNamespace coreNs = (XLCoreNamespace) cNs;

            coreNs.SetWritelineImpl(x => sb.AppendLine(x));

            XLangParser parser = new XLangParser(c);
            parser.Parse(File.ReadAllText(file));

            IXLangRuntimeFunction entry = c.GetType("DEFAULT.Program")?.GetMember("Main") as IXLangRuntimeFunction;

            Assert.True(entry != null);

            entry.Invoke(null, new IXLangRuntimeTypeInstance[entry.ParameterList.Length]);


            Assert.Pass(sb.ToString());
        }
    }
}
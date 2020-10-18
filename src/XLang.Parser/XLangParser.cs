using System.Collections.Generic;
using System.Linq;

using XLang.Core;
using XLang.Parser.Base;
using XLang.Parser.Reader;
using XLang.Parser.Token;

namespace XLang.Parser
{
    public class XLangParser
    {

        private readonly XLangContext Context;

        public XLangParser() : this(new XLangContext(new XLangSettings(), "XL"))
        {
        }

        public XLangParser(XLangContext context)
        {
            Context = context;
        }

        public void Parse(string source)
        {
            XLangBaseReader lx = new XLangBaseReader(Context.Settings, source);
            List<IXLangToken> tokenStream = new List<IXLangToken>();
            IXLangToken token = null;
            while ((token = lx.Advance()).Type != XLangTokenType.EOF)
            {
                tokenStream.Add(token);
            }

            //Elevate
            XLangBroadParser.ElevateOneLineComment(Context.Settings, tokenStream);

            tokenStream = tokenStream.Where(x => x.Type != XLangTokenType.OpNewLine).ToList();


            XLangBroadParser.ElevateOneLineString(Context.Settings, tokenStream);
            XLangBroadParser.ElevateReservedKeys(Context.Settings, tokenStream);
            XLangBroadParser.ElevateBlocks(Context.Settings, tokenStream);
            XLangBroadParser.ElevateNamespace(Context.Settings, tokenStream);
            XLangBroadParser.ElevateClass(Context.Settings, tokenStream);
            XLangBroadParser.ElevateStatements(Context.Settings, tokenStream);


            XLangBroadParser.ElevateTypeDef(Context, tokenStream);
            XLangBroadParser.ElevateFunctionDef(Context.Settings, tokenStream);
            XLangBroadParser.ElevateExpressions(Context, tokenStream);

            XLangBroadParser.ElevateToRuntimeTokens(
                                                    tokenStream,
                                                    Context
                                                   );
        }

    }
}
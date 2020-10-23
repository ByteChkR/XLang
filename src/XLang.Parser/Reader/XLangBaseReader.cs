using System;
using System.Text;
using XLang.Core;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;

namespace XLang.Parser.Reader
{
    public class XLangBaseReader
    {
        public readonly string Input;
        private readonly XLangSettings settings;

        private int currentIndex;
        private IXLangToken currentToken;

        public XLangBaseReader(XLangSettings settings, string input)
        {
            Input = input;
            this.settings = settings;
        }


        public IXLangToken Advance()
        {
            if (currentIndex < Input.Length)
            {
                if (IsNewLine(Input[currentIndex]))
                {
                    currentToken = ReadNewLine();
                }
                else if (IsSpace(Input[currentIndex]))
                {
                    currentToken = ReadSpace();
                    currentToken = Advance();
                }
                else if (IsSymbol(Input[currentIndex]))
                {
                    currentToken = ReadSymbol();
                }
                else if (IsNumber(Input[currentIndex]))
                {
                    currentToken = ReadNumber();
                }
                else if (IsLetter(Input[currentIndex]))
                {
                    currentToken = ReadWord();
                }
                else
                {
                    throw new Exception($"Unknown Character '{Input[currentIndex]}' at index: {currentIndex}");
                }

                return currentToken;
            }

            currentToken = new TextToken(XLangTokenType.EOF, "", Input.Length);

            return currentToken;
        }


        private bool IsSymbol(char c)
        {
            return settings.ReservedSymbols.ContainsKey(c);
        }

        private static bool IsNumber(char c)
        {
            return char.IsDigit(c);
        }

        private static bool IsLetter(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private IXLangToken ReadSpace()
        {
            int len = 0;
            int start = currentIndex;
            do
            {
                len++;
                currentIndex++;
            } while (currentIndex < Input.Length && IsSpace(Input[currentIndex]));

            return new TextToken(XLangTokenType.OpSpace, new StringBuilder().Append(' ', len).ToString(), start);
        }

        private IXLangToken ReadNewLine()
        {
            int len = 0;
            int start = currentIndex;
            do
            {
                len++;
                currentIndex++;
            } while (currentIndex < Input.Length && IsNewLine(Input[currentIndex]));

            return new TextToken(XLangTokenType.OpNewLine, new StringBuilder().Append('\n', len).ToString(), start);
        }

        private IXLangToken ReadWord()
        {
            int start = currentIndex;
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(Input[currentIndex]);
                currentIndex++;
            } while (currentIndex < Input.Length && (IsNumber(Input[currentIndex]) || IsLetter(Input[currentIndex])));

            return new TextToken(XLangTokenType.OpWord, sb.ToString(), start);
        }

        private IXLangToken ReadNumber()
        {
            int start = currentIndex;
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(Input[currentIndex]);
                currentIndex++;
            } while (currentIndex < Input.Length && IsNumber(Input[currentIndex]));

            return new TextToken(XLangTokenType.OpNumber, sb.ToString(), start);
        }

        private IXLangToken ReadSymbol()
        {
            char val = Input[currentIndex];
            int start = currentIndex;
            currentIndex++;
            return new TextToken(settings.ReservedSymbols[val], val.ToString(), start);
        }

        private static bool IsNewLine(char c)
        {
            return c == '\n';
        }

        private static bool IsSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r';
        }
    }
}
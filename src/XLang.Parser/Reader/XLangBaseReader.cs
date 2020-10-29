using System;
using System.Text;
using XLang.Core;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;

/// <summary>
/// Contains the Different Token Reader Implementations for the Different parsing phases.
/// </summary>
namespace XLang.Parser.Reader
{
    /// <summary>
    ///     XLang base Reader that is used by the BoardParser
    /// </summary>
    public class XLangBaseReader
    {
        /// <summary>
        ///     Input Source
        /// </summary>
        public readonly string Input;
        /// <summary>
        ///     XL Settings
        /// </summary>
        private readonly XLangSettings settings;
        /// <summary>
        ///     The Current Reader Index.
        /// </summary>
        private int currentIndex;
        /// <summary>
        ///     The Current Token
        /// </summary>
        private IXLangToken currentToken;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="input">Source</param>
        public XLangBaseReader(XLangSettings settings, string input)
        {
            Input = input;
            this.settings = settings;
        }


        /// <summary>
        ///     Advances the reader by one position inside the source tokens
        /// </summary>
        /// <returns></returns>
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
                    //currentToken = Advance();
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


        /// <summary>
        ///     Returns true if the Character is a Reserved Symbol
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if Reserved Symbol</returns>
        private bool IsSymbol(char c)
        {
            return settings.ReservedSymbols.ContainsKey(c);
        }


        /// <summary>
        ///     Returns true if the Character is a Number
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if number</returns>
        private static bool IsNumber(char c)
        {
            return char.IsDigit(c);
        }

        /// <summary>
        ///     Returns true if the character is a letter or underscore
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns>True if Letter or Underscore</returns>
        private static bool IsLetter(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        /// <summary>
        ///     Reads a sequence of spaces until the first "non-space" character is encountered.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     Reads all new Lines until a "non-newline" character is encountered.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     Reads a Word from the Source
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     reads a number from the source
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     Reads a Symbol from the Source
        /// </summary>
        /// <returns></returns>
        private IXLangToken ReadSymbol()
        {
            char val = Input[currentIndex];
            int start = currentIndex;
            currentIndex++;
            return new TextToken(settings.ReservedSymbols[val], val.ToString(), start);
        }

        /// <summary>
        ///     Returns true if the character is a new line '\n'
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns></returns>
        private static bool IsNewLine(char c)
        {
            return c == '\n';
        }

        /// <summary>
        ///     Returns true if the character is a new line ' ' || '\t' || '\r'
        /// </summary>
        /// <param name="c">Character to Check</param>
        /// <returns></returns>
        private static bool IsSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r';
        }
    }
}
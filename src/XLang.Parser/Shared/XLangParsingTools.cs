using System.Collections.Generic;
using XLang.Core;
using XLang.Parser.Exceptions;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;


/// <summary>
/// Contains Shared Logic between different Parser Stages.
/// </summary>
namespace XLang.Parser.Shared
{
    /// <summary>
    ///     Collection of Tools for Parsing through a stream of tokens.
    /// </summary>
    public static class XLangParsingTools
    {
        /// <summary>
        ///     Reads any symbol at the specified index.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <returns>Token at index start</returns>
        public static IXLangToken ReadAny(List<IXLangToken> tokens, int start)
        {
            if (!ReadAnyOrNone(tokens, start, out IXLangToken ret))
            {
                throw new XLangTokenReadException(tokens, XLangTokenType.Any, ret.Type, start);
            }

            return ret;
        }

        /// <summary>
        ///     Reads Any token or none if the token stream reached the end
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="result">Specified Tokens</param>
        /// <returns></returns>
        public static bool ReadAnyOrNone(List<IXLangToken> tokens, int start, out IXLangToken result)
        {
            if (start >= 0 && tokens.Count > start)
            {
                result = tokens[start];

                return true;
            }

            result = new TextToken(XLangTokenType.EOF, "", tokens.Count);
            return false;
        }

        /// <summary>
        ///     Reads None or any of the specified tokens
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="type">Accepted Tokens</param>
        /// <param name="result">Read Token</param>
        /// <returns>True if any token was read.</returns>
        public static bool ReadNoneOrAnyOf(
            List<IXLangToken> tokens, int start, XLangTokenType[] type, out IXLangToken result)
        {
            foreach (XLangTokenType tokenType in type)
            {
                if (ReadOneOrNone(tokens, start, tokenType, out result))
                {
                    return true;
                }

                if (result.Type == XLangTokenType.EOF)
                {
                    return false;
                }
            }

            result = tokens[start];
            return false;
        }

        /// <summary>
        ///     reads exactly one token of the specifed tokens.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="type">Accepted Types</param>
        /// <returns>The read token</returns>
        public static IXLangToken ReadOneOfAny(List<IXLangToken> tokens, int start, XLangTokenType[] type)
        {
            if (!ReadNoneOrAnyOf(tokens, start, type, out IXLangToken ret))
            {
                throw new XLangTokenReadException(tokens, type, ret.Type, start);
            }

            return ret;
        }

        /// <summary>
        ///     Reads one or more tokens of the specified types
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Types</param>
        /// <returns>Read Tokens</returns>
        public static IXLangToken[] ReadOneOrManyOf(
            List<IXLangToken> tokens, int start, int step, XLangTokenType[] type)
        {
            List<IXLangToken> ret = new List<IXLangToken> {ReadOneOfAny(tokens, start, type)};
            ret.AddRange(ReadNoneOrManyOf(tokens, start + step, step, type));
            return ret.ToArray();
        }

        /// <summary>
        ///     Reads none or more tokens of the accepted tokens
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Types</param>
        /// <returns>Read Tokens</returns>
        public static IXLangToken[] ReadNoneOrManyOf(
            List<IXLangToken> tokens, int start, int step, XLangTokenType[] type)
        {
            List<IXLangToken> res = new List<IXLangToken>();
            int currentIdx = start;
            while (true)
            {
                if (ReadNoneOrAnyOf(tokens, currentIdx, type, out IXLangToken found))
                {
                    res.Add(found);
                    currentIdx += step;
                }
                else
                {
                    return res.ToArray();
                }
            }
        }

        /// <summary>
        ///     Reads Exactly one token of a specified type
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="type">Accepted Type</param>
        /// <returns></returns>
        public static IXLangToken ReadOne(List<IXLangToken> tokens, int start, XLangTokenType type)
        {
            if (!ReadOneOrNone(tokens, start, type, out IXLangToken ret))
            {
                throw new XLangTokenReadException(tokens, type, ret.Type, start);
            }

            return ret;
        }


        /// <summary>
        ///     reads one or none token of the accepted type
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="type">Token Type</param>
        /// <param name="result">Read Token</param>
        /// <returns>True if token was read.</returns>
        public static bool ReadOneOrNone(
            List<IXLangToken> tokens, int start, XLangTokenType type, out IXLangToken result)
        {
            if (start >= 0 && tokens.Count > start)
            {
                result = tokens[start];
                if (tokens[start].Type == type)
                {
                    return true;
                }

                return false;
            }

            result = new TextToken(XLangTokenType.EOF, "", tokens.Count);
            return false;
        }

        /// <summary>
        ///     Reads none or many of the specified token
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Type</param>
        /// <returns>Read Tokens</returns>
        public static IXLangToken[] ReadNoneOrMany(List<IXLangToken> tokens, int start, int step, XLangTokenType type)
        {
            List<IXLangToken> ret = new List<IXLangToken>();
            int currentStart = start;
            while (ReadOneOrNone(tokens, currentStart, type, out IXLangToken current))
            {
                currentStart += step;
                ret.Add(current);
            }

            return ret.ToArray();
        }

        /// <summary>
        ///     Reads one or many tokens of the specified type
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Token</param>
        /// <returns>Read Tokens</returns>
        public static IXLangToken[] ReadOneOrMany(List<IXLangToken> tokens, int start, int step, XLangTokenType type)
        {
            List<IXLangToken> ret = new List<IXLangToken> {ReadOne(tokens, start, type)};
            ret.AddRange(ReadNoneOrMany(tokens, start + step, step, type));
            return ret.ToArray();
        }


        /// <summary>
        ///     Reads until end of stream or the specified type has been read.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read Token</param>
        /// <param name="type">End token type</param>
        /// <returns>Read Tokens</returns>
        public static IXLangToken[] ReadUntil(List<IXLangToken> tokens, int start, int step, XLangTokenType type)
        {
            return ReadUntilAny(tokens, start, step, new[] {type});
        }

        /// <summary>
        ///     Reads until any of the End Tokens were read.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read Token</param>
        /// <param name="type">End Tokens</param>
        /// <returns>Read Tokens</returns>
        public static IXLangToken[] ReadUntilAny(List<IXLangToken> tokens, int start, int step, XLangTokenType[] type)
        {
            List<IXLangToken> ret = new List<IXLangToken>();
            int currentStart = start;
            while (true)
            {
                if (ReadNoneOrAnyOf(tokens, currentStart, type, out IXLangToken result))
                {
                    return ret.ToArray();
                }

                currentStart += step;
                if (result.Type != XLangTokenType.EOF)
                {
                    ret.Add(result);
                }
                else
                {
                    return ret.ToArray();
                }
            }
        }
    }
}
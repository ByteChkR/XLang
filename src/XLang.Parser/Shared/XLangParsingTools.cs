using System.Collections.Generic;

using XLang.Core;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;

namespace XLang.Parser.Shared
{
    public static class XLangParsingTools
    {

        public static IXLangToken ReadAny(List<IXLangToken> tokens, int start)
        {
            if (!ReadAnyOrNone(tokens, start, out IXLangToken ret))
            {
                throw new XLangTokenReadException(tokens, XLangTokenType.Any, ret.Type, start);
            }

            return ret;
        }

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

        public static IXLangToken ReadOneOfAny(List<IXLangToken> tokens, int start, XLangTokenType[] type)
        {
            if (!ReadNoneOrAnyOf(tokens, start, type, out IXLangToken ret))
            {
                throw new XLangTokenReadException(tokens, type, ret.Type, start);
            }

            return ret;
        }

        public static IXLangToken[] ReadOneOrManyOf(
            List<IXLangToken> tokens, int start, int step, XLangTokenType[] type)
        {
            List<IXLangToken> ret = new List<IXLangToken> { ReadOneOfAny(tokens, start, type) };
            ret.AddRange(ReadNoneOrManyOf(tokens, start + step, step, type));
            return ret.ToArray();
        }

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

        public static IXLangToken ReadOne(List<IXLangToken> tokens, int start, XLangTokenType type)
        {
            if (!ReadOneOrNone(tokens, start, type, out IXLangToken ret))
            {
                throw new XLangTokenReadException(tokens, type, ret.Type, start);
            }

            return ret;
        }


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

        public static IXLangToken[] ReadOneOrMany(List<IXLangToken> tokens, int start, int step, XLangTokenType type)
        {
            List<IXLangToken> ret = new List<IXLangToken> { ReadOne(tokens, start, type) };
            ret.AddRange(ReadNoneOrMany(tokens, start + step, step, type));
            return ret.ToArray();
        }


        public static IXLangToken[] ReadUntil(List<IXLangToken> tokens, int start, int step, XLangTokenType type)
        {
            return ReadUntilAny(tokens, start, step, new[] { type });
        }

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
using System.Collections.Generic;
using System.Linq;
using XLang.Parser.Shared;
using XLang.Parser.Token;

namespace XLang.Parser.Reader
{
    /// <summary>
    ///     XLang Expression Reader Implementation
    /// </summary>
    public class XLangExpressionReader
    {
        /// <summary>
        ///     Input Token Stream
        /// </summary>
        public readonly List<IXLangToken> Tokens;

        /// <summary>
        ///     The Current Position inside the token Stream
        /// </summary>
        private int currentIdx;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        public XLangExpressionReader(List<IXLangToken> tokens)
        {
            this.Tokens = tokens.ToList();
        }

        /// <summary>
        ///     Peeks into the next (or specified) position relative to the current position
        /// </summary>
        /// <param name="advance">Relative offset to the current position</param>
        /// <returns>Token at the specified Position</returns>
        public IXLangToken PeekNext(int advance)
        {
            XLangParsingTools.ReadAnyOrNone(Tokens, currentIdx + advance - 1, out IXLangToken result);
            return result;
        }

        /// <summary>
        ///     Peeks into the next (or specified) position relative to the current position
        /// </summary>
        /// <returns>Token at the specified Position</returns>
        public IXLangToken PeekNext()
        {
            return PeekNext(1);
        }

        /// <summary>
        ///     Advances the Reader by one position and returns the read token
        /// </summary>
        /// <returns></returns>
        public IXLangToken GetNext()
        {
            XLangParsingTools.ReadAnyOrNone(Tokens, currentIdx, out IXLangToken result);
            currentIdx++;
            return result;
        }
    }
}
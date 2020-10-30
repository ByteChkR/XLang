using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Implements a Class Definition Token
    /// </summary>
    public class ClassDefinitionToken : CombinedToken
    {
        /// <summary>
        ///     The Base class of this Class
        /// </summary>
        public readonly IXLangToken BaseClass;
        /// <summary>
        ///     The Class key that was detected
        /// </summary>
        public readonly IXLangToken ClassKey;
        /// <summary>
        ///     Access Modifiers
        /// </summary>
        public readonly IXLangToken[] Modifiers;
        /// <summary>
        ///     Token Name
        /// </summary>
        public readonly IXLangToken Name;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="classKey">Class Key</param>
        /// <param name="name">Class Name</param>
        /// <param name="baseClass">Base Class</param>
        /// <param name="modifiers">Class Modifiers</param>
        /// <param name="subtokens">Child Tokens</param>
        public ClassDefinitionToken(
            IXLangToken classKey, IXLangToken name, IXLangToken baseClass, IXLangToken[] modifiers,
            IXLangToken[] subtokens) : base(
            XLangTokenType
                .OpClassDefinition,
            subtokens,
            classKey
                .SourceIndex
        )
        {
            BaseClass = baseClass;
            Name = name;
            Modifiers = modifiers;
            ClassKey = classKey;
        }

        /// <summary>
        ///     String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Unpack(Modifiers)} {ClassKey.GetValue()} {Name.GetValue()}";
        }
    }
}
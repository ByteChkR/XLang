using System.Collections.Generic;
using XLang.Core;
using XLang.Parser.Token;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;

namespace XLang.Parser.Runtime
{
    /// <summary>
    ///     Implements a Member Definition Token
    /// </summary>
    public abstract class XLangMemberDefinitionToken : IXLangToken
    {
        /// <summary>
        ///     The Binding Flags of this Member
        /// </summary>
        public readonly XLangMemberFlags BindingFlags;

        /// <summary>
        ///     The Type of member.
        /// </summary>
        public readonly XLangMemberType MemberType;

        /// <summary>
        ///     The Member Name
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="startIndex">Start index in the source</param>
        /// <param name="name">Member Name</param>
        /// <param name="type">Member Type</param>
        /// <param name="bindingFlags">Binding Flags</param>
        protected XLangMemberDefinitionToken(
            int startIndex, string name, XLangMemberType type, XLangMemberFlags bindingFlags)
        {
            MemberType = type;
            BindingFlags = bindingFlags;
            StartIndex = startIndex;
            Name = name;
        }

        /// <summary>
        ///     Returns all Child Tokens
        /// </summary>
        /// <returns></returns>
        public List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        /// <summary>
        ///     Returns the Token Value
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            return ToString();
        }

        /// <summary>
        ///     The Token Type
        /// </summary>
        public XLangTokenType Type => XLangTokenType.OpRuntimeMember;

        /// <summary>
        ///     The Start Index in the source.
        /// </summary>
        public int StartIndex { get; }
    }
}
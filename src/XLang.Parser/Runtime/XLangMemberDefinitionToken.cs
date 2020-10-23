using System.Collections.Generic;
using XLang.Core;
using XLang.Parser.Token;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;

namespace XLang.Parser.Runtime
{
    public abstract class XLangMemberDefinitionToken : IXLangToken
    {
        public readonly XLangMemberFlags BindingFlags;
        public readonly XLangMemberType MemberType;

        public readonly string Name;

        protected XLangMemberDefinitionToken(
            int startIndex, string name, XLangMemberType type, XLangMemberFlags bindingFlags)
        {
            MemberType = type;
            BindingFlags = bindingFlags;
            StartIndex = startIndex;
            Name = name;
        }

        public List<IXLangToken> GetChildren()
        {
            return new List<IXLangToken>();
        }

        public string GetValue()
        {
            return ToString();
        }

        public XLangTokenType Type => XLangTokenType.OpRuntimeMember;

        public int StartIndex { get; }
    }
}
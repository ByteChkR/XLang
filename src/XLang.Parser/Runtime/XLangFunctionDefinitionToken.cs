using XLang.Parser.Token.Expressions;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Types;

namespace XLang.Parser.Runtime
{
    public class XLangFunctionDefinitionToken : XLangMemberDefinitionToken
    {
        public readonly string ReturnType;
        public readonly XLangExpression[] Sequence;

        public readonly XLangRuntimeType TypeDefinition;

        public XLangFunctionDefinitionToken(
            int startIndex, string name, string returnType, XLangRuntimeType typeDef, XLangExpression[] sequence,
            XLangMemberFlags bindingFlags) : base(startIndex, name, XLangMemberType.Function, bindingFlags)
        {
            TypeDefinition = typeDef;
            Sequence = sequence;
            ReturnType = returnType;
        }

        public string Fullname => $"{TypeDefinition.FullName}.{Name}";
    }
}
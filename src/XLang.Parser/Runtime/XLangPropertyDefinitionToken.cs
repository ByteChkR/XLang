using XLang.Parser.Token.Expressions.Operands;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Types;

namespace XLang.Parser.Runtime
{
    public class XLangPropertyDefinitionToken : XLangMemberDefinitionToken
    {
        public readonly XLangRuntimeType TypeDefinition;
        public readonly string TypeName;
        public readonly XLangVarDefOperand VarInitializationExpression;

        public XLangPropertyDefinitionToken(
            int startIndex, string name, string typeName, XLangVarDefOperand initExpr, XLangRuntimeType typeDef,
            XLangMemberFlags bindingFlags) : base(startIndex, name, XLangMemberType.Property, bindingFlags)
        {
            TypeName = typeName;
            TypeDefinition = typeDef;
            VarInitializationExpression = initExpr;
        }

        public string FullName => $"{TypeDefinition.FullName}.{Name}";

        public override string ToString()
        {
            return $"{FullName} of Type '{TypeName}'";
        }
    }
}
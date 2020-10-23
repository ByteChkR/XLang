using System.Text;
using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    public class FunctionDefinitionToken : CombinedToken
    {
        public readonly VariableDefinitionToken[] Arguments;

        public readonly bool IsConstructor;
        public readonly IXLangToken[] Modifiers;
        public readonly IXLangToken Name;
        public readonly IXLangToken TypeName;

        public FunctionDefinitionToken(
            IXLangToken name, IXLangToken typeName, VariableDefinitionToken[] arguments, IXLangToken[] modifiers,
            IXLangToken[] subtokens, bool isConstructor = false) : base(
            XLangTokenType.OpFunctionDefinition,
            subtokens,
            name.StartIndex
        )
        {
            Modifiers = modifiers;
            Name = name;
            TypeName = typeName;
            Arguments = arguments;
            IsConstructor = isConstructor;
        }

        public override string ToString()
        {
            return $"{Unpack(Modifiers)} {TypeName.GetValue()} {Name.GetValue()} {UnpackArgs()}";
        }

        private string UnpackArgs()
        {
            StringBuilder sb = new StringBuilder("(");
            for (int i = 0; i < Arguments.Length; i++)
            {
                VariableDefinitionToken token = Arguments[i];
                sb.Append($"type:{token.TypeName.GetValue()} name:{token.Name}");
                if (i != Arguments.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
            return sb.ToString();
        }
    }
}
using System.Text;
using XLang.Core;

namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Implements a Function Definition Token
    /// </summary>
    public class FunctionDefinitionToken : CombinedToken
    {
        /// <summary>
        ///     Function Arguments
        /// </summary>
        public readonly VariableDefinitionToken[] Arguments;

        /// <summary>
        ///     Flag that indicates if this function is a constructor of the implementing class
        /// </summary>
        public readonly bool IsConstructor;
        /// <summary>
        ///     Function Modifiers
        /// </summary>
        public readonly IXLangToken[] Modifiers;
        /// <summary>
        ///     Function Name
        /// </summary>
        public readonly IXLangToken Name;
        /// <summary>
        ///     The Type Name of the Return Value
        /// </summary>
        public readonly IXLangToken TypeName;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Function Name</param>
        /// <param name="typeName">Return Type</param>
        /// <param name="arguments">Function Arguments</param>
        /// <param name="modifiers">Function Modifiers</param>
        /// <param name="subtokens">Child Tokens</param>
        /// <param name="isConstructor">Is Constructor Flag</param>
        public FunctionDefinitionToken(
            IXLangToken name, IXLangToken typeName, VariableDefinitionToken[] arguments, IXLangToken[] modifiers,
            IXLangToken[] subtokens, bool isConstructor = false) : base(
            XLangTokenType.OpFunctionDefinition,
            subtokens,
            name.SourceIndex
        )
        {
            Modifiers = modifiers;
            Name = name;
            TypeName = typeName;
            Arguments = arguments;
            IsConstructor = isConstructor;
        }

        /// <summary>
        ///     String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Unpack(Modifiers)} {TypeName.GetValue()} {Name.GetValue()} {UnpackArgs()}";
        }

        /// <summary>
        ///     Helper Function that Unpacks the Function Arguments into a string representation
        /// </summary>
        /// <returns></returns>
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
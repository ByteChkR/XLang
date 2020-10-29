using XLang.Core;
using XLang.Parser.Token.Expressions;

namespace XLang.Parser.Token.Combined
{
    /// <summary>
    ///     Implements a Variable(Property) Definition Token
    /// </summary>
    public class VariableDefinitionToken : CombinedToken
    {
        /// <summary>
        ///     Initializer Expression
        /// </summary>
        public readonly XLangExpression InitializerExpression;

        /// <summary>
        ///     Variable Modifiers
        /// </summary>
        public readonly IXLangToken[] Modifiers;
        /// <summary>
        ///     Variable Name
        /// </summary>
        public readonly IXLangToken Name;
        /// <summary>
        ///     Variable Type Name
        /// </summary>
        public readonly IXLangToken TypeName;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="typeName">Variable Type Name</param>
        /// <param name="modifiers">Variable Modifiers</param>
        /// <param name="subtokens">Child Tokens</param>
        /// <param name="initializerExpression">Initializer Expression</param>
        public VariableDefinitionToken(
            IXLangToken name, IXLangToken typeName, IXLangToken[] modifiers, IXLangToken[] subtokens,
            XLangExpression initializerExpression) : base(
            XLangTokenType.OpVariableDefinition,
            subtokens,
            typeName.StartIndex
        )
        {
            Modifiers = modifiers;
            Name = name;
            TypeName = typeName;
            InitializerExpression = initializerExpression;
        }

        /// <summary>
        ///     String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Unpack(Modifiers)} {TypeName.GetValue()} {Name.GetValue()}";
        }
    }
}
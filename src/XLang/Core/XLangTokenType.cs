/// <summary>
/// Contains Core Logic and Enums
/// </summary>
namespace XLang.Core
{
    /// <summary>
    ///     Token Type Enum containing all tokens used inside XL
    /// </summary>
    public enum XLangTokenType
    {
        Any,
        Unknown,
        OpNone,
        OpNewLine,
        OpPlus,
        OpMinus,
        OpAsterisk,
        OpFwdSlash,
        OpBackSlash,
        OpPercent,
        OpSpace,
        OpNumber,
        OpWord,
        OpBracketOpen,
        OpBracketClose,
        OpBlockBracketOpen,
        OpBlockBracketClose,
        OpIndexerBracketOpen,
        OpIndexerBracketClose,

        OpSemicolon,
        OpComma,
        OpColon,
        OpDot,
        OpEquality,
        OpAnd,
        OpLogicalAnd,
        OpPipe,
        OpLogicalOr,
        OpCap,
        OpBang,
        OpLessThan,
        OpGreaterThan,
        OpLessOrEqual,
        OpGreaterOrEqual,
        OpSingleQuote,
        OpDoubleQuote,
        OpTilde,
        OpComparison,

        //Reserved Key Values
        OpIf,
        OpElse,
        OpForEach,
        OpFor,
        OpIn,
        OpIs,
        OpAs,
        OpDo,
        OpWhile,
        OpSwitch,
        OpTry,
        OpCatch,
        OpFinally,
        OpUsing,
        OpNamespace,
        OpClass,
        OpContinue,
        OpBreak,
        OpNew,
        OpBase,
        OpThis,
        OpPublicMod,
        OpPrivateMod,
        OpProtectedMod,
        OpVirtualMod,
        OpAbstractMod,
        OpOverrideMod,
        OpStaticMod,

        OpComment,

        OpNamespaceDefinition,
        OpClassDefinition,
        OpVariableDefinition,
        OpFunctionDefinition,
        OpExpression,
        OpStringLiteral,
        OpStatement,
        OpTypeVoid,
        OpBlockToken,
        OpReturn,
        OpInvocation,
        OpArrayAccess,
        OpRuntimeNamespace,
        OpOperatorImpl,
        OpRuntimeMember,


        //End Of File
        EOF
    }
}
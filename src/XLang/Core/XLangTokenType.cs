﻿namespace XLang.Core
{
    public enum XLangTokenType
    {

        Any,
        OpNone,
        OpNewLine,
        OpPlus,
        OpMinus,
        OpAsterisk,
        OpFwdSlash,
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

        OpRuntimeNamespace,
        OpOperatorImpl,
        OpRuntimeMember,


        //End Of File
        EOF

    }
}
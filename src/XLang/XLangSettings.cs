using System.Collections.Generic;
using System.Linq;
using XLang.Core;

/// <summary>
/// Contains the XLang Language Parser/Runtime
/// </summary>
namespace XLang
{
    /// <summary>
    ///     XL Parser Settings and Symbol Mappings
    /// </summary>
    public class XLangSettings
    {
        #region Keys

        /// <summary>
        ///     Reserved Key Map
        /// </summary>
        public Dictionary<string, XLangTokenType> ReservedKeys =>
            new Dictionary<string, XLangTokenType>
            {
                {IfKey, XLangTokenType.OpIf},
                {ElseKey, XLangTokenType.OpElse},
                {ForEachKey, XLangTokenType.OpForEach},
                {ForKey, XLangTokenType.OpFor},
                {InKey, XLangTokenType.OpIn},
                {IsKey, XLangTokenType.OpIs},
                {AsKey, XLangTokenType.OpAs},
                {DoKey, XLangTokenType.OpDo},
                {WhileKey, XLangTokenType.OpWhile},
                {SwitchKey, XLangTokenType.OpSwitch},
                {TryKey, XLangTokenType.OpTry},
                {CatchKey, XLangTokenType.OpCatch},
                {FinallyKey, XLangTokenType.OpFinally},
                {UsingKey, XLangTokenType.OpUsing},
                {NamespaceKey, XLangTokenType.OpNamespace},
                {ClassKey, XLangTokenType.OpClass},
                {ContinueKey, XLangTokenType.OpContinue},
                {BreakKey, XLangTokenType.OpBreak},
                {NewKey, XLangTokenType.OpNew},
                {BaseKey, XLangTokenType.OpBase},
                {ThisKey, XLangTokenType.OpThis},
                {PublicModifier, XLangTokenType.OpPublicMod},
                {PrivateModifier, XLangTokenType.OpPrivateMod},
                {ProtectedModifier, XLangTokenType.OpProtectedMod},
                {VirtualModifier, XLangTokenType.OpVirtualMod},
                {AbstractModifier, XLangTokenType.OpAbstractMod},
                {OverrideModifier, XLangTokenType.OpOverrideMod},
                {StaticModifier, XLangTokenType.OpStaticMod},
                {ReturnKey, XLangTokenType.OpReturn},
                {VoidKey, XLangTokenType.OpTypeVoid},
                {OperatorKey, XLangTokenType.OpOperatorImpl}
            };


        /// <summary>
        ///     Valid Member Modifiers.
        /// </summary>
        public Dictionary<string, XLangTokenType> MemberModifiers =>
            new Dictionary<string, XLangTokenType>
            {
                {PublicModifier, XLangTokenType.OpPublicMod},
                {PrivateModifier, XLangTokenType.OpPrivateMod},
                {ProtectedModifier, XLangTokenType.OpProtectedMod},
                {VirtualModifier, XLangTokenType.OpVirtualMod},
                {AbstractModifier, XLangTokenType.OpAbstractMod},
                {OverrideModifier, XLangTokenType.OpOverrideMod},
                {StaticModifier, XLangTokenType.OpStaticMod},
                {OperatorKey, XLangTokenType.OpOperatorImpl}
            };


        /// <summary>
        ///     Valid Class Modifiers
        /// </summary>
        public Dictionary<string, XLangTokenType> ClassModifiers =>
            new Dictionary<string, XLangTokenType>
            {
                {PublicModifier, XLangTokenType.OpPublicMod},
                {PrivateModifier, XLangTokenType.OpPrivateMod},
                {ProtectedModifier, XLangTokenType.OpProtectedMod},
                {AbstractModifier, XLangTokenType.OpAbstractMod},
                {StaticModifier, XLangTokenType.OpStaticMod}
            };

        #region Keys / Modifiers

        private readonly string PublicModifier = "public";
        private readonly string PrivateModifier = "private";
        private readonly string ProtectedModifier = "protected";
        private readonly string VirtualModifier = "virtual";
        private readonly string AbstractModifier = "abstract";
        private readonly string OverrideModifier = "override";
        private readonly string StaticModifier = "static";
        private readonly string IfKey = "if";
        private readonly string ElseKey = "else";
        private readonly string ForKey = "for";
        private readonly string ForEachKey = "foreach";
        private readonly string InKey = "in";
        private readonly string IsKey = "is";
        private readonly string AsKey = "as";
        private readonly string DoKey = "do";
        private readonly string WhileKey = "while";
        private readonly string SwitchKey = "switch";
        private readonly string TryKey = "try";
        private readonly string CatchKey = "catch";
        private readonly string FinallyKey = "finally";
        private readonly string UsingKey = "using";
        private readonly string NamespaceKey = "namespace";
        private readonly string ClassKey = "class";
        private readonly string ContinueKey = "continue";
        private readonly string BreakKey = "break";
        private readonly string NewKey = "new";
        private readonly string BaseKey = "base";
        private readonly string ThisKey = "this";
        private readonly string VoidKey = "void";
        private readonly string ReturnKey = "return";
        private readonly string OperatorKey = "operator";

        #endregion

        #endregion

        #region Symbols

        private readonly char OperatorDoubleQuote = '"';
        private readonly char OperatorSingleQuote = '\'';
        private readonly char OperatorBlockOpen = '{';
        private readonly char OperatorBlockClose = '}';
        private readonly char OperatorBracketsOpen = '(';
        private readonly char OperatorBracketsClose = ')';
        private readonly char OperatorIndexAccessorOpen = '[';
        private readonly char OperatorIndexAccessorClose = ']';
        private readonly char OperatorAsterisk = '*';
        private readonly char OperatorFwdSlash = '/';
        private readonly char OperatorBackSlash = '\\';
        private readonly char OperatorSemicolon = ';';
        private readonly char OperatorComma = ',';
        private readonly char OperatorColon = ':';
        private readonly char OperatorDot = '.';
        private readonly char OperatorPlus = '+';
        private readonly char OperatorMinus = '-';
        private readonly char OperatorPercent = '%';
        private readonly char OperatorEquality = '=';
        private readonly char OperatorAnd = '&';
        private readonly char OperatorPipe = '|';
        private readonly char OperatorCap = '^';
        private readonly char OperatorBang = '!';
        private readonly char OperatorLessThan = '<';
        private readonly char OperatorGreaterThan = '>';
        private readonly char OperatorTilde = '~';


        /// <summary>
        ///     Reverse Reserved Symbols (XLangToken - char)
        /// </summary>
        public Dictionary<XLangTokenType, char> ReverseReservedSymbols =>
            ReservedSymbols.ToDictionary(pair => pair.Value, pair => pair.Key);

        /// <summary>
        ///     Reserved Symbols (char -  XLangToken)
        /// </summary>
        public Dictionary<char, XLangTokenType> ReservedSymbols =>
            new Dictionary<char, XLangTokenType>
            {
                {OperatorBackSlash, XLangTokenType.OpBackSlash},
                {OperatorSingleQuote, XLangTokenType.OpSingleQuote},
                {OperatorDoubleQuote, XLangTokenType.OpDoubleQuote},
                {OperatorBlockOpen, XLangTokenType.OpBlockBracketOpen},
                {OperatorBlockClose, XLangTokenType.OpBlockBracketClose},
                {OperatorBracketsOpen, XLangTokenType.OpBracketOpen},
                {OperatorBracketsClose, XLangTokenType.OpBracketClose},
                {OperatorIndexAccessorOpen, XLangTokenType.OpIndexerBracketOpen},
                {OperatorIndexAccessorClose, XLangTokenType.OpIndexerBracketClose},
                {OperatorAsterisk, XLangTokenType.OpAsterisk},
                {OperatorFwdSlash, XLangTokenType.OpFwdSlash},
                {OperatorSemicolon, XLangTokenType.OpSemicolon},
                {OperatorComma, XLangTokenType.OpComma},
                {OperatorColon, XLangTokenType.OpColon},
                {OperatorDot, XLangTokenType.OpDot},
                {OperatorPlus, XLangTokenType.OpPlus},
                {OperatorMinus, XLangTokenType.OpMinus},
                {OperatorPercent, XLangTokenType.OpPercent},
                {OperatorEquality, XLangTokenType.OpEquality},
                {OperatorAnd, XLangTokenType.OpAnd},
                {OperatorPipe, XLangTokenType.OpPipe},
                {OperatorCap, XLangTokenType.OpCap},
                {OperatorBang, XLangTokenType.OpBang},
                {OperatorLessThan, XLangTokenType.OpLessThan},
                {OperatorGreaterThan, XLangTokenType.OpGreaterThan},
                {OperatorTilde, XLangTokenType.OpTilde}
            };

        #endregion
    }
}
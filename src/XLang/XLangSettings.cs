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

        private string PublicModifier = "public";
        private string PrivateModifier = "private";
        private string ProtectedModifier = "protected";
        private string VirtualModifier = "virtual";
        private string AbstractModifier = "abstract";
        private string OverrideModifier = "override";
        private string StaticModifier = "static";
        private string IfKey = "if";
        private string ElseKey = "else";
        private string ForKey = "for";
        private string ForEachKey = "foreach";
        private string InKey = "in";
        private string IsKey = "is";
        private string AsKey = "as";
        private string DoKey = "do";
        private string WhileKey = "while";
        private string SwitchKey = "switch";
        private string TryKey = "try";
        private string CatchKey = "catch";
        private string FinallyKey = "finally";
        private string UsingKey = "using";
        private string NamespaceKey = "namespace";
        private string ClassKey = "class";
        private string ContinueKey = "continue";
        private string BreakKey = "break";
        private string NewKey = "new";
        private string BaseKey = "base";
        private string ThisKey = "this";
        private string VoidKey = "void";
        private string ReturnKey = "return";
        private string OperatorKey = "operator";

        #endregion

        #endregion

        #region Symbols

        private char OperatorDoubleQuote = '"';
        private char OperatorSingleQuote = '\'';
        private char OperatorBlockOpen = '{';
        private char OperatorBlockClose = '}';
        private char OperatorBracketsOpen = '(';
        private char OperatorBracketsClose = ')';
        private char OperatorIndexAccessorOpen = '[';
        private char OperatorIndexAccessorClose = ']';
        private char OperatorAsterisk = '*';
        private char OperatorFwdSlash = '/';
        private char OperatorBackSlash = '\\';
        private char OperatorSemicolon = ';';
        private char OperatorComma = ',';
        private char OperatorColon = ':';
        private char OperatorDot = '.';
        private char OperatorPlus = '+';
        private char OperatorMinus = '-';
        private char OperatorPercent = '%';
        private char OperatorEquality = '=';
        private char OperatorAnd = '&';
        private char OperatorPipe = '|';
        private char OperatorCap = '^';
        private char OperatorBang = '!';
        private char OperatorLessThan = '<';
        private char OperatorGreaterThan = '>';
        private char OperatorTilde = '~';


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
                {OperatorBackSlash, XLangTokenType.OpBackSlash },
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
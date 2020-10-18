using System.Collections.Generic;
using System.Linq;

using XLang.Core;

namespace XLang
{
    public class XLangSettings
    {

        #region Keys

        public Dictionary<string, XLangTokenType> ReservedKeys =>
            new Dictionary<string, XLangTokenType>
            {
                { IfKey, XLangTokenType.OpIf },
                { ElseKey, XLangTokenType.OpElse },
                { ForEachKey, XLangTokenType.OpForEach },
                { ForKey, XLangTokenType.OpFor },
                { InKey, XLangTokenType.OpIn },
                { IsKey, XLangTokenType.OpIs },
                { AsKey, XLangTokenType.OpAs },
                { DoKey, XLangTokenType.OpDo },
                { WhileKey, XLangTokenType.OpWhile },
                { SwitchKey, XLangTokenType.OpSwitch },
                { TryKey, XLangTokenType.OpTry },
                { CatchKey, XLangTokenType.OpCatch },
                { FinallyKey, XLangTokenType.OpFinally },
                { UsingKey, XLangTokenType.OpUsing },
                { NamespaceKey, XLangTokenType.OpNamespace },
                { ClassKey, XLangTokenType.OpClass },
                { ContinueKey, XLangTokenType.OpContinue },
                { BreakKey, XLangTokenType.OpBreak },
                { NewKey, XLangTokenType.OpNew },
                { BaseKey, XLangTokenType.OpBase },
                { ThisKey, XLangTokenType.OpThis },
                { PublicModifier, XLangTokenType.OpPublicMod },
                { PrivateModifier, XLangTokenType.OpPrivateMod },
                { ProtectedModifier, XLangTokenType.OpProtectedMod },
                { VirtualModifier, XLangTokenType.OpVirtualMod },
                { AbstractModifier, XLangTokenType.OpAbstractMod },
                { OverrideModifier, XLangTokenType.OpOverrideMod },
                { StaticModifier, XLangTokenType.OpStaticMod },
                { ReturnKey, XLangTokenType.OpReturn },
                { VoidKey, XLangTokenType.OpTypeVoid },
                { OperatorKey, XLangTokenType.OpOperatorImpl }
            };


        public Dictionary<string, XLangTokenType> MemberModifiers =>
            new Dictionary<string, XLangTokenType>
            {
                { PublicModifier, XLangTokenType.OpPublicMod },
                { PrivateModifier, XLangTokenType.OpPrivateMod },
                { ProtectedModifier, XLangTokenType.OpProtectedMod },
                { VirtualModifier, XLangTokenType.OpVirtualMod },
                { AbstractModifier, XLangTokenType.OpAbstractMod },
                { OverrideModifier, XLangTokenType.OpOverrideMod },
                { StaticModifier, XLangTokenType.OpStaticMod },
                { OperatorKey, XLangTokenType.OpOperatorImpl }
            };


        public Dictionary<string, XLangTokenType> ClassModifiers =>
            new Dictionary<string, XLangTokenType>
            {
                { PublicModifier, XLangTokenType.OpPublicMod },
                { PrivateModifier, XLangTokenType.OpPrivateMod },
                { ProtectedModifier, XLangTokenType.OpProtectedMod },
                { AbstractModifier, XLangTokenType.OpAbstractMod },
                { StaticModifier, XLangTokenType.OpStaticMod }
            };

        public string PublicModifier = "public";
        public string PrivateModifier = "private";
        public string ProtectedModifier = "protected";
        public string VirtualModifier = "virtual";
        public string AbstractModifier = "abstract";
        public string OverrideModifier = "override";
        public string StaticModifier = "static";
        public string IfKey = "if";
        public string ElseKey = "else";
        public string ForKey = "for";
        public string ForEachKey = "foreach";
        public string InKey = "in";
        public string IsKey = "is";
        public string AsKey = "as";
        public string DoKey = "do";
        public string WhileKey = "while";
        public string SwitchKey = "switch";
        public string TryKey = "try";
        public string CatchKey = "catch";
        public string FinallyKey = "finally";
        public string UsingKey = "using";
        public string NamespaceKey = "namespace";
        public string ClassKey = "class";
        public string ContinueKey = "continue";
        public string BreakKey = "break";
        public string NewKey = "new";
        public string BaseKey = "base";
        public string ThisKey = "this";
        public string VoidKey = "void";
        public string ReturnKey = "return";
        public string OperatorKey = "operator";

        #endregion

        #region Symbols

        //public char OperatorNewLine = '\n';
        //public char OperatorCRNewLine = '\r';
        public char OperatorDoubleQuote = '"';
        public char OperatorSingleQuote = '\'';
        public char OperatorBlockOpen = '{';
        public char OperatorBlockClose = '}';
        public char OperatorBracketsOpen = '(';
        public char OperatorBracketsClose = ')';
        public char OperatorIndexAccessorOpen = '[';
        public char OperatorIndexAccessorClose = ']';
        public char OperatorAsterisk = '*';
        public char OperatorFwdSlash = '/';
        public char OperatorSemicolon = ';';
        public char OperatorComma = ',';
        public char OperatorColon = ':';
        public char OperatorDot = '.';
        public char OperatorPlus = '+';
        public char OperatorMinus = '-';
        public char OperatorPercent = '%';
        public char OperatorEquality = '=';
        public char OperatorAnd = '&';
        public char OperatorPipe = '|';
        public char OperatorCap = '^';
        public char OperatorBang = '!';
        public char OperatorLessThan = '<';
        public char OperatorGreaterThan = '>';
        public char OperatorTilde = '~';

        //public XLangTokenType[] WhiteSpaceTypes => new []{XLangTokenType.OpSpace, XLangTokenType.OpNewLine, XLangTokenType.OpCRNewLine};

        public Dictionary<XLangTokenType, char> ReverseReservedSymbols =>
            ReservedSymbols.ToDictionary(pair => pair.Value, pair => pair.Key);

        public Dictionary<char, XLangTokenType> ReservedSymbols =>
            new Dictionary<char, XLangTokenType>
            {
                //{OperatorCRNewLine, XLangTokenType.OpCRNewLine },
                //{OperatorNewLine, XLangTokenType.OpNewLine },
                { OperatorSingleQuote, XLangTokenType.OpSingleQuote },
                { OperatorDoubleQuote, XLangTokenType.OpDoubleQuote },
                { OperatorBlockOpen, XLangTokenType.OpBlockBracketOpen },
                { OperatorBlockClose, XLangTokenType.OpBlockBracketClose },
                { OperatorBracketsOpen, XLangTokenType.OpBracketOpen },
                { OperatorBracketsClose, XLangTokenType.OpBracketClose },
                { OperatorIndexAccessorOpen, XLangTokenType.OpIndexerBracketOpen },
                { OperatorIndexAccessorClose, XLangTokenType.OpIndexerBracketClose },
                { OperatorAsterisk, XLangTokenType.OpAsterisk },
                { OperatorFwdSlash, XLangTokenType.OpFwdSlash },
                { OperatorSemicolon, XLangTokenType.OpSemicolon },
                { OperatorComma, XLangTokenType.OpComma },
                { OperatorColon, XLangTokenType.OpColon },
                { OperatorDot, XLangTokenType.OpDot },
                { OperatorPlus, XLangTokenType.OpPlus },
                { OperatorMinus, XLangTokenType.OpMinus },
                { OperatorPercent, XLangTokenType.OpPercent },
                { OperatorEquality, XLangTokenType.OpEquality },
                { OperatorAnd, XLangTokenType.OpAnd },
                { OperatorPipe, XLangTokenType.OpPipe },
                { OperatorCap, XLangTokenType.OpCap },
                { OperatorBang, XLangTokenType.OpBang },
                { OperatorLessThan, XLangTokenType.OpLessThan },
                { OperatorGreaterThan, XLangTokenType.OpGreaterThan },
                { OperatorTilde, XLangTokenType.OpTilde }
            };

        #endregion

    }
}
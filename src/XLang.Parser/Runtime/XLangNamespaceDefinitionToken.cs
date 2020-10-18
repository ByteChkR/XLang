//using System.Collections.Generic;
//using System.Linq;

//using XLang.Core;
//using XLang.Parser.Tokens;
//using XLang.Parser.Tokens.RuntimeToken;

//namespace XLang.Parser.Tokens.RuntimeToken
//{
//    public class XLangNamespaceDefinitionToken : IXLangToken
//    {

//        public XLangNamespaceDefinitionToken Parent;
//        public readonly string Name;
//        public IReadOnlyList<XLangNamespaceDefinitionToken> Children => children.AsReadOnly();
//        public IReadOnlyList<XLangTypeDefinitionToken> DefinedTypes => types.AsReadOnly();
//        public string FullName =>
//            Parent == null ? Name : Parent.Name + settings.ReverseReservedSymbols[XLangTokenType.OpDot] + Name;

//        public List<IXLangToken> GetChildren()=> new List<IXLangToken>();

//        public string GetValue() => ToString();

//        public XLangTokenType Type => XLangTokenType.OpRuntimeNamespace;
//        public int StartIndex { get; }


//        private readonly List<XLangNamespaceDefinitionToken> children = new List<XLangNamespaceDefinitionToken>();
//        private readonly List<XLangTypeDefinitionToken> types = new List<XLangTypeDefinitionToken>();
//        private readonly XLangSettings settings;
//        private readonly XLangParserResultCollection collection;


//        public XLangNamespaceDefinitionToken(XLangParserResultCollection collection, XLangSettings settings, string name, int startIndex, XLangNamespaceDefinitionToken parent)
//        {
//            this.settings = settings;
//            this.collection = collection;
//            StartIndex = startIndex;
//            Name = name;
//            Parent = parent;
//            Parent?.children.Add(this);
//        }


//        public void AddType(XLangTypeDefinitionToken typeDef)
//        {
//            if(DefinedTypes.Any(x=>x.Name == typeDef.Name))throw new XLangNameAmbiguityException($"A Type with the name '{typeDef.Name}' already exists in namespace '{FullName}', source index: {typeDef.StartIndex}.");
//        }


//    }
//}


namespace XLang.Parser.Runtime
{
    //public class XLangTypeDefinitionToken : IXLangToken
    //{

    //    public readonly XLangTypeDefinitionToken BaseType;

    //    public readonly XLangRuntimeNamespace Namespace;

    //    public List<IXLangToken> GetChildren()=> new List<IXLangToken>();

    //    public string GetValue() => ToString();

    //    public XLangTokenType Type => XLangTokenType.OpRuntimeType;

    //    public int StartIndex { get; }

    //    public readonly string Name;

    //    public string Fullname => $"{Namespace.FullName}.{Name}";

    //    public readonly XLangBindingFlags BindingFlags;

    //    private readonly List<XLangMemberDefinitionToken> members;

    //    public XLangTypeDefinitionToken(XLangBindingFlags bindingFlags,
    //        int startIndex, string name, XLangTypeDefinitionToken baseType, XLangRuntimeNamespace nameSpace, XLangParserResultCollection nsCollection, XLangSettings settings, List<IXLangToken> content)
    //    {
    //        BindingFlags = bindingFlags;
    //        BaseType = baseType;
    //        StartIndex = startIndex;
    //        Name = name;
    //        Namespace = nameSpace;
    //        Namespace.AddType(this);
    //        members = XLangTokenElevation.ElevateMembers(content, this, settings, nameSpace, nsCollection);
    //    }

    //    public List<XLangMemberDefinitionToken> GetAllMembers() => members;

    //}
}
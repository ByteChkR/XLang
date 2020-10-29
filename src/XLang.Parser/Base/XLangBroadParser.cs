using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLang.Core;
using XLang.Exceptions;
using XLang.Parser.Exceptions;
using XLang.Parser.Expressions;
using XLang.Parser.Reader;
using XLang.Parser.Runtime;
using XLang.Parser.Shared;
using XLang.Parser.Token;
using XLang.Parser.Token.BaseTokens;
using XLang.Parser.Token.Combined;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operands;
using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Types;
using XLang.Shared;

/// <summary>
/// Contains the Board Phase Parser that prepares the source for the Expression Parser
/// </summary>
namespace XLang.Parser.Base
{
    /// <summary>
    ///     Broad Phase Parser implementing Tokenization on Context level
    /// </summary>
    public static class XLangBroadParser
    {
        /// <summary>
        ///     Removes all One line Comment Token Sequences
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateOneLineComment(XLangSettings settings, List<IXLangToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i < tokens.Count - 1 &&
                    tokens[i].Type == XLangTokenType.OpFwdSlash &&
                    tokens[i + 1].Type == XLangTokenType.OpFwdSlash)
                {
                    int idx = tokens.FindIndex(i + 2, t => t.Type == XLangTokenType.OpNewLine);
                    if (idx == -1)
                    {
                        idx = tokens.Count - 1;
                    }

                    tokens.RemoveRange(i, idx - i);
                }
            }
        }

        /// <summary>
        ///     Replaces all One Line Strings with a StringLiteral Token
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateOneLineString(XLangSettings settings, List<IXLangToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i < tokens.Count - 1 &&
                    tokens[i].Type == XLangTokenType.OpDoubleQuote)
                {
                    int idx = tokens.FindIndex(i + 1, t => t.Type == XLangTokenType.OpNewLine);
                    int endQuote = tokens.FindIndex(i + 1, t => t.Type == XLangTokenType.OpDoubleQuote);
                    if (idx == -1)
                    {
                        idx = tokens.Count - 1;
                    }

                    if (endQuote == -1 || endQuote > idx)
                    {
                        throw new XLangTokenReadException(
                            tokens,
                            XLangTokenType.OpDoubleQuote,
                            XLangTokenType.OpNewLine,
                            tokens[i].StartIndex
                        );
                    }

                    List<IXLangToken> content = tokens.GetRange(i + 1, endQuote - i - 1);

                    string ConcatContent()
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (IXLangToken token in content)
                        {
                            sb.Append(token.GetValue());
                        }

                        return sb.ToString();
                    }

                    IXLangToken newToken = new TextToken(
                        XLangTokenType.OpStringLiteral,
                        ConcatContent(),
                        tokens[i].StartIndex
                    );
                    tokens.RemoveRange(i, endQuote - i + 1);
                    tokens.Insert(i, newToken);
                }
            }
        }

        /// <summary>
        ///     Replaces all Reserved Keys with the Correct Token
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateReservedKeys(XLangSettings settings, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                IXLangToken token = tokens[i];
                if (token.Type == XLangTokenType.OpWord && settings.ReservedKeys.ContainsKey(token.GetValue()))
                {
                    tokens[i] = new TextToken(
                        settings.ReservedKeys[token.GetValue()],
                        token.GetValue(),
                        token.StartIndex
                    );
                }
            }
        }


        /// <summary>
        ///     Replaces all Namespace Tokens and following Block tokens with a Namespace Definition Token
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateNamespace(XLangSettings settings, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == XLangTokenType.OpBlockToken)
                {
                    ElevateNamespace(settings, tokens[i].GetChildren());
                }

                if (tokens[i].Type == XLangTokenType.OpNamespace)
                {
                    IXLangToken namespaceKey = tokens[i];
                    IXLangToken name = XLangParsingTools.ReadOne(tokens, i + 1, XLangTokenType.OpWord);
                    IXLangToken block = XLangParsingTools.ReadOne(tokens, i + 2, XLangTokenType.OpBlockToken);

                    tokens.RemoveRange(i, 3);


                    tokens.Insert(i, new NamespaceDefinitionToken(namespaceKey, name, block.GetChildren().ToArray()));
                }
            }
        }


        /// <summary>
        ///     Replaces all Class Keys and Surrounding Modifiers with a Class Definition Token
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateClass(XLangSettings settings, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == XLangTokenType.OpBlockToken ||
                    tokens[i].Type == XLangTokenType.OpNamespaceDefinition)
                {
                    ElevateClass(settings, tokens[i].GetChildren());
                }

                if (tokens[i].Type == XLangTokenType.OpClass)
                {
                    IXLangToken classKey = tokens[i];

                    IXLangToken name = XLangParsingTools.ReadOne(tokens, i + 1, XLangTokenType.OpWord);

                    IXLangToken[] mods = XLangParsingTools.ReadNoneOrManyOf(
                        tokens,
                        i - 1,
                        -1,
                        settings.ClassModifiers.Values.ToArray()
                    ).Reverse().ToArray();


                    IXLangToken baseClass = null;
                    int offset = 2;
                    if (XLangParsingTools.ReadOneOrNone(
                        tokens,
                        i + offset,
                        XLangTokenType.OpColon,
                        out IXLangToken inhColon
                    ))
                    {
                        baseClass = XLangParsingTools.ReadOne(tokens, i + offset + 1, XLangTokenType.OpWord);
                        offset += 2;
                    }
                    IXLangToken block = XLangParsingTools.ReadOne(
                        tokens,
                        i + offset,
                        XLangTokenType.OpBlockToken
                    );


                    int start = i - mods.Length;
                    int end = i + offset + 1;
                    tokens.RemoveRange(start, end - start);

                    tokens.Insert(
                        start,
                        new ClassDefinitionToken(
                            classKey,
                            name,
                            baseClass,
                            mods,
                            block.GetChildren().ToArray()
                        )
                    );
                    i = start;
                }
            }
        }

        /// <summary>
        ///     Replaces all Sequences that end with OpSemicolon (;) with a Statement Token
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateStatements(XLangSettings settings, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == XLangTokenType.OpClassDefinition ||
                    tokens[i].Type == XLangTokenType.OpNamespaceDefinition)
                {
                    ElevateStatements(settings, tokens[i].GetChildren());
                }

                if (tokens[i].Type == XLangTokenType.OpSemicolon)
                {
                    int end = i;
                    List<IXLangToken> found = XLangParsingTools.ReadUntilAny(
                        tokens,
                        i - 1,
                        -1,
                        new[]
                        {
                            XLangTokenType.OpSemicolon, XLangTokenType.OpNamespaceDefinition,
                            XLangTokenType.OpClassDefinition
                        }
                    ).Reverse().ToList();

                    if (found.FirstOrDefault()?.Type == XLangTokenType.OpUsing)
                    {
                        continue;
                    }
                    int saveStart = tokens[i].StartIndex;
                    int start = end - found.Count;
                    tokens.RemoveRange(start, found.Count + 1);
                    found.Where(x => x.Type == XLangTokenType.OpBlockToken).ToList()
                        .ForEach(x => ElevateStatements(settings, x.GetChildren()));
                    tokens.Insert(start, new StatementToken(saveStart, found.ToArray()));
                    i = start;
                }
            }
        }


        /// <summary>
        ///     Replaces all Block Brackets(and the content inbetween) with a Block Token
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateBlocks(XLangSettings settings, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == XLangTokenType.OpBlockBracketClose)
                {
                    int current = 1;
                    int start = i - 1;
                    for (; start >= 0; start--)
                    {
                        if (tokens[start].Type == XLangTokenType.OpBlockBracketClose)
                        {
                            current++;
                        }
                        else if (tokens[start].Type == XLangTokenType.OpBlockBracketOpen)
                        {
                            current--;
                            if (current == 0)
                            {
                                break;
                            }
                        }
                    }

                    List<IXLangToken> content = tokens.GetRange(start + 1, i - start - 1).ToList();
                    tokens.RemoveRange(start, i - start + 1);
                    ElevateBlocks(settings, content);
                    tokens.Insert(start, new BlockToken(start, content.ToArray()));
                    i = start;
                }
            }
        }


        /// <summary>
        ///     Replaces all Statement Tokens that are Variable Definitions(All statements in Class Level)
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateTypeDef(XLangContext context, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                List<IXLangToken> children = tokens[i].GetChildren();
                if (tokens[i].Type == XLangTokenType.OpNamespaceDefinition ||
                    tokens[i].Type == XLangTokenType.OpClassDefinition ||
                    tokens[i].Type == XLangTokenType.OpBlockToken)
                {
                    ElevateTypeDef(context, children);
                }

                if (tokens[i].Type == XLangTokenType.OpStatement)
                {
                    List<IXLangToken> content = tokens[i].GetChildren();

                    IXLangToken[] mods = XLangParsingTools.ReadNoneOrManyOf(
                        content,
                        0,
                        1,
                        context.Settings.MemberModifiers.Values
                            .ToArray()
                    );
                    if (!XLangParsingTools.ReadOneOrNone(
                        content,
                        mods.Length,
                        XLangTokenType.OpWord,
                        out IXLangToken typeName
                    ))
                    {
                        continue;
                    }

                    if (!XLangParsingTools.ReadOneOrNone(
                        content,
                        mods.Length + 1,
                        XLangTokenType.OpWord,
                        out IXLangToken propertyName
                    ))
                    {
                        continue;
                    }

                    content.RemoveRange(0, mods.Length + 2);
                    IXLangToken newToken;
                    if (content.FirstOrDefault()?.Type == XLangTokenType.OpEquality)
                    {
                        XLangExpressionParser exParser = XLangExpressionParser.Create(context,
                            new XLangExpressionReader(content.Skip(1).ToList()));
                        newToken = new VariableDefinitionToken(
                            propertyName,
                            typeName,
                            mods,
                            mods.Concat(new[] {typeName, propertyName})
                                .Concat(content).ToArray(),
                            exParser.Parse().First()
                        );
                        content.Clear();
                    }
                    else
                    {
                        newToken = new VariableDefinitionToken(
                            propertyName,
                            typeName,
                            mods,
                            mods.Concat(new[] {typeName, propertyName}).ToArray(),
                            null
                        );
                    }


                    content.Insert(0, newToken);
                }
            }
        }


        /// <summary>
        ///     Replaces all Function Block tokens inside classes with function definitions
        /// </summary>
        /// <param name="settings">XL Settings</param>
        /// <param name="tokens">Token Stream</param>
        /// <param name="classDef">The Containing Class Definition</param>
        public static void ElevateFunctionDef(
            XLangSettings settings, List<IXLangToken> tokens, IXLangToken classDef = null)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == XLangTokenType.OpClassDefinition ||
                    tokens[i].Type == XLangTokenType.OpNamespaceDefinition)
                {
                    ElevateFunctionDef(settings, tokens[i].GetChildren(), tokens[i]);
                }
                else if (tokens[i].Type == XLangTokenType.OpBlockToken)
                {
                    IXLangToken argCloseBracket =
                        XLangParsingTools.ReadOne(tokens, i - 1, XLangTokenType.OpBracketClose);
                    List<IXLangToken> argPart = new List<IXLangToken> {argCloseBracket};
                    IXLangToken[] args = XLangParsingTools.ReadUntil(tokens, i - 2, -1, XLangTokenType.OpBracketOpen);
                    argPart.AddRange(args);
                    IXLangToken argOpenBracket = XLangParsingTools.ReadOne(
                        tokens,
                        i - 2 - args.Length,
                        XLangTokenType.OpBracketOpen
                    );
                    argPart.Add(argOpenBracket);
                    argPart.Reverse();

                    int funcIdx = i - 3 - args.Length;
                    IXLangToken funcName = XLangParsingTools.ReadOne(tokens, funcIdx, XLangTokenType.OpWord);


                    IXLangToken typeName = null;
                    if (funcIdx > 0 &&
                        (tokens[funcIdx - 1].Type == XLangTokenType.OpWord ||
                         tokens[funcIdx - 1].Type == XLangTokenType.OpTypeVoid))
                    {
                        typeName = XLangParsingTools.ReadOneOfAny(
                            tokens,
                            funcIdx - 1,
                            new[]
                            {
                                XLangTokenType.OpWord, XLangTokenType.OpTypeVoid
                            }
                        );
                        int modStart = funcIdx - 1 - 1;
                        IXLangToken[] mods = XLangParsingTools.ReadNoneOrManyOf(
                            tokens,
                            modStart,
                            -1,
                            settings.MemberModifiers.Values
                                .ToArray()
                        ).Reverse().ToArray();
                        int start = modStart - mods.Length + 1;
                        int end = i;
                        IXLangToken block = tokens[i];
                        tokens.RemoveRange(start, end - start + 1);
                        tokens.Insert(
                            start,
                            new FunctionDefinitionToken(
                                funcName,
                                typeName,
                                ParseArgumentList(args.Reverse().ToList()),
                                mods,
                                block.GetChildren().ToArray()
                            )
                        );
                        i = start;
                    }
                    else
                    {
                        if (classDef == null)
                        {
                            throw new Exception("Can not declare a constructor in global namespace");
                        }

                        typeName = funcName;
                        int modStart = funcIdx - 1;
                        IXLangToken[] mods = XLangParsingTools.ReadNoneOrManyOf(
                            tokens,
                            modStart,
                            -1,
                            settings.MemberModifiers.Values
                                .ToArray()
                        ).Reverse().ToArray();
                        int start = modStart - mods.Length + 1;
                        int end = i;
                        IXLangToken block = tokens[i];
                        tokens.RemoveRange(start, end - start + 1);
                        tokens.Insert(
                            start,
                            new FunctionDefinitionToken(
                                funcName,
                                typeName,
                                ParseArgumentList(args.Reverse().ToList()),
                                mods,
                                block.GetChildren().ToArray(),
                                true
                            )
                        );
                        i = start;
                    }
                }
            }
        }

        /// <summary>
        ///     Parses the Argument list of functions
        /// </summary>
        /// <param name="tokens">Argument List</param>
        /// <returns>Parsed Argument List</returns>
        private static VariableDefinitionToken[] ParseArgumentList(List<IXLangToken> tokens)
        {
            XLangExpressionReader reader = new XLangExpressionReader(tokens);

            IXLangToken current = reader.GetNext();
            List<VariableDefinitionToken> ret = new List<VariableDefinitionToken>();

            while (current.Type != XLangTokenType.EOF)
            {
                IXLangToken typeName = current;
                Eat(XLangTokenType.OpWord);
                IXLangToken varName = current;
                Eat(XLangTokenType.OpWord);
                ret.Add(
                    new VariableDefinitionToken(
                        varName,
                        typeName,
                        new IXLangToken[0],
                        new[] {typeName, varName},
                        null
                    )
                );
                if (current.Type == XLangTokenType.EOF)
                {
                    return ret.ToArray();
                }

                Eat(XLangTokenType.OpComma);
            }

            return ret.ToArray();


            void Eat(XLangTokenType type)
            {
                if (current.Type != type)
                {
                    throw new XLangTokenReadException(tokens, type, current.Type, current.StartIndex);
                }

                current = reader.GetNext();
            }
        }


        /// <summary>
        ///     Replaces all Statements that are in function scope with Expressions from the XLExpression Parser
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="tokens">Token Stream</param>
        public static void ElevateExpressions(XLangContext context, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == XLangTokenType.OpClassDefinition ||
                    tokens[i].Type == XLangTokenType.OpNamespaceDefinition ||
                    tokens[i].Type == XLangTokenType.OpBlockToken)
                {
                    ElevateExpressions(context, tokens[i].GetChildren());
                }

                if (tokens[i].Type == XLangTokenType.OpFunctionDefinition)
                {
                    ElevateFunctionExpressions(context, tokens[i].GetChildren());
                }

                if (tokens[i].Type == XLangTokenType.OpStatement)
                {
                    IXLangToken token = tokens[i];
                    XLangExpressionParser parser = XLangExpressionParser.Create(context,
                        new XLangExpressionReader(
                            tokens[i]
                                .GetChildren()
                                .ToList()
                        ));
                    tokens[i] = parser.Parse().First();
                }
            }
        }

        /// <summary>
        ///     Creates Expressions from Function Token Bodies
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="tokens">Token Body</param>
        private static void ElevateFunctionExpressions(XLangContext context, List<IXLangToken> tokens)
        {
            XLangExpressionParser parser = XLangExpressionParser.Create(
                context,
                new XLangExpressionReader(
                    tokens.ToList()
                )
            );
            tokens.Clear();
            XLangExpression[] exprs = parser.Parse();
            tokens.AddRange(exprs);

        }

        /// <summary>
        ///     Turns the Token Stream into a AST/Runtime Tree that can be queried and executed.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="context">XL Context</param>
        /// <returns>List of Tokens</returns>
        public static List<IXLangRuntimeItem> ElevateToRuntimeTokens(
            List<IXLangToken> tokens,
            XLangContext context)
        {
            List<IXLangRuntimeItem> ret = new List<IXLangRuntimeItem>();


            List<(ClassDefinitionToken cDefinition, XLangRuntimeNamespace localNs)> cDefMap =
                CreateCdefMap(tokens, context);


            XLangBroadNameLookup<XLangRuntimeType> GetView(XLangRuntimeNamespace ns)
            {
                XLangBroadNameLookup<XLangRuntimeType> typeNameLookup = new XLangBroadNameLookup<XLangRuntimeType>();
                List<XLangRuntimeNamespace> close = new List<XLangRuntimeNamespace> {ns};
                close.AddRange(
                    context.GetNamespaces().Where(
                        x => context.DefaultImports.Contains(x.FullName) &&
                             !close.Contains(x)
                    )
                );
                foreach (XLangRuntimeNamespace xLangRuntimeNamespace in context.GetNamespaces()
                    .SelectMany(x => x.GetAllNamespacesRecursive()))
                {
                    foreach (XLangRuntimeType xLangRuntimeType in xLangRuntimeNamespace.GetAllTypes())
                    {
                        typeNameLookup.AddResolved(xLangRuntimeType.FullName, xLangRuntimeType);
                        if (close.Contains(xLangRuntimeNamespace))
                        {
                            typeNameLookup.AddResolved(xLangRuntimeType.Name, xLangRuntimeType);
                        }
                    }
                }
                return typeNameLookup;
            }

            bool resolvedAny = true;

            while (resolvedAny)
            {
                resolvedAny = false;
                for (int i = cDefMap.Count - 1; i >= 0; i--)
                {
                    XLangBroadNameLookup<XLangRuntimeType> typeNameLookup = GetView(cDefMap[i].localNs);

                    if (cDefMap[i].cDefinition.BaseClass == null)
                    {
                        XLangRuntimeType newt = ElevateRuntimeType(
                            cDefMap[i].cDefinition,
                            cDefMap[i].localNs,
                            context,
                            null
                        );
                        typeNameLookup.AddResolved(
                            cDefMap[i].cDefinition.Name.GetValue(),
                            newt
                        );
                        ret.Add(newt);
                        cDefMap.RemoveAt(i);

                        resolvedAny = true;
                    }
                    else if (typeNameLookup.CanResolve(cDefMap[i].cDefinition.BaseClass.GetValue()))
                    {
                        XLangRuntimeType newt = ElevateRuntimeType(
                            cDefMap[i].cDefinition,
                            cDefMap[i].localNs,
                            context,
                            typeNameLookup.Resolve(
                                cDefMap[i]
                                    .cDefinition.BaseClass
                                    .GetValue()
                            )
                        );
                        typeNameLookup.AddResolved(
                            cDefMap[i].cDefinition.Name.GetValue(),
                            newt
                        );
                        ret.Add(newt);
                        cDefMap.RemoveAt(i);
                        resolvedAny = true;
                    }


                }


            }

            return ret;
        }

        /// <summary>
        ///     Creates a Mapping of Classes to their Namespaces.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="context"></param>
        /// <param name="ns">Current Namespace</param>
        /// <returns>List of Class/Namespace Mappings</returns>
        private static List<(ClassDefinitionToken cDefinition, XLangRuntimeNamespace localNs)> CreateCdefMap(
            List<IXLangToken> tokens,
            XLangContext context,
            XLangRuntimeNamespace ns = null)
        {

            List<(ClassDefinitionToken cDefinition, XLangRuntimeNamespace localNs)> cDefMap =
                new List<(ClassDefinitionToken cDefinition, XLangRuntimeNamespace localNs)>();
            ProcessUsings(context.DefaultNamespace, tokens);
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is NamespaceDefinitionToken nst)
                {
                    XLangRuntimeNamespace ixLangNs = context.CreateOrGet(nst.Name.GetValue());
                    //List<IXLangRuntimeItem> children =
                    //    ElevateToRuntimeTokens(tokens[i].GetChildren(), context, ixLangNs);

                    ProcessUsings(ixLangNs, tokens[i].GetChildren());
                    cDefMap.AddRange(CreateCdefMap(tokens[i].GetChildren(), context, ixLangNs));
                    //foreach (IXLangRuntimeItem xlRuntimeToken in children)
                    //{
                    //    if (xlRuntimeToken is XLangRuntimeType tDef)
                    //    {
                    //        ixLangNs.AddType(tDef);
                    //    }
                    //    else
                    //    {
                    //        throw new XLangTokenParseException($"Unexpected Type: '{xlRuntimeToken.GetType()}'");
                    //    }
                    //}


                    //ret.Add(ixLangNs);
                }
                else if (tokens[i] is ClassDefinitionToken cDef)
                {
                    XLangRuntimeNamespace localns = ns ?? context.DefaultNamespace;
                    cDefMap.Add((cDef, localns));
                }
                else
                {
                    throw new XLangTokenParseException($"Unexpected Token: '{tokens[i]}'");
                }
            }

            return cDefMap;
        }

        /// <summary>
        ///     Processes the Using Statements inside Namespace Definition Tokens
        /// </summary>
        /// <param name="ns">Current Namespace</param>
        /// <param name="tokens">Token Stream</param>
        private static void ProcessUsings(XLangRuntimeNamespace ns, List<IXLangToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == XLangTokenType.OpUsing)
                {
                    int idx = tokens.FindIndex(i, x => x.Type == XLangTokenType.OpSemicolon);
                    if (idx == -1)
                    {
                        throw new Exception("Expected Semicolon after Using Directive.");
                    }
                    IXLangToken[] name = tokens.GetRange(i + 1, idx - i - 1).ToArray();

                    tokens.RemoveRange(i, idx - i + 1);
                    StringBuilder sb = new StringBuilder();
                    foreach (IXLangToken xLangToken in name)
                    {
                        sb.Append(xLangToken.GetValue());
                    }

                    ns.AddUsing(sb.ToString());
                }
            }
        }

        /// <summary>
        ///     Returns the Runtime Type from the Class Definition Token
        /// </summary>
        /// <param name="input">CDef token</param>
        /// <param name="ns">Containing Namespace</param>
        /// <param name="context">XL Context</param>
        /// <param name="baseClass">Base Class</param>
        /// <returns>Runtime Type</returns>
        public static XLangRuntimeType ElevateRuntimeType(
            ClassDefinitionToken input,
            XLangRuntimeNamespace ns,
            XLangContext context,
            XLangRuntimeType baseClass)
        {
            XLangRuntimeType ret =
                new XLangRuntimeType(input.Name.GetValue(), ns, baseClass, ParseTypeBindingFlags(input));
            ns.AddType(ret);
            ret.SetMembers(ElevateMembers(input.GetChildren(), ret, ns, context).ToArray());
            return ret;
        }

        /// <summary>
        ///     Parses Binding Flags from class Definition Tokens
        /// </summary>
        /// <param name="input">Input Token</param>
        /// <returns>Binding Flags</returns>
        public static XLangBindingFlags ParseTypeBindingFlags(ClassDefinitionToken input)
        {
            XLangBindingFlags current = 0;

            bool Contains(XLangBindingFlags flag)
            {
                return (current & flag) != 0;
            }

            void Add(XLangBindingFlags flag)
            {
                if (Contains(flag))
                {
                    throw new XLangTokenParseException($"Double Modifier '{flag}' on class '{input.Name.GetValue()}'");
                }

                current |= flag;
            }

            foreach (IXLangToken inputModifier in input.Modifiers)
            {
                switch (inputModifier.Type)
                {
                    case XLangTokenType.OpPublicMod:
                        Add(XLangBindingFlags.Public);
                        break;
                    case XLangTokenType.OpPrivateMod:
                        Add(XLangBindingFlags.Private);
                        break;
                    case XLangTokenType.OpAbstractMod:
                        Add(XLangBindingFlags.Abstract);
                        break;
                    case XLangTokenType.OpStaticMod:
                        Add(XLangBindingFlags.Static);
                        break;
                    case XLangTokenType.OpProtectedMod:
                    case XLangTokenType.OpOverrideMod:
                    case XLangTokenType.OpVirtualMod:
                        throw new XLangTokenParseException($"'{inputModifier.Type}' is not a valid modifier");
                    default:
                        throw new XLangTokenParseException($"'{inputModifier.Type}' is not a modifier");
                }
            }

            if ((current & XLangBindingFlags.Static) == 0)
            {
                Add(XLangBindingFlags.Instance);
            }

            return current;
        }

        /// <summary>
        ///     Parses Function Binding Flags from Function Definition Tokens
        /// </summary>
        /// <param name="input">Input Token</param>
        /// <returns>Binding Flags for this Function</returns>
        public static XLangMemberFlags ParseFunctionBindingFlags(FunctionDefinitionToken input)
        {
            XLangMemberFlags current = 0;

            bool Contains(XLangMemberFlags flag)
            {
                return (current & flag) != 0;
            }

            void Add(XLangMemberFlags flag)
            {
                if (Contains(flag))
                {
                    throw new XLangTokenParseException($"Double Modifier '{flag}' on class '{input.Name.GetValue()}'");
                }

                current |= flag;
            }

            foreach (IXLangToken inputModifier in input.Modifiers)
            {
                switch (inputModifier.Type)
                {
                    case XLangTokenType.OpPublicMod:
                        Add(XLangMemberFlags.Public);
                        break;
                    case XLangTokenType.OpPrivateMod:
                        Add(XLangMemberFlags.Private);
                        break;
                    case XLangTokenType.OpAbstractMod:
                        Add(XLangMemberFlags.Abstract);
                        break;
                    case XLangTokenType.OpStaticMod:
                        Add(XLangMemberFlags.Static);
                        break;
                    case XLangTokenType.OpProtectedMod:
                        Add(XLangMemberFlags.Protected);
                        break;
                    case XLangTokenType.OpOverrideMod:
                        Add(XLangMemberFlags.Override);
                        break;
                    case XLangTokenType.OpVirtualMod:
                        Add(XLangMemberFlags.Virtual);
                        break;
                    default:
                        throw new XLangTokenParseException($"'{inputModifier.Type}' is not a modifier");
                }
            }

            if ((current & XLangMemberFlags.Static) == 0)
            {
                Add(XLangMemberFlags.Instance);
            }

            return current;
        }

        /// <summary>
        ///     Parses Property Binding Flags from XLangVarDef Definition Tokens
        /// </summary>
        /// <param name="input">Input Token</param>
        /// <returns>Binding Flags for this Property</returns>
        public static XLangMemberFlags ParsePropertyBindingFlags(XLangVarDefOperand input)
        {
            XLangMemberFlags current = 0;

            bool Contains(XLangMemberFlags flag)
            {
                return (current & flag) != 0;
            }

            void Add(XLangMemberFlags flag)
            {
                if (Contains(flag))
                {
                    throw new XLangTokenParseException(
                        $"Double Modifier '{flag}' on class '{input.value.Name.GetValue()}'"
                    );
                }

                current |= flag;
            }

            foreach (IXLangToken inputModifier in input.value.Modifiers)
            {
                switch (inputModifier.Type)
                {
                    case XLangTokenType.OpPublicMod:
                        Add(XLangMemberFlags.Public);
                        break;
                    case XLangTokenType.OpPrivateMod:
                        Add(XLangMemberFlags.Private);
                        break;
                    case XLangTokenType.OpAbstractMod:
                        Add(XLangMemberFlags.Abstract);
                        break;
                    case XLangTokenType.OpStaticMod:
                        Add(XLangMemberFlags.Static);
                        break;
                    case XLangTokenType.OpProtectedMod:
                        Add(XLangMemberFlags.Protected);
                        break;
                    case XLangTokenType.OpOverrideMod:
                        Add(XLangMemberFlags.Override);
                        break;
                    case XLangTokenType.OpVirtualMod:
                        Add(XLangMemberFlags.Virtual);
                        break;
                    default:
                        throw new XLangTokenParseException($"'{inputModifier.Type}' is not a modifier");
                }
            }

            if ((current & XLangMemberFlags.Static) == 0)
            {
                Add(XLangMemberFlags.Instance);
            }

            return current;
        }

        /// <summary>
        ///     Elevates Members from the children of a Class Token
        /// </summary>
        /// <param name="children">Class Token Children</param>
        /// <param name="input">Type of the Class</param>
        /// <param name="ns">Containing Namespace</param>
        /// <param name="context">XL Context</param>
        /// <returns></returns>
        public static List<IXLangRuntimeMember> ElevateMembers(
            List<IXLangToken> children,
            XLangRuntimeType input,
            XLangRuntimeNamespace ns,
            XLangContext context)
        {
            List<IXLangRuntimeMember> ret = new List<IXLangRuntimeMember>();
            foreach (IXLangToken xlParserToken in children)
            {
                if (xlParserToken is FunctionDefinitionToken fDef)
                {
                    ret.Add(ElevateFunction(input, fDef, ns, context));
                }
                else if (xlParserToken is XLangVarDefOperand pDef)
                {
                    ret.Add(ElevateProperty(input, pDef, ns, context));
                }
                else
                {
                    throw new XLangTokenParseException($"Unrecognized Token: '{xlParserToken.GetType()}'");
                }
            }

            return ret;
        }

        /// <summary>
        ///     Elevates a Property Token into a Runtime Property
        /// </summary>
        /// <param name="tDef">Type Definition</param>
        /// <param name="pDef">Property Definition</param>
        /// <param name="ns">Containing Namespace</param>
        /// <param name="context">XL Context</param>
        /// <returns>Parsed Runtime Property</returns>
        public static IXLangRuntimeProperty ElevateProperty(
            XLangRuntimeType tDef,
            XLangVarDefOperand pDef,
            XLangRuntimeNamespace ns,
            XLangContext context)
        {
            XLangProperty ret = new XLangProperty(
                pDef.value.Name.GetValue(),
                FindType(
                    context.GetVisibleTypes(ns),
                    pDef.value.TypeName.GetValue()
                ),
                tDef,
                ParsePropertyBindingFlags(pDef),
                context,
                pDef.value.InitializerExpression
            );

            return ret;
        }


        /// <summary>
        ///     Finds the Type in a collection of Types
        /// </summary>
        /// <param name="types">Type Collection</param>
        /// <param name="name">Type Name</param>
        /// <returns>Search Result</returns>
        private static XLangRuntimeType FindType(IEnumerable<XLangRuntimeType> types, string name)
        {
            XLangRuntimeType ret = types.FirstOrDefault(x => x.Name == name);
            if (ret == null)
            {
                throw new XLangRuntimeTypeException("Can not find Type: " + name);
            }

            return ret;
        }


        /// <summary>
        ///     Elevates a Function Token into a Runtime Function
        /// </summary>
        /// <param name="tDef">Type Definition</param>
        /// <param name="fDef">Function Definition</param>
        /// <param name="ns">Containing Namespace</param>
        /// <param name="context">XL Context</param>
        /// <returns>Parsed Runtime Function</returns>
        public static IXLangRuntimeFunction ElevateFunction(
            XLangRuntimeType tDef,
            FunctionDefinitionToken fDef,
            XLangRuntimeNamespace ns,
            XLangContext context)
        {
            XLangMemberFlags flags = ParseFunctionBindingFlags(fDef);

            if (fDef.IsConstructor)
            {
                flags |= XLangMemberFlags.Constructor;
            }

            return new XLangFunction(
                fDef.Name.GetValue(),
                FindType(context.GetVisibleTypes(ns), fDef.TypeName.GetValue()),
                tDef,
                fDef.Arguments.Select(
                    x => new XLangFunctionArgument(
                        x.Name.GetValue(),
                        FindType(
                            context
                                .GetVisibleTypes(
                                    ns
                                ),
                            x.TypeName
                                .GetValue()
                        )
                    )
                ).Cast<IXLangRuntimeFunctionArgument>().ToArray(),
                flags,
                fDef.SubTokens.Cast<XLangExpression>().ToArray(),
                context
            );
        }

        /// <summary>
        ///     Delegate for Elevating Type Tokens to Runtime Types
        /// </summary>
        /// <param name="input">Class Definition Input</param>
        /// <param name="ns">Containing Namespace</param>
        /// <param name="context">XL Context</param>
        /// <returns></returns>
        private delegate XLangRuntimeType ElevateTypeDel(
            ClassDefinitionToken input,
            XLangRuntimeNamespace ns,
            XLangContext context);

        //Step 6
        //Parse if/try/while/for/foreach definitions
        //Keys with () => If / While / For / Foreach / Switch / Catch / Using
        //Keys without () => Try / Finally / 

        //Step X
        //"Containerize Keys
        //Find all keys that have containers => "if"/"while"/"for"/...
        //Embed the Blocks into the Token.
        //
        //Add All tokens that are read to the parent.
        //When finding a Key that has a block => set all block content as child of current token.
    }
}
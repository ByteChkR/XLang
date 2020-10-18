using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using XLang.Core;
using XLang.Parser;
using XLang.Parser.Expressions;
using XLang.Parser.Runtime;
using XLang.Parser.Token.Expressions;
using XLang.Parser.Token.Expressions.Operators;
using XLang.Parser.Token.Expressions.Operators.Special;
using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Implementations;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Types;
using XLang.Shared;

namespace XLang.TokenExplorer
{
    public partial class TokenExplorerForm : Form
    {

        public TokenExplorerForm(string[] files)
        {
            InitializeComponent();

            foreach (string file in files)
            {
                XLangContext c = new XLangContext(new XLangSettings(), "XL", "Test");
                MakeMsgBoxInterface(c);
                XLangParser parser = new XLangParser(c);
                if (Directory.Exists(file))
                {
                    foreach (string s in Directory.GetFiles(file, "*.xl", SearchOption.AllDirectories))
                    {
                        parser.Parse(File.ReadAllText(s));
                        CreateView(c, Path.GetFileName(s), c);
                        XLangRuntimeType ftype = c.GetType("DEFAULT.Program");
                        if (ftype != null)
                        {
                            IXLangRuntimeFunction func = ftype.GetMember("Main") as IXLangRuntimeFunction;
                            func.Invoke(null, new IXLangRuntimeTypeInstance[] { null });
                        }
                    }
                }
                else
                {
                    parser.Parse(File.ReadAllText(file));
                    CreateView(c, Path.GetFileName(file), c);
                }

                XLangRuntimeType type = c.GetType("DEFAULT.Program");
                if (type != null)
                {
                    IXLangRuntimeFunction func = type.GetMember("Main") as IXLangRuntimeFunction;
                    func.Invoke(null, new IXLangRuntimeTypeInstance[] { null });
                }
            }

            tvNodeView.AfterSelect += TvNodeView_AfterSelect;
        }

        private void MakeMsgBoxInterface(XLangContext context)
        {
            XLangRuntimeNamespace ns = new XLangRuntimeNamespace(
                                                                 "Test",
                                                                 null,
                                                                 new List<XLangRuntimeType>(),
                                                                 context.Settings
                                                                );
            XLangRuntimeType objectType = new XLangRuntimeType(
                                                               "MSG",
                                                               ns,
                                                               null,
                                                               XLangBindingFlags.Static | XLangBindingFlags.Public
                                                              );
            IXLangRuntimeFunctionArgument arg = new XLangFunctionArgument("inputStr", context.GetType("XL.string"));
            IXLangRuntimeFunction show = new DelegateXLFunction(
                                                                "Show",
                                                                ShowMsgBox,
                                                                context.GetType("XL.void"),
                                                                XLangMemberFlags.Public |
                                                                XLangMemberFlags.Static,
                                                                arg
                                                               );
            objectType.SetMembers(new[] { show });
            ns.AddType(objectType);
            context.LoadNamespace(ns);
        }

        private IXLangRuntimeTypeInstance ShowMsgBox(IXLangRuntimeTypeInstance arg1, IXLangRuntimeTypeInstance[] arg2)
        {
            MessageBox.Show(arg2.First().GetRaw().ToString(), "XL SAYS:");
            return null;
        }

        private void TvNodeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
            }
        }


        private void CreateView(XLangContext context, string name, XLangContext nsCollection)
        {
            TreeNode tn = new TreeNode(name);
            foreach (XLangRuntimeNamespace xLangRuntimeNamespace in nsCollection.GetNamespaces())
            {
                CreateView(context, tn, xLangRuntimeNamespace);
            }

            tvNodeView.Nodes.Add(tn);
        }

        private void CreateView(XLangContext context, TreeNode parent, XLangRuntimeNamespace ns)
        {
            TreeNode tn = new TreeNode(ns.Name);
            TreeNode types = new TreeNode("Types");
            tn.Nodes.Add(types);
            foreach (XLangRuntimeNamespace xLangRuntimeNamespace in ns.Children)
            {
                CreateView(context, tn, xLangRuntimeNamespace);
            }

            foreach (XLangRuntimeType xLangRuntimeType in ns.DefinedTypes)
            {
                CreateView(context, types, xLangRuntimeType);
            }

            parent.Nodes.Add(tn);
        }

        private void CreateView(XLangContext context, TreeNode parent, XLangRuntimeType type)
        {
            TreeNode tn = new TreeNode(type.Name);
            IXLangRuntimeMember[] members = type.GetAllMembers();
            foreach (IXLangRuntimeMember xLangRuntimeMember in members)
            {
                if (xLangRuntimeMember is IXLangRuntimeFunction func)
                {
                    CreateView(context, tn, func);
                }
                else if (xLangRuntimeMember is IXLangRuntimeProperty prop)
                {
                    CreateView(context, tn, prop);
                }
            }

            parent.Nodes.Add(tn);
        }

        private void CreateView(XLangContext context, TreeNode parent, IXLangRuntimeProperty member)
        {
            TreeNode tn = new TreeNode($"Property '{member.Name}' of type '{member.PropertyType.FullName}'");
            parent.Nodes.Add(tn);
        }

        private void CreateView(XLangContext context, TreeNode parent, IXLangRuntimeFunction member)
        {
            TreeNode tn = new TreeNode($"Function '{member.Name}'");
            TreeNode args = new TreeNode("Arguments");
            foreach (IXLangRuntimeFunctionArgument xLangRuntimeFunctionArgument in member.ParameterList)
            {
                CreateView(args, xLangRuntimeFunctionArgument);
            }

            tn.Nodes.Add(new TreeNode($"Return '{member.ReturnType}'"));

            TreeNode exprs = new TreeNode("Block");

            if (member is XLangFunction func)
            {
                CreateView(context, exprs, func);
            }

            tn.Nodes.Add(args);
            tn.Nodes.Add(exprs);
            parent.Nodes.Add(tn);
        }


        private void CreateView(XLangContext context, TreeNode parent, XLangFunction func)
        {
            XLangExpression[] expr = func.InspectBlock();
            foreach (XLangExpression xLangExpression in expr)
            {
                CreateView(context, parent, xLangExpression);
            }
        }

        private void CreateView(XLangContext context, TreeNode parent, XLangExpression expr)
        {
            TreeNode tn = new TreeNode($"Expr: {expr.Type}");
            if (expr is XLangBinaryOp binOp)
            {
                CreateView(context, tn, binOp.Left);
                CreateView(context, tn, binOp.Right);
            }
            else if (expr is XLangUnaryOp unOp)
            {
                CreateView(context, tn, unOp.Left);
            }
            else if (expr is XLangArrayAccessorOp aacOp)
            {
                CreateView(context, tn, aacOp.Left);
                TreeNode subn = new TreeNode("Params: ");
                foreach (XLangExpression xLangExpression in aacOp.ParameterList)
                {
                    CreateView(context, subn, xLangExpression);
                }

                tn.Nodes.Add(subn);
            }
            else if (expr is XLangInvocationOp invOp)
            {
                CreateView(context, tn, invOp.Left);
                TreeNode subn = new TreeNode("Params: ");
                foreach (XLangExpression xLangExpression in invOp.ParameterList)
                {
                    CreateView(context, subn, xLangExpression);
                }

                tn.Nodes.Add(subn);
            }

            parent.Nodes.Add(tn);
        }

        private void CreateView(TreeNode parent, IXLangRuntimeFunctionArgument arg)
        {
            TreeNode tn = new TreeNode($"Argument '{arg.Name}' of type '{arg.Type.FullName}'");
            parent.Nodes.Add(tn);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

    }
}
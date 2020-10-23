using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XLang.BaseTypes;
using XLang.Parser;
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

        private Dictionary<string, IXLangRuntimeItem> itemMap = new Dictionary<string, IXLangRuntimeItem>();

        public TokenExplorerForm(string[] files)
        {
            InitializeComponent();

            foreach (string file in files)
            {

                if (Directory.Exists(file))
                {
                    foreach (string s in Directory.GetFiles(file, "*.xl", SearchOption.AllDirectories))
                    {
                        XLangContext c = new XLangContext(new XLangSettings(), "XL", "Test");
                        MakeMsgBoxInterface(c);
                        XLangParser parser = new XLangParser(c);
                        parser.Parse(File.ReadAllText(s));
                        CreateView(c, Path.GetFileName(s), c);
                    }
                }
                else
                {
                    XLangContext c = new XLangContext(new XLangSettings(), "XL", "Test");
                    MakeMsgBoxInterface(c);
                    XLangParser parser = new XLangParser(c);
                    parser.Parse(File.ReadAllText(file));
                    CreateView(c, Path.GetFileName(file), c);
                }
            }

            tvNodeView.AfterSelect += TvNodeView_AfterSelect;
        }

        private void MakeMsgBoxInterface(XLangContext context)
        {
            if (context.TryGet("XL", out XLangRuntimeNamespace coreNs) && coreNs is XLCoreNamespace cNs)
            {
                cNs.SetWritelineImpl(x => MessageBox.Show(x, "XL Write Line: ", MessageBoxButtons.OK));
            }
        }

        private IXLangRuntimeTypeInstance ShowMsgBox(IXLangRuntimeTypeInstance arg1, IXLangRuntimeTypeInstance[] arg2)
        {
            MessageBox.Show(arg2.First().GetRaw().ToString(), "XL SAYS:");
            return null;
        }

        private void TvNodeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PopulateInfo(e.Node.FullPath);
        }


        private void CreateView(XLangContext context, string name, XLangContext nsCollection)
        {
            TreeNode tn = new TreeNode(name);

            tvNodeView.Nodes.Add(tn);
            foreach (XLangRuntimeNamespace xLangRuntimeNamespace in nsCollection.GetNamespaces())
            {
                CreateView(context, tn, xLangRuntimeNamespace);
            }
        }

        private void CreateView(XLangContext context, TreeNode parent, XLangRuntimeNamespace ns)
        {
            TreeNode tn = new TreeNode(ns.Name);
            TreeNode types = new TreeNode("Types");
            tn.Nodes.Add(types);
            parent.Nodes.Add(tn);
            itemMap[tn.FullPath] = ns;
            foreach (XLangRuntimeNamespace xLangRuntimeNamespace in ns.Children)
            {
                CreateView(context, tn, xLangRuntimeNamespace);
            }

            foreach (XLangRuntimeType xLangRuntimeType in ns.DefinedTypes)
            {
                CreateView(context, types, xLangRuntimeType);
            }

        }

        private void CreateView(XLangContext context, TreeNode parent, XLangRuntimeType type)
        {
            TreeNode tn = new TreeNode(type.Name);
            parent.Nodes.Add(tn);
            itemMap[tn.FullPath] = type;
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

        }

        private void CreateView(XLangContext context, TreeNode parent, IXLangRuntimeProperty member)
        {
            TreeNode tn = new TreeNode($"Property '{member.Name}' of type '{member.PropertyType.FullName}'");
            parent.Nodes.Add(tn);
            itemMap[tn.FullPath] = member;
        }

        private void CreateView(XLangContext context, TreeNode parent, IXLangRuntimeFunction member)
        {
            TreeNode tn = new TreeNode($"Function '{member.Name}'");
            TreeNode args = new TreeNode("Arguments");
            TreeNode exprs = new TreeNode("Block");

            tn.Nodes.Add(args);
            tn.Nodes.Add(exprs);
            parent.Nodes.Add(tn);
            tn.Nodes.Add(new TreeNode($"Return '{member.ReturnType}'"));

            itemMap[tn.FullPath] = member;

            foreach (IXLangRuntimeFunctionArgument xLangRuntimeFunctionArgument in member.ParameterList)
            {
                CreateView(args, xLangRuntimeFunctionArgument);
            }



            if (member is XLangFunction func)
            {
                CreateView(context, exprs, func);
            }

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
            parent.Nodes.Add(tn);
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
                tn.Nodes.Add(subn);
                foreach (XLangExpression xLangExpression in aacOp.ParameterList)
                {
                    CreateView(context, subn, xLangExpression);
                }

            }
            else if (expr is XLangInvocationOp invOp)
            {
                CreateView(context, tn, invOp.Left);
                TreeNode subn = new TreeNode("Params: ");
                tn.Nodes.Add(subn);
                foreach (XLangExpression xLangExpression in invOp.ParameterList)
                {
                    CreateView(context, subn, xLangExpression);
                }

            }

        }

        private void CreateView(TreeNode parent, IXLangRuntimeFunctionArgument arg)
        {
            TreeNode tn = new TreeNode($"Argument '{arg.Name}' of type '{arg.Type.FullName}'");
            parent.Nodes.Add(tn);
        }

        private void PopulateInfo(string path)
        {
            string cData;
            string cTitle;
            IXLangRuntimeItem item = null;
            if (itemMap.ContainsKey(path)) item = itemMap[path];

            if (item is XLangRuntimeType type)
            {
                cTitle = $"Type: {type.Name}";
                cData = $"Full Name: {type.FullName}\nBinding Flags: {type.BindingFlags}\nBase Type: {type.BaseType?.FullName}\nMembers: \n";
                IXLangRuntimeMember[] members = type.GetAllMembers();
                if (members.Length != 0)
                {
                    cData += "\t";
                    for (int i = 0; i < members.Length; i++)
                    {
                        IXLangRuntimeMember xLangRuntimeMember = members[i];
                        cData += xLangRuntimeMember.Name;
                        if (i != members.Length - 1) cData += ", \n\t";
                    }
                }

                cData += "\n";
            }
            else if (item is XLangRuntimeNamespace ns)
            {
                cTitle = $"Namespace: {ns.Name}";
                cData = $"Full Name: {ns.FullName}\nNamespaces: \n";
                if (ns.Children.Count != 0)
                {
                    cData += "\t";
                    for (int i = 0; i < ns.Children.Count; i++)
                    {
                        XLangRuntimeNamespace cns = ns.Children[i];
                        cData += cns.Name;
                        if (i != ns.Children.Count - 1) cData += ", \n\t";
                    }
                }
                cData += "\n";

                cData += $"Types: \n\t";
                if (ns.DefinedTypes.Count != 0)
                    for (int i = 0; i < ns.DefinedTypes.Count; i++)
                    {
                        XLangRuntimeType langRuntimeType = ns.DefinedTypes[i];
                        cData += langRuntimeType.Name;
                        if (i != ns.DefinedTypes.Count - 1) cData += ", \n\t";
                    }
                cData += "\n";
            }
            else if (item is IXLangRuntimeFunction func)
            {
                cTitle = $"Function: {func.Name}";
                cData = $"Full Name: {func.ImplementingClass}.{func.Name}\nBinding Flags: {func.BindingFlags}\nReturn Type: {func.ReturnType}\nArguments: \n";

                if (func.ParameterList.Length != 0)
                {
                    cData += "\t";
                    for (int i = 0; i < func.ParameterList.Length; i++)
                    {
                        IXLangRuntimeFunctionArgument fa = func.ParameterList[i];
                        cData += fa.Name + $"(type:{fa.Type.FullName})";
                        if (i != func.ParameterList.Length - 1) cData += ", \n\t";
                    }
                }
                cData += "\n";
            }
            else if (item is IXLangRuntimeProperty prop)
            {
                cTitle = $"Function: {prop.Name}";
                cData = $"Full Name: {prop.ImplementingClass}.{prop.Name}\nBinding Flags: {prop.BindingFlags}\nProperty Type: {prop.PropertyType}";
            }
            else
            {
                cData = "NO DATA";
                cTitle = path;
            }

            lblName.Text = cTitle;
            rtbCustomData.Text = cData;
        }

        private void btnOpenLiveEditor_Click(object sender, System.EventArgs e)
        {
            LiveEdit le = new LiveEdit();
            le.ShowDialog();

            try
            {
                XLangContext c = new XLangContext(new XLangSettings(), "XL", "Test");
                MakeMsgBoxInterface(c);
                XLangParser parser = new XLangParser(c);
                parser.Parse(le.Code);
                IXLangRuntimeFunction func = c.GetType("DEFAULT.Program")?.GetMember("Main") as IXLangRuntimeFunction;
                if (func == null)
                {
                    MessageBox.Show("Could not Find Entry: DEFAULT.Program.Main()");
                }
                else
                {
                    func.Invoke(null, new IXLangRuntimeTypeInstance[0]);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception);
            }


        }
    }
}
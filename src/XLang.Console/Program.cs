using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using XLang.Parser;
using XLang.Runtime;
using XLang.Runtime.Binding;
using XLang.Runtime.Members;
using XLang.Runtime.Members.Functions;
using XLang.Runtime.Members.Properties;
using XLang.Runtime.Types;
using XLang.Shared.Enum;

namespace XLang.Console
{
    internal class Program
    {

        private static string InputFile;
        private static string FuncTarget;
        private static string[] Imports = new[] { "XL", "DEFAULT" };

        private static void PrintHelp()
        {
            string appName = Path.GetFileName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            System.Console.WriteLine("Commandline Argument Help:");
            System.Console.WriteLine($"Syntax\n\t {appName} <input-file> <flag> <data0> <...> <flag1> <data0> <...");
            System.Console.WriteLine("Flags:\n\t--help\t\t\tDisplay this help text\n\t--target | -t\t\tSet Target start function. Default: \"DEFAULT.Program.Main\"\n\t--import | -i\t\tSet Imported Namespaces. Default \"XL\"");
        }


        private static void PrintHeader()
        {

            System.Console.WriteLine($"XLang Parser Console({Path.GetFileNameWithoutExtension(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath)}) Version: {Assembly.GetExecutingAssembly().GetName().Version}-prototype");
            System.Console.WriteLine($"");

        }

        private static void Main(string[] args)
        {
            Exec(args);
#if DEBUG
            System.Console.ReadLine();
#endif
        }

        private static void Exec(string[] args)
        {
            PrintHeader();

            if (args.Length == 0)
            {
                System.Console.WriteLine("[xlc]No arguments specified.");
                PrintHelp();
                return;
            }

            if (args.Contains("--help") || args.Contains("-h"))
            {
                PrintHelp();
            }
            else
            {
                InputFile = args[0];
                if (args.Length != 1)
                {
                    for (int i = 1; i < args.Length - 1; i++)
                    {
                        if (args[i] == "--target" || args[i] == "-t")
                        {
                            if (args[i + 1].StartsWith("--") || args[i + 1].StartsWith("-"))
                            {
                                System.Console.WriteLine("[xlc]Expected Target Function after flag: " + args[i]);
                            }
                            else
                            {
                                FuncTarget = args[i + 1];
                            }
                        }
                        else if (args[i] == "--import" || args[i] == "-i")
                        {
                            if (args[i + 1].StartsWith("--") || args[i + 1].StartsWith("-"))
                            {
                                System.Console.WriteLine("[xlc]Expected One or more Namespace Imports on Function after flag: " + args[i]);
                            }
                            else
                            {
                                List<string> imps = new List<string>();
                                for (int j = i + 1; j < args.Length; j++)
                                {
                                    if (args[j].StartsWith("--") || args[j].StartsWith("-"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        imps.Add(args[j]);
                                    }
                                }

                                if (imps.Count != 0) Imports = imps.ToArray();
                            }
                        }
                    }
                }
                else
                {
                    FuncTarget = "DEFAULT.Program.Main";
                }

                XLangContext xlC = new XLangContext(new XLangSettings(), Imports);
                XLangParser xlP = new XLangParser(xlC);
                if (!File.Exists(InputFile))
                {
                    System.Console.WriteLine($"[xlc]File '{InputFile}' does not exist.");
                    return;
                }

                string src = File.ReadAllText(InputFile);
                xlP.Parse(src);

                int lastDot = FuncTarget.LastIndexOf('.');
                string funcN = FuncTarget.Remove(0, lastDot + 1);
                string typeN = FuncTarget.Remove(lastDot, FuncTarget.Length - lastDot);
                XLangRuntimeType target = xlC.GetType(typeN);
                if (target == null)
                {
                    System.Console.WriteLine($"[xlc]Type: '{typeN}' does not exist.");
                    return;
                }

                IXLangRuntimeMember mbmr = target.GetMember(
                                                            funcN,
                                                            XLangBindingQuery.Function |
                                                            XLangBindingQuery.Constructor |
                                                            XLangBindingQuery.Property |
                                                            XLangBindingQuery.Inclusive |
                                                            XLangBindingQuery.Public |
                                                            XLangBindingQuery.Private |
                                                            XLangBindingQuery.Protected
                                                           );
                if (mbmr == null)
                {
                    System.Console.WriteLine($"[xlc]Member: '{funcN}' does not exist.");
                    return;
                }

                if (mbmr is IXLangRuntimeFunction func)
                {
                    if (func.MemberType == XLangMemberType.Constructor)
                    {
                        IXLangRuntimeTypeInstance inst = target.CreateEmptyBase();
                        func.Invoke(inst, func.ParameterList.Select(x => x.Type.CreateEmptyBase()).ToArray());
                    }
                    else if( (func.BindingFlags & XLangBindingFlags.Static) != 0)
                    {
                        System.Console.WriteLine($"[xlc]Invoking Function: {func.ImplementingClass.FullName}.{func.Name}");
                        func.Invoke(null, func.ParameterList.Select(x => x.Type.CreateEmptyBase()).ToArray());
                        System.Console.WriteLine($"[xlc]Execution Ended.");
                    }
                    else
                    {
                        System.Console.WriteLine($"[xlc]Function: '{funcN}' can not be invoked because it is no constructor nor static.");
                        
                    }
                }else if (mbmr is IXLangRuntimeProperty prop)
                {
                    if ((prop.BindingFlags & XLangBindingFlags.Static) != 0)
                    {
                        System.Console.WriteLine($"[xlc]Invoking Property: {prop.ImplementingClass.FullName}.{prop.Name}");
                        prop.GetValue(null);
                        System.Console.WriteLine($"[xlc]Execution Ended.");
                    }
                    else
                    {
                        System.Console.WriteLine($"[xlc]Property: '{funcN}' can not be invoked because it is not static.");
                        
                    }
                }
                else
                {
                    System.Console.WriteLine($"[xlc]Member Type '{mbmr}' is not supported");
                    
                }

            }

        }
    }
}

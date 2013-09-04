using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Win32;
using System.Diagnostics;

namespace Unprotect
{
    class Program
    {
        static string dotNetInstall64 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework",
            "InstallRoot",
            @"C:\Windows\Microsoft.NET\Framework\") as string;
        static string dotNetInstall32 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework",
            "InstallRoot",
            @"C:\Windows\Microsoft.NET\Framework\") as string;

        static string ildasmArgFmt = @"/TEXT /TYPELIST /OUT=""{0}.il"" ""{0}""";
        static string ilasmArgFmt = @"/NOLOGO /QUIET /DLL /OUTPUT=""{0}"" ""{0}"".il";

        class SafeAssemblyLoader : MarshalByRefObject
        {
            public void SafeLoadAssembly(string filename) { System.Reflection.Assembly.ReflectionOnlyLoadFrom(filename); }
        }

        static void Main(string[] args)
        {
            string ildasm = "";
            string ilasm = "";

            try
            {
                #region Find the ildasm exe
                string v70a = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-SDKTools";
                string v80a_root = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\v8.0A\";
                string v80a_fx35 = @"WinSDK-NetFx35Tools";
                string v80a_fx40 = @"WinSDK-NetFx40Tools";
                string valueName = @"InstallationFolder";

                string ildasmFolder = Registry.GetValue(v80a_root + v80a_fx40, valueName, null) as string;
                if (ildasmFolder == null || !Directory.Exists(ildasmFolder))
                {
                    ildasmFolder = Registry.GetValue(v80a_root + v80a_fx35, valueName, null) as string;
                    if (ildasmFolder == null || !Directory.Exists(ildasmFolder))
                    {
                        ildasmFolder = Registry.GetValue(v70a, valueName, null) as string;
                        if (ildasmFolder == null || !Directory.Exists(ildasmFolder))
                        {
                            Console.WriteLine("Could not find ildasm installation.");
                            throw new Exception();
                        }
                    }
                }

                ildasm = Path.Combine(ildasmFolder, "ildasm.exe");
                if (!File.Exists(ildasm))
                {
                    Console.WriteLine("Could not find ildasm exe.");
                    throw new Exception();
                }
                #endregion

                #region Find the ilasm exe
                string dotNetInstall = Directory.Exists(dotNetInstall64) ? dotNetInstall64 : dotNetInstall32;
                if (!Directory.Exists(dotNetInstall))
                {
                    Console.WriteLine("Could not find .Net installation.");
                    throw new Exception();
                }
                foreach (var v in new[] { "v4.5.50709", "v4.0.30319", "v4.0.30128", })
                {
                    ilasm = Path.Combine(Path.Combine(dotNetInstall, v), "ilasm.exe");
                    if (File.Exists(ilasm))
                        break;
                }
                if (!File.Exists(ilasm))
                {
                    Console.WriteLine("Could not find ilasm exe.");
                    throw new Exception();
                }
                #endregion
            }
            catch
            {
                Console.Write("Press the 'ANY' key to continue... ");
                Console.ReadKey(true);
                Console.WriteLine();
                Environment.Exit(1);
            }

            try
            {
                List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
                regEx.Add(new Tuple<string, string>("\\.field private (.*)\\s(\\S*EventHandler)", ".field Simlogical $1 $2"));
                regEx.Add(new Tuple<string, string>("(\\s)assembly(\\s)", "$1public$2"));
                regEx.Add(new Tuple<string, string>("(\\s)family(\\s)", "$1public$2"));
                regEx.Add(new Tuple<string, string>("(\\s)private(\\s)", "$1public$2"));
                regEx.Add(new Tuple<string, string>("(\\s)sealed(\\s)", "$1"));
                regEx.Add(new Tuple<string, string>("(\\s)initonly(\\s)", "$1"));
                regEx.Add(new Tuple<string, string>("\\.field (|static )assembly", ".field $1 public"));
                //regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname instance void(\\s*\\n\\s*add_)", ".method private hidebysig specialname instance void$1"));
                //regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname instance void(\\s*\\n\\s*remove_)", ".method private hidebysig specialname instance void$1"));
                //regEx.Add(new Tuple<string, string>("\\.method public hidebysig newslot specialname virtual(| final)(\\s*\\n\\s*instance void  add_)", ".method private hidebysig newslot specialname virtual $1$2"));
                //regEx.Add(new Tuple<string, string>("\\.method public hidebysig newslot specialname virtual(| final)(\\s*\\n\\s*instance void  remove_)", ".method private hidebysig newslot specialname virtual $1$2"));
                //regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname static(\\s*\\n\\s*void  add_)", ".method private hidebysig specialname static$1"));
                //regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname static(\\s*\\n\\s*void  remove_)", ".method private hidebysig specialname static$1"));
                regEx.Add(new Tuple<string, string>("\\.field Simlogical (.*)\\s(\\S*EventHandler)", ".field private $1 $2"));

                string contents = null;

                foreach (string filename in args)
                {
                    Console.Write(filename);
                    // Create a domain
                    AppDomain ad = AppDomain.CreateDomain("CheckAssembly");
                    try
                    {
                        SafeAssemblyLoader loader = (SafeAssemblyLoader)ad.CreateInstanceAndUnwrap(System.Reflection.Assembly.GetEntryAssembly().FullName, typeof(SafeAssemblyLoader).FullName);
                        try
                        {
                            // Load the assembly inside the domain
                            loader.SafeLoadAssembly(filename);
                        }
                        catch
                        {
                            Console.WriteLine(" is not an assembly - skipped!");
                            continue;
                        }
                    }
                    catch { }
                    finally
                    {
                        // Unload the domain
                        try { AppDomain.Unload(ad); }
                        catch { }
                    }

                    Console.Write("... Disassembling");
                    using (Process p = new Process())
                    {
                        p.StartInfo.Arguments = string.Format(ildasmArgFmt, filename);
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.FileName = ildasm;
                        p.StartInfo.UseShellExecute = false;
                        if (p.Start())
                        {
                            p.WaitForExit();
                            if (p.ExitCode != 0)
                            {
                                Console.WriteLine(" - ildasm returned " + p.ExitCode.ToString("X8") + ", skipping!");
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine(" - ildasm failed to start, aborting!");
                            Environment.Exit(1);
                        }
                    }

                    Console.Write("... Unprotecting");
                    using (StreamReader file = new StreamReader(filename + ".il"))
                    {
                        contents = file.ReadToEnd();
                    }

                    foreach (Tuple<string, string> value in regEx)
                    {
                        Regex regex = new Regex(value.Item1);
                        contents = regex.Replace(contents, value.Item2);
                    }

                    using (StreamWriter file = new StreamWriter(filename + ".il"))
                    {
                        file.Write(contents.ToCharArray());
                    }

                    if (File.Exists(filename + ".bak"))
                    {
                        File.Delete(filename + ".bak");
                    }
                    File.Move(filename, filename + ".bak");

                    Console.Write("... Reassembling");
                    using (Process p = new Process())
                    {
                        p.StartInfo.Arguments = string.Format(ilasmArgFmt, filename);
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.FileName = ilasm;
                        p.StartInfo.UseShellExecute = false;
                        if (p.Start())
                        {
                            p.WaitForExit();
                            if (p.ExitCode != 0)
                            {
                                Console.WriteLine("\r\n" + p.StartInfo.FileName + " " + p.StartInfo.Arguments);
                                Console.WriteLine(" - ilasm returned " + p.ExitCode + ", failed!");
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("\r\n" + p.StartInfo.FileName + " " + p.StartInfo.Arguments);
                            Console.WriteLine(" - ilasm failed to start, aborting!");
                            Environment.Exit(1);
                        }
                    }

                    Console.WriteLine("... Done!");
                }
            }
            finally
            {
                Console.Write("Press the 'ANY' key to continue... ");
                Console.ReadKey(true);
                Console.WriteLine();
            }
        }
    }
}

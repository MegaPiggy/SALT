﻿using SALT.Extensions;
using SALT.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SALT.Diagnostics
{
    /// <summary>
    /// A class that helps give source information to stack traces. It uses a file type called Source Database,
    /// generated by this class for any assembly loaded by AssemblyUtils or when a stack trace is displayed.
    /// <br />nbsp;<br />
    /// PDB files need to be present, otherwise a normal stack trace appears.
    /// </summary>
    public static class StackTracing
    {
        private const string ILDASM_ARGS = "/utf8 /linenum /out=\"{file}.sdb\" \"{file}.dll\"";
        private const char PLUS_SYMBOL = '+';
        private const char SLASH_SYMBOL = '/';
        private const string REF_TAG = "&";
        private const string SPIDER_SYMBOL = "::";
        private const string AT_WORD = "  at ";
        private const string ALT_AT_WORD = "  - ";
        private static readonly Dictionary<string, string> TYPE_TO_STRING = new Dictionary<string, string>()
        {
          {
            "System.String",
            "string"
          },
          {
            "System.Boolean",
            "bool"
          },
          {
            "System.Byte",
            "uint8"
          },
          {
            "System.SByte",
            "int8"
          },
          {
            "System.Char",
            "char"
          },
          {
            "System.Decimal",
            "float128"
          },
          {
            "System.Double",
            "float64"
          },
          {
            "System.Single",
            "float32"
          },
          {
            "System.Int32",
            "int32"
          },
          {
            "System.UInt32",
            "uint32"
          },
          {
            "System.Int64",
            "int64"
          },
          {
            "System.UInt64",
            "uint64"
          },
          {
            "System.Int16",
            "int16"
          },
          {
            "System.UInt16",
            "uint16"
          },
          {
            "System.IntPtr",
            "intptr"
          },
          {
            "System.Exception",
            "exception"
          },
          {
            "System.Object",
            "object"
          }
        };

        private static string ildasmLocation = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\ildasm.exe";

        /// <summary>Sets the path for ILDASM, the tool to help generate the SDB files</summary>
        public static void SetIldasmLocation(string path) => ildasmLocation = path;

        public static string CurrentCallTrace() => string.Empty;

        /// <summary>
        /// Parses a stack trace to potentially add source file information during runtime
        /// </summary>
        /// <param name="exception">The exception that caused the trace</param>
        /// <returns>The stack trace parsed</returns>
        public static string ParseStackTrace(Exception exception) => ParseStackTrace(new StackTrace(exception, true));

        /// <summary>
        /// Parses a stack trace to potentially add source file information during runtime
        /// </summary>
        /// <param name="sTrace">The stack trace to parse</param>
        /// <returns>The stack trace parsed</returns>
        public static string ParseStackTrace(StackTrace sTrace)
        {
            if (sTrace.FrameCount == 0)
                return sTrace.ToString();
            StringBuilder stringBuilder = new StringBuilder(byte.MaxValue);
            foreach (StackFrame frame in sTrace.GetFrames())
            {
                FileInfo sourceInfo = ExtractSourceInfo(frame.GetMethod()?.DeclaringType.Assembly);
                string input = sourceInfo == null || !sourceInfo.Exists ? string.Empty : File.ReadAllText(sourceInfo.FullName);
                MethodBase method1 = frame.GetMethod();
                if (method1 != null)
                {
                    string str1 = string.Empty;
                    stringBuilder.Append(AT_WORD);
                    Type declaringType = method1.DeclaringType;
                    if (declaringType != null)
                    {
                        string className = declaringType.FullName.Replace(SPIDER_SYMBOL, ".");
                        if (className.StartsWith("DMD<>"))
                            className = "DynamicMethods";
                        stringBuilder.Append(className);
                        stringBuilder.Append(SPIDER_SYMBOL);
                        str1 = str1 + declaringType.FullName.Replace(SPIDER_SYMBOL, ".") + SPIDER_SYMBOL;
                    }
                    stringBuilder.Append(method1.Name);
                    string str2 = str1 + method1.Name;
                    MethodInfo methodInfo = method1 as MethodInfo;
                    if (methodInfo != null && methodInfo.IsGenericMethod)
                    {
                        Type[] genericArguments = methodInfo.GetGenericArguments();
                        stringBuilder.Append("<");
                        string str3 = str2 + "<";
                        for (int index = 0; index < genericArguments.Length; ++index)
                        {
                            string str4 = (index == 0 ? string.Empty : ", ") + genericArguments[index].Name;
                            stringBuilder.Append(str4);
                            str3 += str4;
                        }
                        stringBuilder.Append(">");
                        str2 = str3 + ">";
                    }
                    stringBuilder.Append("(");
                    string str5 = str2 + "(";
                    ParameterInfo[] parameters = method1.GetParameters();
                    for (int index = 0; index < parameters.Length; ++index)
                    {
                        string str6 = parameters[index].ParameterType.FullName.Replace(REF_TAG, string.Empty);
                        string str7 = TYPE_TO_STRING.ContainsKey(str6) ? parameters[index].ParameterType.FullName.Replace(str6, TYPE_TO_STRING[str6]) : parameters[index].ParameterType.FullName;
                        string str8 = (index == 0 ? string.Empty : ", ") + str7.Replace(PLUS_SYMBOL, SLASH_SYMBOL) + " " + parameters[index].Name;
                        stringBuilder.Append(str8);
                        str5 += str8;
                    }
                    stringBuilder.Append(")");
                    string str9 = str5 + ")";
                    if (frame.GetILOffset() != -1)
                    {
                        string lower = string.Format(" [0x0{0:X4}]", frame.GetILOffset()).ToLower();
                        stringBuilder.Append(lower);
                        string str10 = str9 + lower;
                        Match match = Regex.Match(input, str10.Replace(".", "\\.").Replace("(", "\\(").Replace(")", "\\)").Replace("[", "\\[").Replace("]", "\\]") + "\\|(.+)");
                        if (match.Success)
                        {
                            string fl = match.Groups[1].Value.Replace("\r", string.Empty);
                            if (!string.IsNullOrWhiteSpace(fl))
                                stringBuilder.Append(" in " + fl);
                            else
                            {
                                string assemblyName = frame.GetMethod()?.DeclaringType.Assembly.GetName().Name;
                                if (assemblyName.StartsWith("DMDASM"))
                                    assemblyName = "DynamicAssembly";
                                stringBuilder.Append(" in " + assemblyName);//<UnknownFile>[#]");
                            }
                        }
                        else
                        {
                            string assemblyName = frame.GetMethod()?.DeclaringType.Assembly.GetName().Name;
                            if (assemblyName.StartsWith("DMDASM"))
                                assemblyName = "DynamicAssembly";
                            stringBuilder.Append(" in " + assemblyName);//<UnknownFile>[#]");
                        }
                        stringBuilder.Append(Environment.NewLine);
                    }
                    if ((bool)(frame.GetType().GetMethod("GetIsLastFrameFromForeignExceptionStackTrace", BindingFlags.NonPublic)?.Invoke(frame, new object[0]) ?? false))
                    {
                        stringBuilder.Append(Environment.NewLine);
                        MethodInfo method2 = typeof(Environment).GetMethod("GetResourceString", BindingFlags.Static | BindingFlags.NonPublic);
                        if (method2 != null)
                            stringBuilder.Append(method2.Invoke(null, new object[1]
                            {
                                "Exception_EndStackTraceFromPreviousThrow"
                            }));
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Parses a string of a stack trace to potentially add source file information during runtime
        /// </summary>
        /// <param name="sTrace">The string to parse</param>
        /// <returns>The stack trace parsed</returns>
        public static string ParseStackTrace(string sTrace)
        {
            if (string.IsNullOrWhiteSpace(sTrace))
                return sTrace;
            StringBuilder stringBuilder = new StringBuilder(byte.MaxValue);
            string str1 = sTrace;
            char[] chArray = new char[2] { '\n', '\r' };
            foreach (string @this in str1.Split(chArray))
            {
                if (!string.IsNullOrWhiteSpace(@this))
                {
                    if (@this.Contains("Exception: ") || @this.Contains("Exception of type") || @this.Contains("Exception=") || @this.Matches("(in|at) <?[\\w\\d\\.]*>?\\[[\\d#]*\\]"))
                        stringBuilder.Append(@this.Replace("\n", string.Empty).Replace("\r", string.Empty) + Environment.NewLine);
                    else if (@this.Contains("(wrapper ") || @this.Matches("\\(at .*:0\\)"))
                    {
                        if (!@this.StartsWith(AT_WORD) && !@this.StartsWith(ALT_AT_WORD))
                            stringBuilder.Append(ALT_AT_WORD);
                        stringBuilder.Append(@this.RegexReplace("\\(at .*:0\\)", "(at <UnknownFile>[#])").Replace("\n", string.Empty).Replace("\r", string.Empty) + Environment.NewLine);
                    }
                    else
                    {
                        try
                        {
                            string str2 = @this.Substring(0, @this.LastIndexOfInvariant(" in ")).Replace(AT_WORD, string.Empty).Replace(ALT_AT_WORD, string.Empty);
                            char ch = str2[str2.Length - 1];
                            Console.Console.Log(str2);
                            string str3 = str2.RegexReplace("^(.*)(?:\\s\\(.*)$", "$1");
                            string message = str3.Substring(0, str3.LastIndexOf('.'));
                            Console.Console.Log(message);
                            string name = message;
                            if (str2.Matches("^(.*)(?:\\+<>.*)$"))
                                name = str2.RegexReplace("^(.*)(?:\\+<>.*)$", "$1");
                            Console.Console.Log(name + "\n");
                            string str4 = str2.Substring(0, str2.LastIndexOf(ch == ']' ? '[' : '<')).Replace(message + ".", string.Empty).Replace('[', '<').Replace(']', '>').Replace(" (", "(");
                            foreach (string key in TYPE_TO_STRING.Keys)
                                str4 = str4.Replace(key, TYPE_TO_STRING[key]);
                            string str5 = str2.Substring(str2.LastIndexOf(ch == ']' ? '[' : '<') + 1).Replace(ch == ']' ? "]" : ">", string.Empty);
                            FileInfo sourceInfo = ExtractSourceInfo(TypeUtils.GetTypeBySearch(name)?.Assembly);
                            string input = sourceInfo == null || !sourceInfo.Exists ? string.Empty : File.ReadAllText(sourceInfo.FullName);
                            string empty = string.Empty;
                            stringBuilder.Append(AT_WORD);
                            string className = message;
                            if (className.StartsWith("DMD<>"))
                                className = "DynamicMethods";
                            stringBuilder.Append(className);
                            stringBuilder.Append(SPIDER_SYMBOL);
                            string str6 = empty + message + SPIDER_SYMBOL;
                            stringBuilder.Append(str4);
                            string str7 = str6 + str4;
                            string lower = ("[" + str5 + "]").ToLower();
                            stringBuilder.Append(lower);
                            string str8 = str7 + lower;
                            Match match = Regex.Match(input, str8.Replace(".", "\\.").Replace("(", "\\(").Replace(")", "\\)").Replace("[", "\\[").Replace("]", "\\]") + "\\|(.+)");
                            if (match.Success)
                            {
                                string fl = match.Groups[1].Value.Replace("\r", string.Empty);
                                if (!string.IsNullOrWhiteSpace(fl))
                                    stringBuilder.Append(" in " + fl);
                                else
                                {
                                    string assemblyName = TypeUtils.GetTypeBySearch(name)?.Assembly.GetName().Name;
                                    if (assemblyName.StartsWith("DMDASM"))
                                        assemblyName = "DynamicAssembly";
                                    stringBuilder.Append(" in " + assemblyName);//<UnknownFile>[#]");
                                }
                            }
                            else
                            {
                                string assemblyName = TypeUtils.GetTypeBySearch(name)?.Assembly.GetName().Name;
                                if (assemblyName.StartsWith("DMDASM"))
                                    assemblyName = "DynamicAssembly";
                                stringBuilder.Append(" in " + assemblyName);//<UnknownFile>[#]");
                            }
                            stringBuilder.Append(Environment.NewLine);
                        }
                        catch (Exception)
                        {
                            stringBuilder.Append(@this.Replace("\n", string.Empty).Replace("\r", string.Empty) + Environment.NewLine);
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Extracts the source information from an assembly and the PDB file, creating a SDB file
        /// used by this system to provide line and file info for stack traces
        /// </summary>
        /// <param name="assem">The assembly</param>
        /// <returns>The resulting SDB file if needed</returns>
        public static FileInfo ExtractSourceInfo(Assembly assem)
        {
            if (assem == null)
                return null;
            string path = assem.GetPath();
            FileInfo file = new FileInfo(path.Replace(".dll", ".sdb"));
            if (file.Exists)
            {
                using (StreamReader streamReader = file.OpenText())
                {
                    string str = streamReader.ReadLine();
                    if (str != null && str.Replace("!- ", string.Empty).Equals(File.GetLastWriteTime(path).ToString(CultureInfo.InvariantCulture)))
                        return file;
                }
            }
            if (ildasmLocation == null || !File.Exists(ildasmLocation) || !new FileInfo(path.Replace(".dll", ".pdb")).Exists)
                return null;
            Process.Start(new ProcessStartInfo()
            {
                FileName = ildasmLocation,
                Arguments = ILDASM_ARGS.Replace("{file}", path.Replace(".dll", string.Empty)),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            })?.WaitForExit();
            FileInfo fileInfo = new FileInfo(path.Replace(".dll", ".res"));
            if (fileInfo.Exists)
                fileInfo.Delete();
            if (!File.Exists(file.FullName))
                return null;
            CreateFile(file, File.GetLastWriteTime(path));
            return file;
        }

        private static void CreateFile(FileInfo file, DateTime creationTime)
        {
            List<string> lines = new List<string>();
            string str = File.ReadAllText(file.FullName);
            foreach (string line in Regex.Split(str, "\r\n|\r|\n").Where(line => line.StartsWith(".mresource public ")).Select(line => line.RemoveEverythingBefore(".mresource public ", true).Trim()))
            {
                if (File.Exists(line))
                    File.Delete(line);
            }
            str = str.RegexReplace("\\.class .+ (?='|[^']\\w)", "Class: ").RegexReplace("} \\/\\/ end of class ", "EndClass: ").RegexReplace("\\.method .+(\\n|\\r)?.+(\\s+)(?=['.A-Za-z_]+(\\(|<))", "Method: ").RegexReplace("} \\/\\/ end of method .+:", "EndMethod: ").RegexReplace("(class|valuetype) \\[['A-Za-z0-9.-]+\\]|\\[['A-Za-z0-9.-]+\\]", string.Empty).RegexReplace("\\(class\\s+(?=[a-zA-Z'])", "(").RegexReplace("\\(valuetype\\s+(?=[a-zA-Z'])", "(").RegexReplace("\\<class[^\\(]+", "<").RegexReplace("\\<valuetype[^\\(]+", "<").RegexReplace(",(\\n|\\r)\\s+", ", ").RegexReplace("\\(\\s*(\\n|\\r)\\s+", "(").RegexReplace("\\.line (\\d+),(\\d+) : (\\d+),(\\d+) ('.*')", "File: $5\rLines: $1-$2").RegexReplace("(\\n|\\r)\\s+(?=.+)", "$1");
            lines.Add("!- " + creationTime.ToString(CultureInfo.InvariantCulture));
            ExtractLines(lines, str.Split('\n', '\r'));
#if DEBUG
            Console.Console.LogWarning("Creating file at path: " + file.FullName);
#endif
            File.WriteAllLines(file.FullName, lines);
        }

        private static void ExtractLines(List<string> lines, string[] originLines)
        {
            string @this = string.Empty;
            string str1 = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            foreach (string originLine in originLines)
            {
                if (originLine.StartsWith("Class: "))
                {
                    string str4 = originLine.Replace("Class: ", string.Empty).Trim('\'');
                    @this = @this.Equals(string.Empty) ? str4 : @this + "+" + str4;
                }
                else if (originLine.StartsWith("EndClass: "))
                {
                    string str5 = originLine.Replace("EndClass: ", string.Empty).Trim('\'');
                    if (@this.Contains("+" + str5))
                    {
                        @this = @this.RegexReplace("\\+" + str5 + "[^\\+]*", string.Empty);
                    }
                    else
                    {
                        if (@this.Contains("+"))
                            @this = @this.RegexReplace(str5 + "[^\\+]*", string.Empty);
                        if (@this.StartsWith(str5))
                            @this = string.Empty;
                    }
                }
                else if (originLine.StartsWith("Method: "))
                    str1 = originLine.Replace("Method: ", string.Empty).Replace(" cil managed", string.Empty).RegexReplace("(?<!>)\\(.+?\\) ", string.Empty).Replace("( ", "(").Replace("'", string.Empty).Trim('\'');
                else if (originLine.StartsWith("EndMethod: "))
                    str1 = str1.StartsWith(originLine.Replace("EndMethod: ", string.Empty).Trim('\'')) ? string.Empty : str1;
                else if (originLine.StartsWith("File: ") && !originLine.Equals("File: ''"))
                    str2 = Path.GetFileName(originLine.Replace("File: '", string.Empty).TrimEnd('\''));
                else if (originLine.StartsWith("Lines: "))
                {
                    string[] strArray = originLine.Replace("Lines: ", string.Empty).Split('-');
                    int num1 = int.Parse(strArray[0].Trim());
                    int num2 = int.Parse(strArray[1].Trim());
                    str3 = num1 == num2 ? string.Format("[{0}]", num1) : string.Format("[{0} to {1}]", num1, num2);
                }
                else if (originLine.StartsWith("IL_"))
                {
                    string rr = (originLine.RegexReplace("IL_(.{4}):.+", @this + (str1.Equals(string.Empty) ? string.Empty : SPIDER_SYMBOL) + str1 + " [0x0$1]|" + str2 + str3));
                    if (rr.Contains("+SALT"))
                        rr = "SALT" + rr.Reverse().RemoveEverythingAfter("+SALT".Reverse(), true).Reverse();
                    if (rr.Contains("A_1"))
                        rr = rr.Replace("A_1", "");
                    lines.Add(rr);
                }
            }
        }
    }
}
using SALT.Diagnostics;
using System;
using System.Diagnostics;
using System.Reflection;

namespace SALT.Utils
{
    /// <summary>An utility class to help when working with Assemblies</summary>
    public static class AssemblyUtils
    {
        /// <summary>Gets the Relevant assembly by tracing the call</summary>
        /// <param name="type">The type to get the assembly from</param>
        /// <returns>The relevant assembly to the context</returns>
        public static Assembly GetRelevant(object type = null)
        {
            if (type != null)
                return type.GetType().Assembly;
            StackTrace stackTrace = new StackTrace();
            Assembly calling = Assembly.GetCallingAssembly();
            StackFrame[] frames = stackTrace.GetFrames();
            return ExceptionUtils.IgnoreErrors(() =>
            {
                if (frames == null)
                    return calling;
                foreach (StackFrame stackFrame in frames)
                {
                    Assembly assembly = stackFrame.GetMethod().DeclaringType?.Assembly;
                    if (assembly != calling && assembly != null)
                        return assembly;
                }
                return calling;
            }, calling);
        }

        /// <summary>
        /// Loads an assembly from a given name, generating the SDB files from the PDB symbols.
        /// </summary>
        /// <param name="fullPath">The full path of the assembly to load</param>
        /// <returns>The loaded assembly</returns>
        public static Assembly LoadWithSymbols(string fullPath)
        {
            Assembly assem = Assembly.LoadFrom(fullPath);
            StackTracing.ExtractSourceInfo(assem);
            return assem;
        }
    }
}

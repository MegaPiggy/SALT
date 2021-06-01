using System.Diagnostics;
using System.Reflection;

namespace SALT.Utils
{
    /// <summary>
    /// An utility class to help with <see cref="System.Reflection"/>
    /// </summary>
    public static class ReflectionUtils
    {
        internal static Assembly ourAssembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Gets the last <see cref="Assembly"/> used.
        /// </summary>
        /// <returns>The relevant <see cref="Assembly"/></returns>
        public static Assembly GetRelevantAssembly()
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            try
            {
                foreach (StackFrame stackFrame in frames)
                {
                    Assembly assembly = stackFrame.GetMethod().DeclaringType.Assembly;
                    if (assembly != ReflectionUtils.ourAssembly)
                        return assembly;
                }
            }
            catch { }
            return ReflectionUtils.ourAssembly;
        }

        /// <summary>
        /// Checks to see if the code was previously in a certain method.
        /// </summary>
        /// <param name="search">Name of the method</param>
        /// <returns><see langword="true"/> if was, <see langword="false"/> if it was not.</returns>
        public static bool FromMethod(string search)
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            try
            {
                foreach (StackFrame stackFrame in frames)
                {
                    string name = stackFrame.GetMethod().Name;
                    if (name == search)
                        return true;
                }
            }
            catch
            {
            }
            return false;
        }
    }
}

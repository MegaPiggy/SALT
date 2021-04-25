using System.Diagnostics;
using System.Reflection;

namespace SAL.Utils
{
    public static class ReflectionUtils
    {
        internal static Assembly ourAssembly = Assembly.GetExecutingAssembly();

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
            catch
            {
            }
            return ReflectionUtils.ourAssembly;
        }
    }
}

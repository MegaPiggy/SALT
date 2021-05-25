using SALT.Editor.Runtime;
using System;

namespace SALT.Editor
{
    internal class ResolvedInstance
    {
        public object Instance { get; private set; }

        public static ResolvedInstance Resolve(IInstanceInfo info)
        {
            ResolvedInstance resolvedInstance = new ResolvedInstance();
            IRuntimeInstanceInfo runtimeInstanceInfo = (IRuntimeInstanceInfo) info; 
            resolvedInstance.Instance = runtimeInstanceInfo.OnResolve();
            return resolvedInstance;
        }
    }
}

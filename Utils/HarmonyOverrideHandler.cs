using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SALT.Utils
{
    internal static class HarmonyOverrideHandler
    {
        public static Dictionary<MethodBase, List<MethodBase>> methodsToPatch = new Dictionary<MethodBase, List<MethodBase>>();

        public static bool CheckMethod() => true;

        public static IEnumerable<CodeInstruction> Transpiler(
          MethodBase __originalMethod,
          IEnumerable<CodeInstruction> instr,
          ILGenerator gen)
        {
            List<MethodBase> methods;
            if (HarmonyOverrideHandler.methodsToPatch.TryGetValue(__originalMethod, out methods))
            {
                Label lab1 = gen.DefineLabel();
                yield return new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(HarmonyOverrideHandler), "CheckMethod"));
                yield return new CodeInstruction(OpCodes.Brfalse, (object)lab1);
                Label labLoop = new Label();
                foreach (MethodBase methodBase in methods)
                {
                    MethodBase v = methodBase;
                    CodeInstruction code = new CodeInstruction(OpCodes.Ldarg_0);
                    if (labLoop != new Label())
                        code.labels.Add(labLoop);
                    labLoop = gen.DefineLabel();
                    yield return code;
                    yield return new CodeInstruction(OpCodes.Ldind_Ref);
                    yield return new CodeInstruction(OpCodes.Isinst, (object)v.ReflectedType);
                    yield return new CodeInstruction(OpCodes.Brfalse, (object)(v == methods.Last<MethodBase>() ? lab1 : labLoop));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    int i = 1;
                    ParameterInfo[] parameterInfoArray = v.GetParameters();
                    for (int index = 0; index < parameterInfoArray.Length; ++index)
                    {
                        ParameterInfo p = parameterInfoArray[index];
                        yield return new CodeInstruction(OpCodes.Ldarg, (object)i++);
                        p = (ParameterInfo)null;
                    }
                    parameterInfoArray = (ParameterInfo[])null;
                    yield return new CodeInstruction(v.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, (object)v);
                    yield return new CodeInstruction(OpCodes.Ret);
                    code = (CodeInstruction)null;
                    v = (MethodBase)null;
                }
                List<CodeInstruction> list = instr.ToList<CodeInstruction>();
                list.First<CodeInstruction>().labels.Add(lab1);
                foreach (CodeInstruction codeInstruction in list)
                {
                    CodeInstruction v = codeInstruction;
                    yield return v;
                    v = (CodeInstruction)null;
                }
                lab1 = new Label();
                labLoop = new Label();
                list = (List<CodeInstruction>)null;
            }
            else
            {
                foreach (CodeInstruction codeInstruction in instr)
                {
                    CodeInstruction v = codeInstruction;
                    yield return v;
                    v = (CodeInstruction)null;
                }
            }
        }

        public static void PatchAll()
        {
            foreach (KeyValuePair<MethodBase, List<MethodBase>> keyValuePair in HarmonyOverrideHandler.methodsToPatch)
                HarmonyPatcher.Instance.Patch(keyValuePair.Key, transpiler: new HarmonyMethod(AccessTools.Method(typeof(HarmonyOverrideHandler), "Transpiler")));
        }

        public static void LoadOverrides(Module module)
        {
            foreach (MethodInfo methodInfo in ((IEnumerable<Type>)module.GetTypes()).SelectMany<Type, MethodInfo>((Func<Type, IEnumerable<MethodInfo>>)(x => (IEnumerable<MethodInfo>)x.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))))
            {
                if (((IEnumerable<object>)methodInfo.GetCustomAttributes(false)).Any<object>((Func<object, bool>)(x => x is HarmonyOverrideAttribute)))
                {
                    Type type = methodInfo.ReflectedType;
                    MethodBase key = (MethodBase)null;
                    do
                    {
                        type = type.BaseType;
                        if (!(type == (Type)null))
                            key = (MethodBase)AccessTools.Method(type, methodInfo.Name, ((IEnumerable<ParameterInfo>)methodInfo.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>)(x => x.ParameterType)).ToArray<Type>(), methodInfo.IsGenericMethod ? methodInfo.GetGenericArguments() : (Type[])null);
                        else
                            break;
                    }
                    while (!(key != (MethodBase)null));
                    List<MethodBase> methodBaseList;
                    if (!HarmonyOverrideHandler.methodsToPatch.TryGetValue(key, out methodBaseList))
                    {
                        methodBaseList = new List<MethodBase>();
                        HarmonyOverrideHandler.methodsToPatch[key] = methodBaseList;
                    }
                    methodBaseList.Add((MethodBase)methodInfo);
                }
            }
        }
    }
}

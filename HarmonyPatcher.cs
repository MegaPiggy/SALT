using HarmonyLib;
using System.Reflection;

namespace SALT
{
    public static class HarmonyPatcher
    {
        private static Harmony _instance;

        internal static Harmony Instance
        {
            get
            {
                if (HarmonyPatcher._instance == null)
                    HarmonyPatcher.InitializeInstance();
                return HarmonyPatcher._instance;
            }
        }

        private static void InitializeInstance() => HarmonyPatcher._instance = new Harmony("net.megapiggy.SALT");

        internal static void PatchAll() => HarmonyPatcher.Instance.PatchAll(Assembly.GetExecutingAssembly());

        public static Harmony SetInstance(string name)
        {
            Mod currentMod = Mod.GetCurrentMod();
            currentMod.CreateHarmonyInstance(name);
            return currentMod.HarmonyInstance;
        }

        public static Harmony GetInstance() => Mod.GetCurrentMod().HarmonyInstance;
    }
}

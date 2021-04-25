using System.Collections.Generic;

namespace SAL.Editor
{
    internal static class ReplacerCache
    {
        private static Dictionary<IFieldReplacer, ResolvedReplacer> replacers = new Dictionary<IFieldReplacer, ResolvedReplacer>();

        public static ResolvedReplacer GetReplacer(IFieldReplacer replacer)
        {
            ResolvedReplacer resolvedReplacer1;
            if (ReplacerCache.replacers.TryGetValue(replacer, out resolvedReplacer1))
                return resolvedReplacer1;
            ResolvedReplacer resolvedReplacer2 = ResolvedReplacer.Resolve(replacer);
            ReplacerCache.replacers.Add(replacer, resolvedReplacer2);
            return resolvedReplacer2;
        }

        public static void ClearCache() => ReplacerCache.replacers.Clear();
    }
}

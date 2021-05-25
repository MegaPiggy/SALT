using System;
using System.Collections.Generic;
using System.Reflection;

namespace SALT.Editor
{
    internal class ResolvedReplacer
    {
        public ResolvedInstance InstanceInfo;
        public Dictionary<FieldInfo, FieldInfo> FieldToField = new Dictionary<FieldInfo, FieldInfo>();

        public static ResolvedReplacer Resolve(IFieldReplacer replacer)
        {
            ResolvedReplacer resolvedReplacer = new ResolvedReplacer();
            resolvedReplacer.InstanceInfo = ResolvedInstance.Resolve(replacer.InstanceInfo);
            if (replacer.FieldReplacements == null)
                throw new Exception("No replacements found!");
            foreach (IFieldReplacement fieldReplacement in (IEnumerable<IFieldReplacement>)replacer.FieldReplacements)
            {
                FieldInfo field1;
                FieldInfo field2;
                if (!fieldReplacement.TryResolveTarget(out field1) || !fieldReplacement.TryResolveSource(out field2))
                    throw new Exception("Unable to resolve field!");
                resolvedReplacer.FieldToField.Add(field1, field2);
            }
            return resolvedReplacer;
        }
    }
}

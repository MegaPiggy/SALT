using System.Collections.Generic;

namespace SAL.Editor
{
    public interface IFieldReplacer
    {
        IInstanceInfo InstanceInfo { get; }

        bool ReplaceInChildren { get; }

        ICollection<IFieldReplacement> FieldReplacements { get; }
    }
}

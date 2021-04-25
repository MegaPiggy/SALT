using System.Reflection;

namespace SAL.Editor
{
    public interface IFieldReplacement
    {
        bool TryResolveSource(out FieldInfo field);

        bool TryResolveTarget(out FieldInfo field);
    }
}

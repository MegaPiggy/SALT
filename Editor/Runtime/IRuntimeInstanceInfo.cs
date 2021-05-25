namespace SALT.Editor.Runtime
{
    public interface IRuntimeInstanceInfo
    {
        RuntimeInstanceProviderDelegate OnResolve { get; }
    }
}

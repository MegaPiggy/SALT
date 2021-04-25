namespace SAL.Editor.Runtime
{
    public interface IRuntimeInstanceInfo
    {
        RuntimeInstanceProviderDelegate OnResolve { get; }
    }
}

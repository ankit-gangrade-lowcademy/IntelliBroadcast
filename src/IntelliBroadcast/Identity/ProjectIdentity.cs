namespace IntelliBroadcast.Identity;

public interface IProjectIdentity
{
    Guid ProjectHash { get; }
}

public sealed class ProjectIdentity : IProjectIdentity
{
    // Single source of truth. This product identifier must never be generated or configuration-driven.
    public static readonly Guid FixedProjectHash = new("4f0d7c72-9a31-4c68-b526-2e8f5a1d93b7");
    public Guid ProjectHash => FixedProjectHash;
}

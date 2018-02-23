namespace OpenSoftware.EntityGraphCore
{
    public enum EntityState
    {
        New,
        Deleted,
        Modified
    }

    public interface IEntityStateTracking
    {
        EntityState EntityState { get; }
    }
}
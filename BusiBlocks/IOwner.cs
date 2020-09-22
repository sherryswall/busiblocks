namespace BusiBlocks
{
    /// <summary>
    /// Use this interface to define an entity with an owner (user).
    /// </summary>
    public interface IOwner
    {
        string Owner { get; }

        string Groups { get; set; }
    }
}
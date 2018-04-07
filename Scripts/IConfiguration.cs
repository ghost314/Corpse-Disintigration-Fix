/// <summary>
/// This interface defines the configuration data used by the rest of the mod.
/// </summary>
public interface IConfiguration
{
    /// <summary>
    /// The farthest distance from the killed zombie, that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    int MAX_SEARCH_RADIUS
    {
        get;
    }

    /// <summary>
    /// The highest point that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    int MAX_HEIGHT
    {
        get;
    }

    /// <summary>
    /// The lowest point that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    int MIN_HEIGHT
    {
        get;
    }

    /// <summary>
    /// How long the corpse positioner algorithm should remember scanned blocks, before resetting.
    /// </summary>
    uint CACHE_PERSISTANCE
    {
        get;
    }

    /// <summary>
    /// Indicates if the mod is currently running in debug mode.
    /// </summary>
    bool DEBUG_MODE
    {
        get;
    }

    /// <summary>
    /// Indicates if the corpse positioner algorithm should avoid spawning corpses on top of spikes.
    /// </summary>
    bool SPAWN_ON_SPIKES
    {
        get;
    }
}
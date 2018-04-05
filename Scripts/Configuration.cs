/// <summary>
/// This class houses some application wide constants, used as configuration data.
/// </summary>
public class Configuration
{
    private readonly int maxHeight;
    private readonly int minHeight;
    private readonly int searchRadius;
    private readonly uint cachePersistance;

    /// <summary>
    /// Sets up a new configuration, based on the given values.
    /// </summary>
    /// <param name="maxHeight">The highest point that the corpse positioner algorithm should ever try to scan.</param>
    /// <param name="minHeight">The lowest point that the corpse positioner algorithm should ever try to scan.</param>
    /// <param name="searchRadius">The farthest distance from the killed zombie, that the corpse positioner algorithm should ever try to scan.</param>
    /// <param name="cachePersistance">How long the corpse positioner algorithm should remember scanned blocks, before resetting.</param>
    public Configuration(int maxHeight, int minHeight, int searchRadius, uint cachePersistance)
    {
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        this.searchRadius = searchRadius;
        this.cachePersistance = cachePersistance;
    }

    /// <summary>
    /// The highest point that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    public int MAX_HEIGHT
    {
        get
        {
            return maxHeight;
        }
    }

    /// <summary>
    /// The lowest point that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    public int MIN_HEIGHT
    {
        get
        {
            return minHeight;
        }
    }

    /// <summary>
    /// The farthest distance from the killed zombie, that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    public int MAX_SEARCH_RADIUS
    {
        get
        {
            return searchRadius;
        }
    }

    /// <summary>
    /// How long the corpse positioner algorithm should remember scanned blocks, before resetting.
    /// </summary>
    public uint CACHE_PERSISTANCE
    {
        get
        {
            return cachePersistance;
        }
    }
}

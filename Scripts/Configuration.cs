public class Configuration
{
    private readonly int maxHeight;
    private readonly int minHeight;
    private readonly int searchRadius;
    private readonly uint cachePersistance;

    public Configuration(int maxHeight, int minHeight, int searchRadius, uint cachePersistance)
    {
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        this.searchRadius = searchRadius;
        this.cachePersistance = cachePersistance;
    }

    public int MAX_HEIGHT
    {
        get
        {
            return maxHeight;
        }
    }

    public int MIN_HEIGHT
    {
        get
        {
            return minHeight;
        }
    }

    public int MAX_SEARCH_RADIUS
    {
        get
        {
            return searchRadius;
        }
    }

    public uint CACHE_PERSISTANCE
    {
        get
        {
            return cachePersistance;
        }
    }
}

public class Configuration
{
    private readonly int maxHeight;
    private readonly int minHeight;
    private readonly int searchRadius;

    public Configuration(int maxHeight, int minHeight, int searchRadius)
    {
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        this.searchRadius = searchRadius;
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
}

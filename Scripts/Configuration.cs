public class Configuration
{
    public Configuration(int maxHeight, int minHeight, int searchRadius)
    {
        MAX_HEIGHT = maxHeight;
        MIN_HEIGHT = minHeight;
        MAX_SEARCH_RADIUS = searchRadius;
    }

    public int MAX_HEIGHT { get; }

    public int MIN_HEIGHT { get; }

    public int MAX_SEARCH_RADIUS { get; }
}

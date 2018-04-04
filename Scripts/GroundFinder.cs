public class GroundFinder : IGroundFinder
{
    public delegate bool IsMovementRestrictingBlock(Vector3i location);
    private readonly int MAX_HEIGHT;
    private readonly int MIN_HEIGHT;
    private readonly IsMovementRestrictingBlock isMovementRestrictingBlock;
    private readonly GroundPositionCache cache;

    public GroundFinder(Configuration config, IsMovementRestrictingBlock isMovementRestrictingBlock, GroundPositionCache cache)
    {
        MAX_HEIGHT = config.MAX_HEIGHT;
        MIN_HEIGHT = config.MIN_HEIGHT;
        this.isMovementRestrictingBlock = isMovementRestrictingBlock;
        this.cache = cache;
    }

    public int FindPositionAboveGroundAt(Vector3i location)
    {
        int groundPosition = -1;
        if (cache.GetGroundPositionFor(location, out groundPosition))
        {
            return groundPosition;
        }

        int originalHeight = location.y;
        if (isMovementRestrictingBlock(location))
        {
            bool hadToSwitchDirections = false;
            int airBlock = FindEmptySpaceAt(location, out hadToSwitchDirections);
            location.y = airBlock;
            if (hadToSwitchDirections)
                originalHeight = MAX_HEIGHT;
        }
        groundPosition = FindGroundBelow(location);
        location.y = originalHeight;
        if (groundPosition < MIN_HEIGHT)
        {
            cache.CacheGroundPositionForLocation(location, -1);
            return -1;
        }
        groundPosition++;
        cache.CacheGroundPositionForLocation(location, groundPosition);
        return groundPosition;
    }

    private int FindEmptySpaceAt(Vector3i location, out bool hadToSwitchDirections)
    {
        int emptySpaceAboveLocation = FindEmptySpaceAbove(location);
        if (emptySpaceAboveLocation != -1)
        {
            hadToSwitchDirections = false;
            return emptySpaceAboveLocation;
        }
        hadToSwitchDirections = true;
        return FindEmptySpaceBelow(location);
    }

    private int FindEmptySpaceAbove(Vector3i location)
    {
        location.y++;
        for (; location.y < MAX_HEIGHT; location.y++)
        {
            if (!isMovementRestrictingBlock(location))
                return location.y;
        }
        return -1;
    }

    private int FindEmptySpaceBelow(Vector3i location)
    {
        location.y--;
        for (; location.y >= MIN_HEIGHT; location.y--)
        {
            if (!isMovementRestrictingBlock(location))
                return location.y;
        }
        return -1;
    }

    private int FindGroundBelow(Vector3i location)
    {
        location.y--;
        for (; location.y >= MIN_HEIGHT; location.y--)
        {
            if (isMovementRestrictingBlock(location))
                return location.y;
        }
        return -1;
    }
}

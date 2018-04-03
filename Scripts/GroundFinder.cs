public class GroundFinder : IGroundFinder
{
    public delegate bool IsMovementRestrictingBlock(Vector3i location);
    private readonly int MAX_HEIGHT;
    private readonly int MIN_HEIGHT;
    private readonly IsMovementRestrictingBlock isMovementRestrictingBlock;

    public GroundFinder(Configuration config, IsMovementRestrictingBlock isMovementRestrictingBlock)
    {
        MAX_HEIGHT = config.MAX_HEIGHT;
        MIN_HEIGHT = config.MIN_HEIGHT;
        this.isMovementRestrictingBlock = isMovementRestrictingBlock;
    }

    public int FindPositionAboveGroundAt(Vector3i location)
    {
        if (isMovementRestrictingBlock(location))
        {
            int airBlock = FindEmptySpaceAt(location);
            location.y = airBlock;
        }
        int groundPosition = FindGroundBelow(location);
        if (groundPosition < MIN_HEIGHT)
        {
            return -1;
        }
        return groundPosition + 1;
    }

    private int FindEmptySpaceAt(Vector3i location)
    {
        int emptySpaceAboveLocation = FindEmptySpaceAbove(location);
        if (emptySpaceAboveLocation != -1)
            return emptySpaceAboveLocation;
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

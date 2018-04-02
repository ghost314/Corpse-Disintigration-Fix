public class GroundFinder
{
    private readonly int MAX_HEIGHT;
    private readonly int MIN_HEIGHT;
    private readonly ZombieCorpsePositioner.GetBlock getBlock;

    public GroundFinder(int maxHeight, int minHeight, ZombieCorpsePositioner.GetBlock getBlock)
    {
        MAX_HEIGHT = maxHeight;
        MIN_HEIGHT = minHeight;
        this.getBlock = getBlock;
    }

    public int FindPositionAboveGroundAt(Vector3i location)
    {
        IBlock currentBlock = getBlock(location);
        if (currentBlock.IsCollideMovement)
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
            if (!getBlock(location).IsCollideMovement)
                return location.y;
        }
        return -1;
    }

    private int FindEmptySpaceBelow(Vector3i location)
    {
        location.y--;
        for (; location.y >= MIN_HEIGHT; location.y--)
        {
            if (!getBlock(location).IsCollideMovement)
                return location.y;
        }
        return -1;
    }

    private int FindGroundBelow(Vector3i location)
    {
        location.y--;
        for (; location.y >= MIN_HEIGHT; location.y--)
        {
            if (getBlock(location).IsCollideMovement)
                return location.y;
        }
        return -1;
    }
}

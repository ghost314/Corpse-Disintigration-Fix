public class ZombieCorpsePositioner
{
    private const int MAX_SEARCH_RADIUS = 200;
    private const int MAX_HEIGHT = 254;
    private const int MIN_HEIGHT = 3;

    public delegate void Logger(string msg);
    public delegate IBlock GetBlock(Vector3i location);

    private Logger log;
    private GetBlock getBlock;

    public ZombieCorpsePositioner(Logger log, GetBlock getBlock)
    {
        this.log = log;
        this.getBlock = getBlock;
    }

    public Vector3i FindSpawnLocationStartingFrom(Vector3i origin)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        nextPositionToCheck.y = FindGroundAt(nextPositionToCheck.x, nextPositionToCheck.z);
        if (!HasGoreBlock(nextPositionToCheck))
            return nextPositionToCheck;

        for (int distanceFromOrigin = 1; distanceFromOrigin < MAX_SEARCH_RADIUS; distanceFromOrigin++)
        {
            Vector3i foundPosition = CheckTopRow(nextPositionToCheck, origin, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            foundPosition = CheckRightColumn(nextPositionToCheck, origin, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            foundPosition = CheckBottomRow(nextPositionToCheck, origin, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            foundPosition = CheckLeftColumn(nextPositionToCheck, origin, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;
        }

        return origin;
    }
    
    private Vector3i CheckTopRow(Vector3i nextPositionToCheck, Vector3i origin, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z + distanceFromOrigin;
        nextPositionToCheck.x = origin.x;
        return CheckRow(nextPositionToCheck, distanceFromOrigin);
    }

    private Vector3i CheckRightColumn(Vector3i nextPositionToCheck, Vector3i origin, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z;
        nextPositionToCheck.x = origin.x + distanceFromOrigin;
        return CheckColumn(nextPositionToCheck, distanceFromOrigin - 1);
    }

    private Vector3i CheckBottomRow(Vector3i nextPositionToCheck, Vector3i origin, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z - distanceFromOrigin;
        nextPositionToCheck.x = origin.x;
        return CheckRow(nextPositionToCheck, distanceFromOrigin);
    }

    private Vector3i CheckLeftColumn(Vector3i nextPositionToCheck, Vector3i origin, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z;
        nextPositionToCheck.x = origin.x - distanceFromOrigin;
        return CheckColumn(nextPositionToCheck, distanceFromOrigin - 1);
    }

    private Vector3i CheckRow(Vector3i origin, int offset)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int xOffset = -offset; xOffset <= offset; xOffset++)
        {
            nextPositionToCheck.x = origin.x + xOffset;
            nextPositionToCheck.y = FindGroundAt(nextPositionToCheck.x, nextPositionToCheck.z);
            if (!HasGoreBlock(nextPositionToCheck))
                return nextPositionToCheck;
        }
        return Vector3i.zero;
    }

    private Vector3i CheckColumn(Vector3i origin, int offset)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int zOffset = -offset; zOffset <= offset; zOffset++)
        {
            nextPositionToCheck.z = origin.z + zOffset;
            nextPositionToCheck.y = FindGroundAt(nextPositionToCheck.x, nextPositionToCheck.z);
            if (!HasGoreBlock(nextPositionToCheck))
                return nextPositionToCheck;
        }
        return Vector3i.zero;
    }

    private bool HasGoreBlock(Vector3i location)
    {
        log("Checking for gore blocks at: " + location.x + "," + location.y + "," + location.z);
        IBlock block = getBlock(location);

        if (block.BlockTag == BlockTags.Gore)
            return true;

        log("No gore blocks at: " + location.x + "," + location.y + "," + location.z);
        return false;
    }

    private int FindGroundAt(int x, int z)
    {
        Vector3i location = new Vector3i(x, 0, z);
        for (int y = MIN_HEIGHT; y < MAX_HEIGHT; y++)
        {
            location.y = y;
            IBlock block = getBlock(location);
            if (!block.IsCollideMovement)
                return y;
        }
        return MAX_HEIGHT;
    }
}

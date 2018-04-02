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
        Vector3i potentialSpawnPoint = FindValidSpawnPointAt(nextPositionToCheck);
        if (potentialSpawnPoint != Vector3i.zero)
            return potentialSpawnPoint;

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
            Vector3i potentialSpawnPoint = FindValidSpawnPointAt(nextPositionToCheck);
            if (potentialSpawnPoint != Vector3i.zero)
                return potentialSpawnPoint;
        }
        return Vector3i.zero;
    }

    private Vector3i CheckColumn(Vector3i origin, int offset)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int zOffset = -offset; zOffset <= offset; zOffset++)
        {
            nextPositionToCheck.z = origin.z + zOffset;
            Vector3i potentialSpawnPoint = FindValidSpawnPointAt(nextPositionToCheck);
            if (potentialSpawnPoint != Vector3i.zero)
                return potentialSpawnPoint;
        }
        return Vector3i.zero;
    }

    private Vector3i FindValidSpawnPointAt(Vector3i location)
    {
        location.y = FindPositionAboveGroundAt(location);
        if (location.y < MIN_HEIGHT)
        {
            return Vector3i.zero;
        }
        if (!HasGoreBlock(location))
        {
            return location;
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

    private int FindPositionAboveGroundAt(Vector3i location)
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

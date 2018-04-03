public class ZombieCorpsePositioner
{
    private readonly int MAX_SEARCH_RADIUS;
    private readonly int MAX_HEIGHT;
    private readonly int MIN_HEIGHT;

    public delegate void Logger(string msg);
    public delegate bool IsGoreBlock(Vector3i location);

    private readonly Logger log;
    private readonly IsGoreBlock isGoreBlock;
    private readonly IGroundFinder groundFinder;

    public ZombieCorpsePositioner(Logger log, IsGoreBlock isGoreBlock, IGroundFinder groundFinder, Configuration config)
    {
        this.log = log;
        this.isGoreBlock = isGoreBlock;
        this.groundFinder = groundFinder;
        MAX_SEARCH_RADIUS = config.MAX_SEARCH_RADIUS;
        MAX_HEIGHT = config.MAX_HEIGHT;
        MIN_HEIGHT = config.MIN_HEIGHT;
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
        location.y = groundFinder.FindPositionAboveGroundAt(location);
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

        if (isGoreBlock(location))
            return true;

        log("No gore blocks at: " + location.x + "," + location.y + "," + location.z);
        return false;
    }

}

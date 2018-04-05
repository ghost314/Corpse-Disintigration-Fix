using System;

/// <summary>
/// The main purpose of this class, is to take a certain starting position (where a zombie died) and find a reasonable place to spawn its corpse.
/// <para>
/// This class makes use of an IGroundFinder and a delegate method that checks if a particular block is a zombie corpse to implement its algorithm.
/// </para>
/// </summary>
public class ZombieCorpsePositioner
{
    private readonly int MAX_SEARCH_RADIUS;
    private readonly int MAX_HEIGHT;
    private readonly int MIN_HEIGHT;

    /// <summary>
    /// A delegate method to use for debug logging.
    /// </summary>
    /// <param name="msg">The message to be logged.</param>
    public delegate void Logger(string msg);

    /// <summary>
    /// A delegate method to check whether or not a particular position is already occupied by a corpse.
    /// </summary>
    /// <param name="location">The position to check</param>
    /// <returns>True IFF the given location is occupied by a corpse.</returns>
    public delegate bool IsGoreBlock(Vector3i location);

    private readonly Logger log;
    private readonly IsGoreBlock isGoreBlock;
    private readonly IGroundFinder groundFinder;

    /// <summary>
    /// Creates a new zombie corpse positioner, backed by the given parameters.
    /// </summary>
    /// <param name="log">This will be used for debug logging.</param>
    /// <param name="isGoreBlock">This will be used to check whether or not a certain location is already occupied with a corpse.</param>
    /// <param name="groundFinder">This will be used to locate the 'ground level' for a certain starting position.</param>
    /// <param name="config">This will be used to limit the scope of the search, for performance reasons.</param>
    /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
    public ZombieCorpsePositioner(Logger log, IsGoreBlock isGoreBlock, IGroundFinder groundFinder, Configuration config)
    {
        this.log = log ?? throw new ArgumentNullException("log", "The given logger must not be null");
        this.isGoreBlock = isGoreBlock ?? throw new ArgumentNullException("isGoreBlock", "The given gore block delegate must not be null");
        this.groundFinder = groundFinder ?? throw new ArgumentNullException("groundFinder", "The given ground finder must not be null");
        if (config == null)
            throw new ArgumentNullException("config", "The given configuration must not be null");
        MAX_SEARCH_RADIUS = config.MAX_SEARCH_RADIUS;
        MAX_HEIGHT = config.MAX_HEIGHT;
        MIN_HEIGHT = config.MIN_HEIGHT;
    }

    /// <summary>
    /// Given a certain starting position, this method will attempt to find a reasonable spot to spawn a new corpse. If no reasonable place can be found, then <paramref name="origin"/> will be returned.
    /// </summary>
    /// <param name="origin">The starting point to begin looking for a corpse spawn location from.</param>
    /// <returns>The best location that could be found to spawn the new corpse.</returns>
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
            log("Unable to find ground position starting from: " + location);
            return Vector3i.zero;
        }
        if (!isGoreBlock(location))
        {
            return location;
        }
        return Vector3i.zero;
    }
}

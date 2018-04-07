using System;

/// <summary>
/// The main purpose of this class, is to take a certain starting position (where a zombie died) and find a reasonable place to spawn its corpse.
/// <para>
/// This class makes use of an IGroundFinder and a delegate method that checks if a particular block is stable, in order to implement its algorithm.
/// </para>
/// <para>
/// A block is considered to be 'stable' if it's normally capable of supporting other blocks being laid down on top of it.
/// </para>
/// </summary>
public class ZombieCorpsePositioner
{
    private readonly int maxSearchRadius;
    private readonly int maxHeight;
    private readonly int minHeight;

    /// <summary>
    /// A delegate method to use for debug logging.
    /// </summary>
    /// <param name="msg">The message to be logged.</param>
    public delegate void Logger(string msg);

    /// <summary>
    /// A delegate method to check whether or not a particular position is occupied by a stable block.
    /// </summary>
    /// <param name="location">The position to check</param>
    /// <returns>True IFF the given location is occupied by a stable block.</returns>
    public delegate bool IsStableBlock(Vector3i location);

    /// <summary>
    /// A delegate method to check whether or not a corpse can be spawned at a particular location by the game engine.
    /// </summary>
    /// <param name="location">The position to check.</param>
    /// <param name="corpseBlock">The corpse block we need to try and spawn.</param>
    /// <returns>True IFF the game is capable of spawning a corpse block at the given location.</returns>
    public delegate bool IsValidSpawnPointForCorpseBlock(Vector3i location, BlockValue corpseBlock);

    private readonly Logger log;
    private readonly IsStableBlock isStableBlock;
    private readonly IsValidSpawnPointForCorpseBlock isValidSpawnPointForCorpseBlock;
    private readonly IGroundFinder groundFinder;

    /// <summary>
    /// Creates a new zombie corpse positioner, backed by the given parameters.
    /// </summary>
    /// <param name="log">This will be used for debug logging.</param>
    /// <param name="isStableBlock">This will be used to check whether or not a certain location is occupied with a stable block.</param>
    /// <param name="isValidSpawnPointForCorpseBlock">This will be used to check whether or not the game engine can spawn a corpse block at a certain location.</param>
    /// <param name="groundFinder">This will be used to locate the 'ground level' for a certain starting position.</param>
    /// <param name="config">This will be used to limit the scope of the search, for performance reasons.</param>
    /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
    public ZombieCorpsePositioner(Logger log, IsStableBlock isStableBlock, IsValidSpawnPointForCorpseBlock isValidSpawnPointForCorpseBlock, IGroundFinder groundFinder, IConfiguration config)
    {
        if (log == null)
            throw new ArgumentNullException("log", "The given logger must not be null");
        this.log = log;
        if (isStableBlock == null)
            throw new ArgumentNullException("isStableBlock", "The given block stability delegate must not be null");
        this.isStableBlock = isStableBlock;
        if (isValidSpawnPointForCorpseBlock == null)
            throw new ArgumentNullException("isValidSpawnPointForCorpseBlock", "The given spawn validity delegate must not be null");
        this.isValidSpawnPointForCorpseBlock = isValidSpawnPointForCorpseBlock;
        if (groundFinder == null)
            throw new ArgumentNullException("groundFinder", "The given ground finder must not be null");
        this.groundFinder = groundFinder;
        if (config == null)
            throw new ArgumentNullException("config", "The given configuration must not be null");
        maxSearchRadius = config.MAX_SEARCH_RADIUS;
        maxHeight = config.MAX_HEIGHT;
        minHeight = config.MIN_HEIGHT;
    }

    /// <summary>
    /// Given a certain starting position and corpse block type, this method will attempt to find a reasonable spot to spawn a new corpse. If no reasonable place can be found, then <paramref name="origin"/> will be returned.
    /// </summary>
    /// <param name="origin">The starting point to begin looking for a corpse spawn location from.</param>
    /// <param name="corpseBlock">The corpse block that needs to be spawned.</param>
    /// <returns>The best location that could be found to spawn the new corpse.</returns>
    public Vector3i FindSpawnLocationStartingFrom(Vector3i origin, BlockValue corpseBlock)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        Vector3i potentialSpawnPoint = FindValidSpawnPointAt(nextPositionToCheck, corpseBlock);
        if (potentialSpawnPoint != Vector3i.zero)
            return potentialSpawnPoint;

        for (int distanceFromOrigin = 1; distanceFromOrigin < maxSearchRadius; distanceFromOrigin++)
        {
            Vector3i foundPosition = CheckTopRow(nextPositionToCheck, origin, corpseBlock, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            foundPosition = CheckRightColumn(nextPositionToCheck, origin, corpseBlock, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            foundPosition = CheckBottomRow(nextPositionToCheck, origin, corpseBlock, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            foundPosition = CheckLeftColumn(nextPositionToCheck, origin, corpseBlock, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;
        }

        return origin;
    }

    private Vector3i CheckTopRow(Vector3i nextPositionToCheck, Vector3i origin, BlockValue corpseBlock, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z + distanceFromOrigin;
        nextPositionToCheck.x = origin.x;
        return CheckRow(nextPositionToCheck, corpseBlock, distanceFromOrigin);
    }

    private Vector3i CheckRightColumn(Vector3i nextPositionToCheck, Vector3i origin, BlockValue corpseBlock, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z;
        nextPositionToCheck.x = origin.x + distanceFromOrigin;
        return CheckColumn(nextPositionToCheck, corpseBlock, distanceFromOrigin - 1);
    }

    private Vector3i CheckBottomRow(Vector3i nextPositionToCheck, Vector3i origin, BlockValue corpseBlock, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z - distanceFromOrigin;
        nextPositionToCheck.x = origin.x;
        return CheckRow(nextPositionToCheck, corpseBlock, distanceFromOrigin);
    }

    private Vector3i CheckLeftColumn(Vector3i nextPositionToCheck, Vector3i origin, BlockValue corpseBlock, int distanceFromOrigin)
    {
        nextPositionToCheck.z = origin.z;
        nextPositionToCheck.x = origin.x - distanceFromOrigin;
        return CheckColumn(nextPositionToCheck, corpseBlock, distanceFromOrigin - 1);
    }

    private Vector3i CheckRow(Vector3i origin, BlockValue corpseBlock, int offset)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int xOffset = -offset; xOffset <= offset; xOffset++)
        {
            nextPositionToCheck.x = origin.x + xOffset;
            Vector3i potentialSpawnPoint = FindValidSpawnPointAt(nextPositionToCheck, corpseBlock);
            if (potentialSpawnPoint != Vector3i.zero)
                return potentialSpawnPoint;
        }
        return Vector3i.zero;
    }

    private Vector3i CheckColumn(Vector3i origin, BlockValue corpseBlock, int offset)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int zOffset = -offset; zOffset <= offset; zOffset++)
        {
            nextPositionToCheck.z = origin.z + zOffset;
            Vector3i potentialSpawnPoint = FindValidSpawnPointAt(nextPositionToCheck, corpseBlock);
            if (potentialSpawnPoint != Vector3i.zero)
                return potentialSpawnPoint;
        }
        return Vector3i.zero;
    }

    private Vector3i FindValidSpawnPointAt(Vector3i location, BlockValue corpseBlock)
    {
        location.y = groundFinder.FindPositionAboveGroundAt(location);
        if (location.y < minHeight)
        {
            log("Unable to find ground position starting from: " + location);
            return Vector3i.zero;
        }
        Vector3i ground = new Vector3i(location.x, location.y - 1, location.z);
        if (isStableBlock(ground) && isValidSpawnPointForCorpseBlock(location, corpseBlock))
            return location;
        return Vector3i.zero;
    }
}

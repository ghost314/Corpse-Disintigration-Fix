using System;
/// <summary>
/// This implementation of IGroundFinder, relies on a delegate to tell if a certain block allows movement or not.
/// <para>
/// A proper 'ground level' is considered to be a block that allows movement, which also has a block that does not allow movement directly underneath it.
/// </para>
/// <para>
/// GroundFinder also makes use of a data cache, to remember its previous findings at certain locations, to improve performance.
/// </para>
/// </summary>
public class GroundFinder : IGroundFinder
{
    /// <summary>
    /// This delegate can be used to determine if a block at a certain location allows free movement or not.
    /// </summary>
    /// <param name="location">The block location to check.</param>
    /// <returns>True if the block at the given location prevents movement, false otherwise.</returns>
    public delegate bool IsMovementRestrictingBlock(Vector3i location);
    private readonly int maxHeight;
    private readonly int minHeight;
    private readonly IsMovementRestrictingBlock isMovementRestrictingBlock;
    private readonly GroundPositionCache cache;

    /// <summary>
    /// Creates a new ground finder, which relies on the supplied delegate and data cache, to find and cache the 'ground level' for various positions.
    /// </summary>
    /// <param name="config">The configuration parameters to use when conducting the search.</param>
    /// <param name="isMovementRestrictingBlock">The delegate to use, to check if a block allows free movement or not.</param>
    /// <param name="cache">The data cache, to use for improving performance.</param>
    /// <exception cref="ArgumentNullException">If any of the supplied parameters are null.</exception>
    public GroundFinder(IConfiguration config, IsMovementRestrictingBlock isMovementRestrictingBlock, GroundPositionCache cache)
    {
        if (config == null)
            throw new ArgumentNullException("config", "The given configuration data must not be null");
        maxHeight = config.MAX_HEIGHT;
        minHeight = config.MIN_HEIGHT;
        if (isMovementRestrictingBlock == null)
            throw new ArgumentNullException("isMovementRestrictingBlock", "The given delegate function must not be null");
        this.isMovementRestrictingBlock = isMovementRestrictingBlock;
        if (cache == null)
            throw new ArgumentNullException("cache", "The given data cache must not be null");
        this.cache = cache;
    }

    /// <summary>
    /// Given a certain starting point, this method will scan blocks above and below the starting point, and look for a suitable place to spawn a new corpse.
    /// <para>The algorithm does <i>not</i> account for existing corpse blocks, its only purpose is to find a suitable 'ground level'.</para>
    /// </summary>
    /// <param name="startingPoint">The starting location to begin searching for ground from.</param>
    /// <returns>The ground position for the given starting point, or -1 if no reasonable position could be found.</returns>
    public int FindPositionAboveGroundAt(Vector3i startingPoint)
    {
        int groundPosition = -1;
        if (cache.GetGroundPositionFor(startingPoint, out groundPosition))
            return groundPosition;

        Vector3i airBlock = GetAirBlockForLocation(startingPoint);
        groundPosition = FindGroundBelow(airBlock);

        if (groundPosition < minHeight)
        {
            cache.CacheGroundPositionForLocation(startingPoint, -1);
            return -1;
        }

        groundPosition++;

        if (airBlock.y < startingPoint.y)
            startingPoint.y = maxHeight;
        cache.CacheGroundPositionForLocation(startingPoint, groundPosition);

        return groundPosition;
    }

    private Vector3i GetAirBlockForLocation(Vector3i location)
    {
        if (isMovementRestrictingBlock(location))
        {
            int airBlock = FindEmptySpaceAt(location);
            location.y = airBlock;
        }
        return location;
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

        for (; location.y < maxHeight; location.y++)
            if (!isMovementRestrictingBlock(location))
                return location.y;

        return -1;
    }

    private int FindEmptySpaceBelow(Vector3i location)
    {
        location.y--;

        for (; location.y >= minHeight; location.y--)
            if (!isMovementRestrictingBlock(location))
                return location.y;

        return -1;
    }

    private int FindGroundBelow(Vector3i location)
    {
        location.y--;

        for (; location.y >= minHeight; location.y--)
            if (isMovementRestrictingBlock(location))
                return location.y;

        return -1;
    }
}

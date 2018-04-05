using System;
using System.Collections.Generic;

/// <summary>
/// This class can be used to maintain a temporary store of ground levels found from various starting points in the world. The cache is automatically cleared upon access if sufficient time has elapsed.
/// </summary>
public class GroundPositionCache
{
    private readonly Dictionary<Vector2i, List<HeightRange>> groundForPosition = new Dictionary<Vector2i, List<HeightRange>>();
    private readonly ICacheTimer timer;

    /// <summary>
    /// Creates a new cache of ground positions, backed by the given timer.
    /// </summary>
    /// <param name="timer">The timer to use to determine when to clear the cache.</param>
    /// <exception cref="ArgumentNullException">If timer is null.</exception>
    public GroundPositionCache(ICacheTimer timer)
    {
        this.timer = timer ?? throw new ArgumentNullException("timer", "The given timer must not be null");
    }

    /// <summary>
    /// This method will record <paramref name="height"/> as the ground level for all positions in between <paramref name="position"/> and <paramref name="height"/> inclusive.
    /// <para>If <paramref name="height"/> is -1, indicating no ground level was found, then the results will be recorded for all positions with the same x,z coordinates.</para>
    /// </summary>
    /// <param name="position">The starting position, that maps to this ground height.</param>
    /// <param name="height">The ground height for the given position, and all positions in between</param>
    public void CacheGroundPositionForLocation(Vector3i position, int height)
    {
        Vector2i queryPosition = new Vector2i(position.x, position.z);
        List<HeightRange> ranges = GetRangesForPosition(queryPosition);

        HeightRange newHeightRange = GenerateHeightRangeFor(position.y, height);
        RemoveOverlaps(ranges, newHeightRange);

        ranges.Add(newHeightRange);
        groundForPosition[queryPosition] = ranges;
    }

    private List<HeightRange> GetRangesForPosition(Vector2i position)
    {
        List<HeightRange> ranges;
        if (!groundForPosition.TryGetValue(position, out ranges))
            ranges = new List<HeightRange>();
        return ranges;
    }

    private void RemoveOverlaps(List<HeightRange> ranges, HeightRange newHeightRange)
    {
        ranges.FindAll(newHeightRange.IsOverlappingWith).ForEach(range =>
        {
            newHeightRange.Min = Math.Min(newHeightRange.Min, range.Min);
            newHeightRange.Max = Math.Max(newHeightRange.Max, range.Max);
        });
        ranges.RemoveAll(newHeightRange.IsOverlappingWith);
    }

    private HeightRange GenerateHeightRangeFor(int positionHeight, int groundHeight)
    {
        int minValue;
        int maxValue;
        if (groundHeight == -1)
        {
            minValue = int.MinValue;
            maxValue = int.MaxValue;
        }
        else
        {
            minValue = Math.Min(positionHeight, groundHeight);
            maxValue = Math.Max(positionHeight, groundHeight);
        }
        return new HeightRange(minValue, maxValue, groundHeight);
    }

    /// <summary>
    /// Retrieves the last recorded ground level for a certain position, if one exists, otherwise false is returned and <paramref name="height"/> is set to 0.
    /// <para>
    /// Note that calling this method will trigger the cache to re-assess whether or not enough time has elapsed since the last time the cache was cleared, to necessitate another clear.
    /// </para>
    /// </summary>
    /// <param name="position">The position that we want to know the ground level for.</param>
    /// <param name="height">Will be set to the last recorded ground level for <paramref name="position"/>, or 0 if none was ever recorded.</param>
    /// <returns>True if there was a cached value for the ground position, false otherwise.</returns>
    public bool GetGroundPositionFor(Vector3i position, out int height)
    {
        ClearOldEntries();
        Vector2i queryPosition = new Vector2i(position.x, position.z);
        List<HeightRange> ranges = GetRangesForPosition(queryPosition);
        return FindHeightInRangesForPosition(ranges, position.y, out height);
    }

    private bool FindHeightInRangesForPosition(List<HeightRange> ranges, int positionHeight, out int recordedHeight)
    {
        HeightRange range = ranges.Find(r => r.IsHeightInRange(positionHeight));
        if (range != null)
        {
            recordedHeight = range.GroundHeight;
            return true;
        }
        recordedHeight = 0;
        return false;
    }

    private void ClearOldEntries()
    {
        if (!timer.IsCacheStillValid())
            Clear();
    }

    /// <summary>
    /// This method can be used to manually clear all previously recorded data from the cache. This should generally be unnecessary, due to the built in timer mechanism.
    /// </summary>
    public void Clear()
    {
        groundForPosition.Clear();
    }
}

using System;
using System.Collections.Generic;

public class GroundPositionCache
{
    private readonly Dictionary<Vector2i, List<HeightRange>> groundForPosition = new Dictionary<Vector2i, List<HeightRange>>();
    private readonly ICacheTimer timer;

    public GroundPositionCache(ICacheTimer timer)
    {
        this.timer = timer;
    }

    public void CacheGroundPositionForLocation(Vector3i position, int height)
    {
        Vector2i queryPosition = new Vector2i(position.x, position.z);
        List<HeightRange> ranges;
        if (!groundForPosition.TryGetValue(queryPosition, out ranges))
            ranges = new List<HeightRange>();

        HeightRange newHeightRange = GenerateHeightRangeFor(position.y, height);
        ranges.FindAll(newHeightRange.IsOverlappingWith).ForEach(range =>
        {
            newHeightRange.Min = Math.Min(newHeightRange.Min, range.Min);
            newHeightRange.Max = Math.Max(newHeightRange.Max, range.Max);
        });
        ranges.RemoveAll(newHeightRange.IsOverlappingWith);
        ranges.Add(newHeightRange);
        groundForPosition[queryPosition] = ranges;
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

    public bool GetGroundPositionFor(Vector3i position, out int height)
    {
        ClearOldEntries();
        Vector2i queryPosition = new Vector2i(position.x, position.z);
        List<HeightRange> ranges = new List<HeightRange>();
        if (groundForPosition.TryGetValue(queryPosition, out ranges))
        {
            foreach (HeightRange range in ranges)
            {
                if (range.IsHeightInRange(position.y))
                {
                    height = range.GroundHeight;
                    return true;
                }
            }
        }
        height = 0;
        return false;
    }

    private void ClearOldEntries()
    {
        if (!timer.IsCacheStillValid())
            Clear();
    }

    public void Clear()
    {
        groundForPosition.Clear();
    }

    private struct HeightRange
    {
        private int min;
        private int max;
        private int groundHeight;

        public HeightRange(int min, int max, int groundHeight)
        {
            this.min = min;
            this.max = max;
            this.groundHeight = groundHeight;
        }

        public int Max
        {
            get { return max; }
            set
            {
                max = value;
            }
        }
        public int Min
        {
            get { return min; }
            set
            {
                min = value;
            }
        }
        public int GroundHeight
        {
            get { return groundHeight; }
            set
            {
                groundHeight = value;
            }
        }

        public bool IsHeightInRange(int height)
        {
            return min <= height && height <= max;
        }

        public bool IsOverlappingWith(HeightRange other)
        {
            return IsHeightInRange(other.min) || IsHeightInRange(other.max) || other.IsHeightInRange(min) || other.IsHeightInRange(max);
        }
    }
}


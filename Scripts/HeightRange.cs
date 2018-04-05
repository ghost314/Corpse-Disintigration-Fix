using System;

/// <summary>
/// This class represents a ground height value, that was recorded as the proper ground height for a range of altitudes.
/// <para>
/// Note that this class deals with only a single dimension, as it is expected to be indexed by (x,z) coordinates.
/// </para>
/// </summary>
public class HeightRange
{
    private int min;
    private int max;
    private int groundHeight;

    /// <summary>
    /// Creates a new height range, which represents a ground height level for all altitudes in between <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="min">The lower end of this range of heights.</param>
    /// <param name="max">The higher end of this range of heights.</param>
    /// <param name="groundHeight">The ground height level, for all heights in between <paramref name="min"/> and <paramref name="max"/></param>
    /// <exception cref="ArgumentException">If <paramref name="max"/> &lt; <paramref name="min"/>, or if <paramref name="groundHeight"/> is not in the specified range.</exception>
    public HeightRange(int min, int max, int groundHeight)
    {
        if (max < min)
            throw new ArgumentException("The given min and max values are in the wrong order, given: " + min + "," + max, "min, max");
        this.min = min;
        this.max = max;
        this.groundHeight = groundHeight;
        if (!IsHeightInRange(groundHeight))
            throw new ArgumentException("The given ground height is not within the given area, min:" + min + ", max:" + max + ", ground:" + groundHeight, "groundHeight");
    }

    /// <summary>
    /// The upper end of this height range, can be altered in either direction, unless it compromizes the other fields.
    /// </summary>
    /// <exception cref="ArgumentException">If set to a value less than min, or ground height.</exception>
    public int Max
    {
        get { return max; }
        set
        {
            if (value < min)
                throw new ArgumentException("The new max value can not be less than the current min value, min: " + min + ", new max: " + value, "Max");
            if (value < groundHeight)
                throw new ArgumentException("The new max value can not be less than the current ground height, ground: " + groundHeight + ", new max: " + value, "Max");
            max = value;
        }
    }

    /// <summary>
    /// The lower end of this height range, can be altered in either direction, unless it compromizes the other fields.
    /// </summary>
    /// <exception cref="ArgumentException">If set to a value greater than max, or ground height.</exception>
    public int Min
    {
        get { return min; }
        set
        {
            if (max < value)
                throw new ArgumentException("The new min value can not be greater than the current max value, max: " + max + ", new min: " + value, "Min");
            if (groundHeight < value)
                throw new ArgumentException("The new min value can not be greater than the current ground height, ground: " + groundHeight + ", new min: " + value, "Min");
            min = value;
        }
    }

    /// <summary>
    /// The ground level for all heights in this height range, can be altered in either direction, as long as it stays within min and max.
    /// </summary>
    /// <exception cref="ArgumentException">If set to a value less than min, or greater than max.</exception>
    public int GroundHeight
    {
        get { return groundHeight; }
        set
        {
            if (value < min || max < value)
                throw new ArgumentException("The new ground height can not be outside of the current height range, min: " + min + ", max: " + max + ", new ground: " + value, "GroundHeight");
            groundHeight = value;
        }
    }

    /// <summary>
    /// This method can be used to quickly check whether or not this height range includes a specific height.
    /// </summary>
    /// <param name="height">The hieght we want to check against.</param>
    /// <returns>True IFF <paramref name="height"/> is within this height range.</returns>
    public bool IsHeightInRange(int height)
    {
        return min <= height && height <= max;
    }

    /// <summary>
    /// This method can be used to quickly check whether or not any part of this height range is intersecting/overlapping with another height range.
    /// </summary>
    /// <param name="other">A height range to test against.</param>
    /// <returns>True IFF this height range is intersecting/overlapping with <paramref name="other"/></returns>
    public bool IsOverlappingWith(HeightRange other)
    {
        return IsHeightInRange(other.min) || IsHeightInRange(other.max) || other.IsHeightInRange(min) || other.IsHeightInRange(max);
    }
}

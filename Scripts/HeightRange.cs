using System;

public class HeightRange
{
    private int min;
    private int max;
    private int groundHeight;

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

    public bool IsHeightInRange(int height)
    {
        return min <= height && height <= max;
    }

    public bool IsOverlappingWith(HeightRange other)
    {
        return IsHeightInRange(other.min) || IsHeightInRange(other.max) || other.IsHeightInRange(min) || other.IsHeightInRange(max);
    }
}

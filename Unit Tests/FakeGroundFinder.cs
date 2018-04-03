using System.Collections.Generic;

class FakeGroundFinder : IGroundFinder
{
    public List<Vector3i> GroundPositions { get; } = new List<Vector3i>();

    public int FindPositionAboveGroundAt(Vector3i location)
    {
        foreach (Vector3i vector in GroundPositions)
        {
            if (vector.x == location.x && vector.z == location.z)
                return vector.y;
        }
        return -1;
    }
}

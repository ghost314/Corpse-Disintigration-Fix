/// <summary>
/// This interface can be used to locate the proper altitude that a zombie corpse should spawn at, assuming no changes in the other 2 dimensions, and without considering other corpse blocks.
/// </summary>
public interface IGroundFinder
{
    /// <summary>
    /// Given a certain starting point, this method will scan blocks above and below the starting point, and look for a suitable place to spawn a new corpse.
    /// <para>The algorithm does <i>not</i> account for existing corpse blocks, its only purpose is to find a suitable 'ground level'.</para>
    /// </summary>
    /// <param name="startingPoint">The starting location to begin searching for ground from.</param>
    /// <returns>The ground position for the given starting point, or -1 if no reasonable position could be found.</returns>
    int FindPositionAboveGroundAt(Vector3i startingPoint);
}

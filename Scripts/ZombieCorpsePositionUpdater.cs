using UnityEngine;

/// <summary>
/// This class serves as a bridge, between the 7D2D game engine, and the rest of this mod.
/// </summary>
public static class ZombieCorpsePositionUpdater
{
    private static readonly ZombieCorpsePositioner positioner = ZombieCorpsePositionerFactory.GenerateNewPositioner();

    /// <summary>
    /// This method is called directly from the core 7D2D game engine, due to the patch script.
    /// </summary>
    /// <param name="position">This will be set to the current position of the zombie that needs to spawn a corpse.</param>
    /// <returns></returns>
    public static Vector3 GetUpdatedPosition(Vector3 position)
    {
        Vector3i newPosition = positioner.FindSpawnLocationStartingFrom(World.worldToBlockPos(position));
        return new Vector3(newPosition.x, newPosition.y, newPosition.z);
    }
}

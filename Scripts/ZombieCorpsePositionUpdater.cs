using System;
using UnityEngine;

/// <summary>
/// This class serves as a bridge, between the 7D2D game engine, and the rest of this mod.
/// </summary>
public static class ZombieCorpsePositionUpdater
{
    private static readonly ZombieCorpsePositioner POSITIONER = ZombieCorpsePositionerFactory.GenerateNewPositioner();
    private static readonly IConfiguration CONFIG = BlockCorpseDisintigrationFixConfig.GetLoadedInstance();

    /// <summary>
    /// This method is called directly from the core 7D2D game engine, due to the patch script.
    /// </summary>
    /// <param name="position">This will be set to the current position of the zombie that needs to spawn a corpse.</param>
    /// <param name="corpseBlock">This will be set to the corpse block that is about to be spawned.</param>
    /// <returns>The new position that the zombie corpse should spawn at.</returns>
    public static Vector3 GetUpdatedPosition(Vector3 position, BlockValue corpseBlock)
    {
        try
        {
            Vector3i newPosition = POSITIONER.FindSpawnLocationStartingFrom(World.worldToBlockPos(position), corpseBlock);
            return new Vector3(newPosition.x, newPosition.y, newPosition.z);
        }
        catch (Exception e)
        {
            Debug.Log("Corpse Disintigration Fix: Uncaught exception: " + e.Message + ", in: " + e.TargetSite);
            Debug.Log("Corpse Disintigration Fix: Stack trace follows:");
            Debug.Log(e.StackTrace);
            if (CONFIG.DEBUG_MODE)
                throw e;
        }

        return position;
    }
}

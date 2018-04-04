using UnityEngine;

public static class ZombieCorpsePositionUpdater
{
    private static readonly Configuration CONFIG = new Configuration(Settings.Default.MAX_HEIGHT, Settings.Default.MIN_HEIGHT, Settings.Default.MAX_SEARCH_RADIUS, Settings.Default.CACHE_PERSISTANCE);

    private static readonly ZombieCorpsePositioner.IsGoreBlock isGoreBlock = location => GameManager.Instance.World.GetBlock(location).Block.BlockTag == BlockTags.Gore;
    private static readonly GroundFinder.IsMovementRestrictingBlock isMovementRestrictingBlock = location => GameManager.Instance.World.GetBlock(location).Block.IsCollideMovement;
    private static readonly ZombieCorpsePositioner positioner = new ZombieCorpsePositioner(msg => Debug.Log("Corpse Disintigration Fix: " + msg), isGoreBlock, new GroundFinder(CONFIG, isMovementRestrictingBlock, new GroundPositionCache(new CacheTimer(CONFIG.CACHE_PERSISTANCE, () => GameTimer.Instance.ticks))), CONFIG);

    public static Vector3 GetUpdatedPosition(Vector3 position)
    {
        Vector3i newPosition = positioner.FindSpawnLocationStartingFrom(World.worldToBlockPos(position));
        return new Vector3(newPosition.x, newPosition.y, newPosition.z);
    }
}
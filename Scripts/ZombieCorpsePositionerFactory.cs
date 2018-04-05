using UnityEngine;

/// <summary>
/// This is a simple factory class for generating a ZombieCorpsePositioner, that relies on access to real 7D2D code.
/// </summary>
public static class ZombieCorpsePositionerFactory
{
    /// <summary>
    /// Generates a new ZombieCorpsePositioner, that relies on real 7D2D code to perform its function.
    /// </summary>
    /// <returns>A new zombie corpse positioner.</returns>
    public static ZombieCorpsePositioner GenerateNewPositioner()
    {
        Configuration CONFIG = new Configuration(Settings.Default.MAX_HEIGHT, Settings.Default.MIN_HEIGHT, Settings.Default.MAX_SEARCH_RADIUS, Settings.Default.CACHE_PERSISTANCE);

        ZombieCorpsePositioner.IsGoreBlock isGoreBlock = location => GameManager.Instance.World.GetBlock(location).Block.BlockTag == BlockTags.Gore;
        GroundFinder.IsMovementRestrictingBlock isMovementRestrictingBlock = location => GameManager.Instance.World.GetBlock(location).Block.IsCollideMovement;
        ZombieCorpsePositioner.Logger log = msg => Debug.Log("Corpse Disintigration Fix: " + msg);

        ICacheTimer cacheTimer = new CacheTimer(CONFIG.CACHE_PERSISTANCE, () => GameTimer.Instance.ticks);
        GroundPositionCache cache = new GroundPositionCache(cacheTimer);
        IGroundFinder groundFinder = new GroundFinder(CONFIG, isMovementRestrictingBlock, cache);

        return new ZombieCorpsePositioner(log, isGoreBlock, groundFinder, CONFIG);
    }
}

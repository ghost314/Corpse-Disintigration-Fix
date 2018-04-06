using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a simple factory class for generating a ZombieCorpsePositioner, that relies on access to real 7D2D code.
/// </summary>
public static class ZombieCorpsePositionerFactory
{
    private const string DAMAGE_PROPERTY = "Damage";

    /// <summary>
    /// Generates a new ZombieCorpsePositioner, that relies on real 7D2D code to perform its function.
    /// </summary>
    /// <returns>A new zombie corpse positioner.</returns>
    public static ZombieCorpsePositioner GenerateNewPositioner()
    {
        Configuration CONFIG = new Configuration(Settings.Default.MAX_HEIGHT, Settings.Default.MIN_HEIGHT, Settings.Default.MAX_SEARCH_RADIUS, Settings.Default.CACHE_PERSISTANCE);

        GroundFinder.IsMovementRestrictingBlock isMovementRestrictingBlock = location => GetBlockAt(location).IsCollideMovement;
        ZombieCorpsePositioner.Logger log = msg => Debug.Log("Corpse Disintigration Fix: " + msg);

        ICacheTimer cacheTimer = new CacheTimer(CONFIG.CACHE_PERSISTANCE, () => GameTimer.Instance.ticks);
        GroundPositionCache cache = new GroundPositionCache(cacheTimer);
        IGroundFinder groundFinder = new GroundFinder(CONFIG, isMovementRestrictingBlock, cache);

        return new ZombieCorpsePositioner(log, IsStableBlock, IsValidSpawnPointForCorpseBlock, groundFinder, CONFIG);
    }

    private static bool IsStableBlock(Vector3i location)
    {
        Block block = GetBlockAt(location);
        return block.StabilitySupport && (Settings.Default.SPAWN_ON_SPIKES || !block.Properties.Values.ContainsKey(DAMAGE_PROPERTY));
    }

    private static Block GetBlockAt(Vector3i location)
    {
        return GetWorld().GetBlock(location).Block;
    }

    private static World GetWorld()
    {
        return GameManager.Instance.World;
    }

    private static bool IsValidSpawnPointForCorpseBlock(Vector3i location, BlockValue corpseBlock)
    {
        return Block.list[corpseBlock.type].CanPlaceBlockAt(GetWorld(), 0, location, corpseBlock, false);
    }
}

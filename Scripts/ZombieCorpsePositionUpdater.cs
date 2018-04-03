using UnityEngine;

public static class ZombieCorpsePositionUpdater
{
    private const int MAX_SEARCH_RADIUS = 200;
    private const int MAX_HEIGHT = 254;
    private const int MIN_HEIGHT = 3;
    private static readonly Configuration CONFIG = new Configuration(MAX_HEIGHT, MIN_HEIGHT, MAX_SEARCH_RADIUS);

    private static readonly ZombieCorpsePositioner.IsGoreBlock isGoreBlock = location => GameManager.Instance.World.GetBlock(location).Block.BlockTag == BlockTags.Gore;
    private static readonly GroundFinder.IsMovementRestrictingBlock isMovementRestrictingBlock = location => GameManager.Instance.World.GetBlock(location).Block.IsCollideMovement;
    private static readonly ZombieCorpsePositioner positioner = new ZombieCorpsePositioner(Debug.Log, isGoreBlock, new GroundFinder(CONFIG, isMovementRestrictingBlock), CONFIG);

    public static Vector3 GetUpdatedPosition(Vector3 position)
    {
        Vector3i newPosition = positioner.FindSpawnLocationStartingFrom(World.worldToBlockPos(position));
        return new Vector3(newPosition.x, newPosition.y, newPosition.z);
    }
}
using UnityEngine;

public static class ZombieCorpsePositionUpdater
{
    private const int MAX_SEARCH_RADIUS = 200;
    private const int MAX_HEIGHT = 254;
    private const int MIN_HEIGHT = 3;
    private static readonly Configuration CONFIG = new Configuration(MAX_HEIGHT, MIN_HEIGHT, MAX_SEARCH_RADIUS);

    private static ZombieCorpsePositioner.GetBlock blockFinder = location => new BlockWrapper(GameManager.Instance.World.GetBlock(location).Block);
    private static ZombieCorpsePositioner positioner = new ZombieCorpsePositioner(Debug.Log, blockFinder, new GroundFinder(CONFIG, blockFinder), CONFIG);

    public static Vector3 GetUpdatedPosition(Vector3 position)
    {
        Vector3i newPosition = positioner.FindSpawnLocationStartingFrom(World.worldToBlockPos(position));
        return new Vector3(newPosition.x, newPosition.y, newPosition.z);
    }
}
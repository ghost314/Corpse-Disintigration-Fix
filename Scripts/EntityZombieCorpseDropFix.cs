using UnityEngine;

public class EntityZombieCorpseDropFix : EntityZombie
{
    private const int MAX_SEARCH_RADIUS = 200;
    private const int MAX_HEIGHT = 254;
    private const int MIN_HEIGHT = 3;

    protected override Vector3i dropCorpseBlock()
    {
        if (lootContainer != null && lootContainer.IsUserAccessing())
        {
            return Vector3i.zero;
        }
        ZombieCorpsePositioner.GetBlock blockFinder = location => new BlockWrapper(world.GetBlock(location).Block);
        Configuration config = new Configuration(MAX_HEIGHT, MIN_HEIGHT, MAX_SEARCH_RADIUS);
        ZombieCorpsePositioner positioner = new ZombieCorpsePositioner(Debug.Log, blockFinder, new GroundFinder(config, blockFinder), config);
        Vector3i newPosition = positioner.FindSpawnLocationStartingFrom(World.worldToBlockPos(position));
        position = new Vector3(newPosition.x, newPosition.y, newPosition.z);
        return base.dropCorpseBlock();
    }
}
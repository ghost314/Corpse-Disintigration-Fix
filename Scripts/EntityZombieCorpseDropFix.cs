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
        ZombieCorpsePositioner positioner = new ZombieCorpsePositioner(Debug.Log, location => new BlockWrapper(world.GetBlock(location).Block));
        Vector3i newPosition = positioner.FindSpawnLocationStartingFrom(World.worldToBlockPos(position));
        this.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);
        return base.dropCorpseBlock();
    }
}
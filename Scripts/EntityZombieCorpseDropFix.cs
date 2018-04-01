using UnityEngine;

public class EntityZombieCorpseDropFix : EntityZombie
{
    private const int MAX_SEARCH_RADIUS = 200;

    protected override Vector3i dropCorpseBlock()
    {
        if (lootContainer != null)
        {
            if (lootContainer.IsUserAccessing())
            {
                return Vector3i.zero;
            }
        }
        Vector3i vector3i = FindAvailableSpawnLocation();
        this.position = new Vector3(vector3i.x, vector3i.y, vector3i.z);
        return base.dropCorpseBlock();
    }

    private Vector3i FindAvailableSpawnLocation()
    {
        Vector3i origin = World.worldToBlockPos(position);
        if (!HasGoreBlock(origin.x, origin.z))
            return origin;

        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int distanceFromOrigin = 1; distanceFromOrigin < MAX_SEARCH_RADIUS; distanceFromOrigin++)
        {
            nextPositionToCheck.z = origin.z + distanceFromOrigin;
            nextPositionToCheck.x = origin.x;
            Vector3i foundPosition = CheckRow(nextPositionToCheck, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            nextPositionToCheck.z = origin.z - distanceFromOrigin;
            nextPositionToCheck.x = origin.x;
            foundPosition = CheckRow(nextPositionToCheck, distanceFromOrigin);
            if (foundPosition != Vector3i.zero)
                return foundPosition;


            nextPositionToCheck.z = origin.z;
            nextPositionToCheck.x = origin.x + distanceFromOrigin;
            foundPosition = CheckColumn(nextPositionToCheck, distanceFromOrigin - 1);
            if (foundPosition != Vector3i.zero)
                return foundPosition;

            nextPositionToCheck.z = origin.z;
            nextPositionToCheck.x = origin.x - distanceFromOrigin;
            foundPosition = CheckRow(nextPositionToCheck, distanceFromOrigin - 1);
            if (foundPosition != Vector3i.zero)
                return foundPosition;
        }

        return origin;
    }

    private Vector3i CheckRow(Vector3i origin, int offset)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int xOffset = -1 * offset; xOffset <= offset; xOffset++)
        {
            nextPositionToCheck.x = origin.x + xOffset;
            if (!HasGoreBlock(nextPositionToCheck.x, nextPositionToCheck.z))
                return nextPositionToCheck;
        }
        return Vector3i.zero;
    }

    private Vector3i CheckColumn(Vector3i origin, int offset)
    {
        Vector3i nextPositionToCheck = new Vector3i(origin.x, origin.y, origin.z);
        for (int zOffset = -1 * offset; zOffset <= offset; zOffset++)
        {
            nextPositionToCheck.z = origin.z + zOffset;
            if (!HasGoreBlock(nextPositionToCheck.x, nextPositionToCheck.z))
                return nextPositionToCheck;
        }
        return Vector3i.zero;
    }

    private bool HasGoreBlock(int x, int z)
    {
        Vector3i location = new Vector3i(x, 0, z);
        Debug.Log("Checking for gore blocks at: " + x + "," + z);
        Debug.Log("Terrain height: " + world.GetTerrainHeight(x, z));
        for (int y = 0; y < 254; y++)
        {
            location.y = y;
            BlockValue block = world.GetBlock(location);
            if (block.Block.BlockTag == BlockTags.Gore)
                return true;
        }

        Debug.Log("No gore blocks at: " + x + "," + z);
        return false;
    }
}
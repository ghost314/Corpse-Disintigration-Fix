using System;
using UnityEngine;

public class BlockGoreFixed : BlockGore
{
    protected override void addTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        Debug.Log("Stack: " + System.Environment.StackTrace);
        Debug.Log("block position = " + _blockPos);
        TileEntityGoreBlock tileEntityGoreBlock = new TileEntityGoreBlock(_chunk)
        {
            localChunkPos = World.toBlock(_blockPos),
            lootListIndex = (int)((ushort)this.lootList)
        };
        Debug.Log("local chunk position = " + tileEntityGoreBlock.localChunkPos);
        tileEntityGoreBlock.SetContainerSize(LootContainer.lootList[this.lootList].size, true);
        _chunk.AddTileEntity(tileEntityGoreBlock);
    }
}

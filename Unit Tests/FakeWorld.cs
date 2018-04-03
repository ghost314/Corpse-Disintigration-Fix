class FakeWorld
{
    private FakeBlock[,,] fakeWorld;

    public FakeWorld(Configuration config)
    {
        fakeWorld = new FakeBlock[(config.MAX_SEARCH_RADIUS * 2) + 1, config.MAX_HEIGHT, (config.MAX_SEARCH_RADIUS * 2) + 1];
    }

    public void ResetWorld(int groundHeight)
    {
        for (int x = 0; x < fakeWorld.GetLength(0); x++)
        {
            for (int y = 0; y < fakeWorld.GetLength(1); y++)
            {
                for (int z = 0; z < fakeWorld.GetLength(2); z++)
                {
                    FakeBlock nextBlock;
                    if (y <= groundHeight)
                        nextBlock = GenerateOccupiedBlock();
                    else
                        nextBlock = GenerateEmptyBlock();
                    fakeWorld[x, y, z] = nextBlock;
                }
            }
        }
    }

    public FakeBlock GetBlockAt(Vector3i position)
    {
        return fakeWorld[position.x, position.y, position.z];
    }

    public void SetBlockAt(Vector3i position, FakeBlock block)
    {
        fakeWorld[position.x, position.y, position.z] = block;
    }

    public FakeBlock GenerateOccupiedBlock()
    {
        return new FakeBlock(BlockTags.None, true);
    }

    public FakeBlock GenerateGoreBlock()
    {
        return new FakeBlock(BlockTags.Gore, false);
    }

    public FakeBlock GenerateEmptyBlock()
    {
        return new FakeBlock(BlockTags.None, false);
    }

    public class FakeBlock
    {
        private BlockTags tag;
        private bool isCollideMovement;

        public FakeBlock(BlockTags tag, bool isCollideMovement)
        {
            this.tag = tag;
            this.isCollideMovement = isCollideMovement;
        }

        public BlockTags BlockTag => tag;

        public bool IsCollideMovement => isCollideMovement;
    }
}

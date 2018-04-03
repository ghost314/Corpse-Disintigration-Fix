class FakeWorld
{
    private IBlock[,,] fakeWorld;

    public FakeWorld(Configuration config)
    {
        fakeWorld = new IBlock[(config.MAX_SEARCH_RADIUS * 2) + 1, config.MAX_HEIGHT, (config.MAX_SEARCH_RADIUS * 2) + 1];
    }

    public void ResetWorld(int groundHeight)
    {
        for (int x = 0; x < fakeWorld.GetLength(0); x++)
        {
            for (int y = 0; y < fakeWorld.GetLength(1); y++)
            {
                for (int z = 0; z < fakeWorld.GetLength(2); z++)
                {
                    IBlock nextBlock;
                    if (y <= groundHeight)
                        nextBlock = GenerateOccupiedBlock();
                    else
                        nextBlock = GenerateEmptyBlock();
                    fakeWorld[x, y, z] = nextBlock;
                }
            }
        }
    }

    public IBlock GetBlockAt(Vector3i position)
    {
        return fakeWorld[position.x, position.y, position.z];
    }

    public void SetBlockAt(Vector3i position, IBlock block)
    {
        fakeWorld[position.x, position.y, position.z] = block;
    }

    public IBlock GenerateOccupiedBlock()
    {
        return new FakeBlock(BlockTags.None, true);
    }

    public IBlock GenerateGoreBlock()
    {
        return new FakeBlock(BlockTags.Gore, false);
    }

    public IBlock GenerateEmptyBlock()
    {
        return new FakeBlock(BlockTags.None, false);
    }

    private class FakeBlock : IBlock
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

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

    public FakeBlock GenerateOccupiedBlock(bool isStableBlock = true, bool isCollideMovement = true)
    {
        return new FakeBlock(isStableBlock, isCollideMovement, false);
    }

    public FakeBlock GenerateEmptyBlock(bool isStableBlock = true)
    {
        return new FakeBlock(isStableBlock, false, true);
    }

    public class FakeBlock
    {
        private bool isStableBlock;
        private bool isCollideMovement;
        private bool isValidSpawnPosition;

        public FakeBlock(bool isStableBlock, bool isCollideMovement, bool isValidSpawnPosition)
        {
            this.isStableBlock = isStableBlock;
            this.isCollideMovement = isCollideMovement;
            this.isValidSpawnPosition = isValidSpawnPosition;
        }

        public bool IsStableBlock => isStableBlock;

        public bool IsCollideMovement => isCollideMovement;

        public bool IsValidSpawnPosition => isValidSpawnPosition;
    }
}

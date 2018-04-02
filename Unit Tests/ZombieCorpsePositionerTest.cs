using NUnit.Framework;
using System;

[TestFixture]
public class ZombieCorpsePositionerTest
{
    private const int WORLD_HEIGHT = 300;
    private const int WORLD_LENGTH = 400;
    private const int GROUND_HEIGHT = 150;
    private ZombieCorpsePositioner positioner;
    private IBlock[,,] fakeWorld = new IBlock[WORLD_LENGTH, WORLD_HEIGHT, WORLD_LENGTH];

    [SetUp]
    public void SetUp()
    {
        positioner = new ZombieCorpsePositioner(Console.WriteLine, GetBlockAt);

        for (int x = 0; x < WORLD_LENGTH; x++)
        {
            for (int y = 0; y < WORLD_HEIGHT; y++)
            {
                for (int z = 0; z < WORLD_LENGTH; z++)
                {
                    IBlock nextBlock;
                    if (y <= GROUND_HEIGHT)
                        nextBlock = GenerateOccupiedBlock();
                    else
                        nextBlock = GenerateEmptyBlock();
                    fakeWorld[x, y, z] = nextBlock;
                }
            }
        }
    }

    [Test]
    public void WhenCurrentPositionIsAlreadyEmptyThenCorpseIsSpawnedAtCurrentPosition()
    {
        int height = GROUND_HEIGHT + 1;
        Vector3i startingPosition = new Vector3i(50, height, 50);

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, height, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionIsBelowGroundThenCorpseIsSpawnedAboveGround()
    {
        Vector3i startingPosition = new Vector3i(50, 25, 50);
        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, GROUND_HEIGHT + 1, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionIsMidAirThenCorpseIsSpawnedAtGround()
    {
        Vector3i startingPosition = new Vector3i(50, GROUND_HEIGHT + 50, 50);
        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, GROUND_HEIGHT + 1, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseThenCorpseIsSpawnedInAdjacentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(startingPosition, GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsAreGroundThenCorpseIsSpawnedInAdjacentPositionAboveGround()
    {
        int height = GROUND_HEIGHT;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(startingPosition, GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height + 1, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsAreAirThenCorpseIsSpawnedInAdjacentPositionAtGround()
    {
        int height = GROUND_HEIGHT + 2;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(startingPosition, GenerateGoreBlock());
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 1, startingPosition.z), GenerateOccupiedBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height - 1, position.y);
    }

    [Test]
    public void WhenCurrentPositionIsInBetweenTwoFloorsThenCorpseIsSpawnedAtCurrentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), GenerateEmptyBlock());
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), GenerateOccupiedBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, height, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionIsInBetweenTwoFloorsAndContainsCorpseThenCorpseIsSpawnedInAdjacentPositionOnSameFloor()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        for (int x = startingPosition.x - 1; x <= startingPosition.x + 1; x++)
        {
            for (int z = startingPosition.z - 1; z <= startingPosition.z + 1; z++)
            {
                SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), GenerateEmptyBlock());
                SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), GenerateOccupiedBlock());
            }
        }
        SetBlockAt(startingPosition, GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsAreGroundWithAirAboveAndBelowThenCorpseIsSpawnedInAdjacentPositionAboveGround()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(50, height, 50);

        for (int x = startingPosition.x - 1; x <= startingPosition.x + 1; x++)
        {
            for (int z = startingPosition.z - 1; z <= startingPosition.z + 1; z++)
            {
                if (x != 0 || z != 0)
                {
                    SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), GenerateEmptyBlock());
                    SetBlockAt(new Vector3i(x, startingPosition.y, z), GenerateOccupiedBlock());
                    SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), GenerateOccupiedBlock());
                }
            }
        }
        SetBlockAt(startingPosition, GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height + 2, position.y);
    }

    private IBlock GetBlockAt(Vector3i position)
    {
        return fakeWorld[position.x, position.y, position.z];
    }

    private void SetBlockAt(Vector3i position, IBlock block)
    {
        fakeWorld[position.x, position.y, position.z] = block;
    }

    private IBlock GenerateOccupiedBlock()
    {
        return new FakeBlock(BlockTags.None, true);
    }

    private IBlock GenerateGoreBlock()
    {
        return new FakeBlock(BlockTags.Gore, false);
    }

    private IBlock GenerateEmptyBlock()
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

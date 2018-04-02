using NUnit.Framework;
using System;

[TestFixture]
public class ZombieCorpsePositionerTest
{
    private const int WORLD_HEIGHT = 20;
    private const int WORLD_LENGTH = 10;
    private const int GROUND_HEIGHT = 10;
    private ZombieCorpsePositioner positioner;
    private IBlock[,,] fakeWorld = new IBlock[WORLD_LENGTH, WORLD_HEIGHT, WORLD_LENGTH];

    [SetUp]
    public void SetUp()
    {
        Configuration config = new Configuration(WORLD_HEIGHT, 0, 5);
        positioner = new ZombieCorpsePositioner(Console.WriteLine, GetBlockAt, new GroundFinder(config, GetBlockAt), config);

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
        Vector3i startingPosition = new Vector3i(5, height, 5);

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, height, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionIsBelowGroundThenCorpseIsSpawnedAboveGround()
    {
        Vector3i startingPosition = new Vector3i(5, 2, 5);
        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, GROUND_HEIGHT + 1, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionIsMidAirThenCorpseIsSpawnedAtGround()
    {
        Vector3i startingPosition = new Vector3i(5, GROUND_HEIGHT + 5, 5);
        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, GROUND_HEIGHT + 1, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseThenCorpseIsSpawnedInAdjacentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
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

        Vector3i startingPosition = new Vector3i(5, height, 5);
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

        Vector3i startingPosition = new Vector3i(5, height, 5);
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

        Vector3i startingPosition = new Vector3i(5, height, 5);
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), GenerateEmptyBlock());
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), GenerateOccupiedBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, height, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionIsInBetweenTwoFloorsAndContainsCorpseThenCorpseIsSpawnedInAdjacentPositionOnSameFloor()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
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

        Vector3i startingPosition = new Vector3i(5, height, 5);

        for (int x = startingPosition.x - 1; x <= startingPosition.x + 1; x++)
        {
            for (int z = startingPosition.z - 1; z <= startingPosition.z + 1; z++)
            {
                if (x != startingPosition.x || z != startingPosition.z)
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

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsAreAirWithGroundAboveAndBelowThenCorpseIsSpawnedInAdjacentPositionAtGround()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);

        for (int x = startingPosition.x - 1; x <= startingPosition.x + 1; x++)
        {
            for (int z = startingPosition.z - 1; z <= startingPosition.z + 1; z++)
            {
                if (x != startingPosition.x || z != startingPosition.z)
                {
                    SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), GenerateEmptyBlock());
                    SetBlockAt(new Vector3i(x, startingPosition.y - 1, z), GenerateEmptyBlock());
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
        Assert.AreEqual(height - 2, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsContainNoAirThenCorpseIsSpawnedFurtherAwayAtGround()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);

        for (int x = startingPosition.x - 1; x <= startingPosition.x + 1; x++)
        {
            for (int z = startingPosition.z - 1; z <= startingPosition.z + 1; z++)
            {
                if (x != startingPosition.x || z != startingPosition.z)
                {
                    for (int y = 0; y < WORLD_HEIGHT; y++)
                    {
                        SetBlockAt(new Vector3i(x, y, z), GenerateOccupiedBlock());
                    }
                }
            }
        }
        SetBlockAt(startingPosition, GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 2);
        Assert.LessOrEqual(deltaZ, 2);
        Assert.Greater(deltaX + deltaZ, 1);
        Assert.AreEqual(height, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAtDifferentAltitudesThenCorpseIsSpawnedAtCurrentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), GenerateEmptyBlock());
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 3, startingPosition.z), GenerateGoreBlock());
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), GenerateOccupiedBlock());
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 2, startingPosition.z), GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(startingPosition.x, height, startingPosition.z), position);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsContainCorpsesAtDifferentAltitudesThenCorpseIsSpawnedAtAppropriatePosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int x = startingPosition.x - 1; x <= startingPosition.x + 1; x++)
        {
            for (int z = startingPosition.z - 1; z <= startingPosition.z + 1; z++)
            {
                if (x != startingPosition.x || z != startingPosition.z)
                {
                    SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), GenerateEmptyBlock());
                    SetBlockAt(new Vector3i(x, startingPosition.y - 3, z), GenerateGoreBlock());
                    SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), GenerateOccupiedBlock());
                    SetBlockAt(new Vector3i(x, startingPosition.y + 2, z), GenerateGoreBlock());
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
        Assert.AreEqual(height, position.y);
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

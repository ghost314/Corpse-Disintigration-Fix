using NUnit.Framework;
using System;

[TestFixture]
public class SystemLevelTests
{
    private const int WORLD_HEIGHT = 20;
    private const int GROUND_HEIGHT = 10;
    private const int SEARCH_RADIUS = 5;
    private const int CACHE_PERSISTANCE = 1;
    private ulong tick;
    private ZombieCorpsePositioner positioner;
    private FakeWorld fakeWorld;

    [OneTimeSetUp]
    public void Init()
    {
        Configuration config = new Configuration(WORLD_HEIGHT, 0, SEARCH_RADIUS, CACHE_PERSISTANCE);
        fakeWorld = new FakeWorld(config);
        positioner = new ZombieCorpsePositioner(Console.WriteLine, location => fakeWorld.GetBlockAt(location).BlockTag != BlockTags.Gore, new GroundFinder(config, location => fakeWorld.GetBlockAt(location).IsCollideMovement, new GroundPositionCache(new CacheTimer(config.CACHE_PERSISTANCE, GetTick))), config);
    }

    private ulong GetTick()
    {
        return tick;
    }

    [SetUp]
    public void SetUp()
    {
        tick += CACHE_PERSISTANCE + 1;
        fakeWorld.ResetWorld(GROUND_HEIGHT);
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
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());

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
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());

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
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());

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
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());

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
                fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), fakeWorld.GenerateEmptyBlock());
                fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), fakeWorld.GenerateOccupiedBlock());
            }
        }
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());

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
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), fakeWorld.GenerateEmptyBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y, z), fakeWorld.GenerateOccupiedBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), fakeWorld.GenerateOccupiedBlock());
                }
            }
        }
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());

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
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), fakeWorld.GenerateEmptyBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y - 1, z), fakeWorld.GenerateEmptyBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), fakeWorld.GenerateOccupiedBlock());
                }
            }
        }
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());

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
                        fakeWorld.SetBlockAt(new Vector3i(x, y, z), fakeWorld.GenerateOccupiedBlock());
                    }
                }
            }
        }
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());

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
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 3, startingPosition.z), fakeWorld.GenerateGoreBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 2, startingPosition.z), fakeWorld.GenerateGoreBlock());

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
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y - 2, z), fakeWorld.GenerateEmptyBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y - 3, z), fakeWorld.GenerateGoreBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), fakeWorld.GenerateOccupiedBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y + 2, z), fakeWorld.GenerateGoreBlock());
                }
            }
        }
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height, position.y);
    }

    [Test]
    public void WhenNoGroundPositionsAreFoundThenCorpseIsSpawnedAtCurrentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int x = startingPosition.x - SEARCH_RADIUS; x <= startingPosition.x + SEARCH_RADIUS; x++)
        {
            for (int z = startingPosition.z - SEARCH_RADIUS; z <= startingPosition.z + SEARCH_RADIUS; z++)
            {
                for (int y = 0; y < WORLD_HEIGHT; y++)
                {
                    fakeWorld.SetBlockAt(new Vector3i(x, y, z), fakeWorld.GenerateOccupiedBlock());
                }
            }
        }

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(startingPosition, position);
    }

    [Test]
    public void WhenAllGroundPositionsContainCorpsesThenCorpseIsSpawnedAtCurrentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int x = startingPosition.x - SEARCH_RADIUS; x <= startingPosition.x + SEARCH_RADIUS; x++)
        {
            for (int z = startingPosition.z - SEARCH_RADIUS; z <= startingPosition.z + SEARCH_RADIUS; z++)
            {
                fakeWorld.SetBlockAt(new Vector3i(x, height, z), fakeWorld.GenerateGoreBlock());
            }
        }

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(startingPosition, position);
    }
}

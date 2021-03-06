﻿using NUnit.Framework;
using System;

[TestFixture]
public class ZombieCorpsePositionerTest
{
    private const int WORLD_HEIGHT = 20;
    private const int GROUND_HEIGHT = 10;
    private const int SEARCH_RADIUS = 5;
    private const int CACHE_PERSISTANCE = 1;
    private ZombieCorpsePositioner positioner;
    private FakeWorld fakeWorld;
    private FakeGroundFinder groundFinder;
    private BlockValue corpseBlock;

    [OneTimeSetUp]
    public void Init()
    {
        IConfiguration config = new BlockCorpseDisintigrationFixConfig(SEARCH_RADIUS, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false);
        corpseBlock = new BlockValue();
        fakeWorld = new FakeWorld(config);
        groundFinder = new FakeGroundFinder();
        positioner = new ZombieCorpsePositioner(Console.WriteLine, location => fakeWorld.GetBlockAt(location).IsStableBlock, (location, corpseBlock) => fakeWorld.GetBlockAt(location).IsValidSpawnPosition, groundFinder, config);
    }

    [SetUp]
    public void SetUp()
    {
        fakeWorld.ResetWorld(GROUND_HEIGHT);
        for (int x = 0; x < SEARCH_RADIUS * 2; x++)
        {
            for (int z = 0; z < SEARCH_RADIUS * 2; z++)
            {
                groundFinder.GroundPositions.Add(new Vector3i(x, GROUND_HEIGHT + 1, z));
            }
        }
    }

    [TearDown]
    public void TearDown()
    {
        groundFinder.GroundPositions.Clear();
    }

    [Test]
    public void WhenThereIsNoCorpseAtGroundPositionThenCorpseIsSpawnedAtGroundPosition()
    {
        Vector3i startingPosition = new Vector3i(5, 0, 5);

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition, corpseBlock);

        Assert.AreEqual(new Vector3i(startingPosition.x, GROUND_HEIGHT + 1, startingPosition.z), position);
    }

    [Test]
    public void WhenGroundPositionContainsCorpseThenCorpseIsSpawnedInAdjacentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateOccupiedBlock(true, false));

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition, corpseBlock);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height, position.y);
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
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateOccupiedBlock(true, false));

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition, corpseBlock);

        int deltaX = Math.Abs(position.x - startingPosition.x);
        int deltaZ = Math.Abs(position.z - startingPosition.z);
        Assert.LessOrEqual(deltaX, 2);
        Assert.LessOrEqual(deltaZ, 2);
        Assert.Greater(deltaX + deltaZ, 1);
        Assert.AreEqual(height, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpsesAtDifferentAltitudesAndGroundPositionIsEmptyThenCorpseIsSpawnedAtGroundPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 3, startingPosition.z), fakeWorld.GenerateOccupiedBlock(true, false));
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 2, startingPosition.z), fakeWorld.GenerateOccupiedBlock(true, false));

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition, corpseBlock);

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
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y - 3, z), fakeWorld.GenerateOccupiedBlock(true, false));
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y + 1, z), fakeWorld.GenerateOccupiedBlock());
                    fakeWorld.SetBlockAt(new Vector3i(x, startingPosition.y + 2, z), fakeWorld.GenerateOccupiedBlock(true, false));
                }
            }
        }
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateOccupiedBlock(true, false));

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition, corpseBlock);

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
        groundFinder.GroundPositions.Clear();
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateOccupiedBlock(true, false));

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition, corpseBlock);

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
                fakeWorld.SetBlockAt(new Vector3i(x, height, z), fakeWorld.GenerateOccupiedBlock(true, false));
            }
        }

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition, corpseBlock);

        Assert.AreEqual(startingPosition, position);
    }

    [Test]
    public void WhenLoggerIsNullThenExceptionIsThrown()
    {
        IConfiguration config = new BlockCorpseDisintigrationFixConfig(SEARCH_RADIUS, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false);
        Assert.That(() => new ZombieCorpsePositioner(null, location => fakeWorld.GetBlockAt(location).IsStableBlock, (location, corpseBlock) => fakeWorld.GetBlockAt(location).IsValidSpawnPosition, groundFinder, config), Throws.ArgumentNullException);
    }

    [Test]
    public void WhenBlockStabilityDelegateIsNullThenExceptionIsThrown()
    {
        IConfiguration config = new BlockCorpseDisintigrationFixConfig(SEARCH_RADIUS, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false);
        Assert.That(() => new ZombieCorpsePositioner(Console.WriteLine, null, (location, corpseBlock) => fakeWorld.GetBlockAt(location).IsValidSpawnPosition, groundFinder, config), Throws.ArgumentNullException);
    }

    [Test]
    public void WhenValidSpawnCheckDelegateIsNullThenExceptionIsThrown()
    {
        IConfiguration config = new BlockCorpseDisintigrationFixConfig(SEARCH_RADIUS, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false);
        Assert.That(() => new ZombieCorpsePositioner(Console.WriteLine, location => fakeWorld.GetBlockAt(location).IsStableBlock, null, groundFinder, config), Throws.ArgumentNullException);
    }

    [Test]
    public void WhenGroundFinderIsNullThenExceptionIsThrown()
    {
        IConfiguration config = new BlockCorpseDisintigrationFixConfig(SEARCH_RADIUS, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false);
        Assert.That(() => new ZombieCorpsePositioner(Console.WriteLine, location => fakeWorld.GetBlockAt(location).IsStableBlock, (location, corpseBlock) => fakeWorld.GetBlockAt(location).IsValidSpawnPosition, null, config), Throws.ArgumentNullException);
    }

    [Test]
    public void WhenConfigIsNullThenExceptionIsThrown()
    {
        Assert.That(() => new ZombieCorpsePositioner(Console.WriteLine, location => fakeWorld.GetBlockAt(location).IsStableBlock, (location, corpseBlock) => fakeWorld.GetBlockAt(location).IsValidSpawnPosition, groundFinder, null), Throws.ArgumentNullException);
    }
}

﻿using NUnit.Framework;

[TestFixture]
public class GroundFinderTest
{
    private const int WORLD_HEIGHT = 20;
    private const int GROUND_HEIGHT = 10;
    private const int CACHE_PERSISTANCE = 1;
    private FakeCacheTimer fakeTimer;
    private FakeWorld fakeWorld;
    private GroundFinder groundFinder;

    [OneTimeSetUp]
    public void Init()
    {
        fakeTimer = new FakeCacheTimer();
        fakeTimer.IsCacheValid = true;
        IConfiguration config = new BlockCorpseDisintigrationFixConfig(5, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false);
        fakeWorld = new FakeWorld(config);
        groundFinder = new GroundFinder(config, location => fakeWorld.GetBlockAt(location).IsCollideMovement, new GroundPositionCache(fakeTimer));
    }

    [SetUp]
    public void SetUp()
    {
        fakeTimer.IsCacheValid = false;
        fakeWorld.ResetWorld(GROUND_HEIGHT);
    }

    [Test]
    public void WhenCurrentPositionIsDirectlyAboveGroundThenCurrentPositionIsReturned()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsDirectlyAboveGroundAndThereArePlatformsAboveAndBelowThenCurrentPositionIsReturned()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsInsideGroundAndThereArePlatformsAboveAndBelowThenUpperPlatformIsReturned()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        fakeWorld.SetBlockAt(startingPosition, fakeWorld.GenerateOccupiedBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 1, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 2, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 3, startingPosition.z), fakeWorld.GenerateOccupiedBlock());

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height + 4, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsInsideAirAndThereArePlatformsAboveAndBelowThenLowerPlatformIsReturned()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 1, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 3, startingPosition.z), fakeWorld.GenerateEmptyBlock());

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height - 3, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsInsideGroundAndThereIsNoAirAboveThenSearchContinuesDownwards()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int y = height; y < WORLD_HEIGHT; y++)
        {
            fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, y, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        }
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 3, startingPosition.z), fakeWorld.GenerateEmptyBlock());

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height - 3, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsInsideAirAndThereIsNoGroundBelowThenSearchReturnsNegativeOne()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int y = height - 1; y >= 0; y--)
        {
            fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, y, startingPosition.z), fakeWorld.GenerateEmptyBlock());
        }
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), fakeWorld.GenerateOccupiedBlock());

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(-1, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsInsideGroundAndThereIsNoAirAboveOrBelowThenSearchReturnsNegativeOne()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int y = height; y < WORLD_HEIGHT; y++)
        {
            fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, y, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        }

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(-1, groundHeight);
    }

    [Test]
    public void WhenCacheIsValidThenCacheIsUsed()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);

        groundFinder.FindPositionAboveGroundAt(startingPosition);
        fakeWorld.ResetWorld(WORLD_HEIGHT);
        fakeTimer.IsCacheValid = true;

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsInsideGroundAndThereIsNoAirAboveThenResultsAreCachedForAllHigherPositions()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int y = height; y < WORLD_HEIGHT; y++)
        {
            fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, y, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        }
        fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 3, startingPosition.z), fakeWorld.GenerateEmptyBlock());

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height - 3, groundHeight);

        fakeTimer.IsCacheValid = true;
        startingPosition.y = WORLD_HEIGHT - 1;
        fakeWorld.ResetWorld(WORLD_HEIGHT);
        groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height - 3, groundHeight);
    }

    [Test]
    public void WhenCurrentPositionIsInsideGroundAndThereIsAirAboveThenResultsAreCachedForAllPositionsInBetween()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(5, height, 5);
        for (int y = height; y < height + 3; y++)
        {
            fakeWorld.SetBlockAt(new Vector3i(startingPosition.x, y, startingPosition.z), fakeWorld.GenerateOccupiedBlock());
        }

        int groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height + 3, groundHeight);

        fakeTimer.IsCacheValid = true;
        startingPosition.y = height + 1;
        fakeWorld.ResetWorld(WORLD_HEIGHT);
        groundHeight = groundFinder.FindPositionAboveGroundAt(startingPosition);

        Assert.AreEqual(height + 3, groundHeight);
    }

    [Test]
    public void WhenConfigurationIsNullThenExceptionIsThrown()
    {
        Assert.That(() => new GroundFinder(null, location => fakeWorld.GetBlockAt(location).IsCollideMovement, new GroundPositionCache(fakeTimer)), Throws.ArgumentNullException);
    }

    [Test]
    public void WhenDelegateIsNullThenExceptionIsThrown()
    {
        Assert.That(() => new GroundFinder(new BlockCorpseDisintigrationFixConfig(5, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false), null, new GroundPositionCache(fakeTimer)), Throws.ArgumentNullException);
    }

    [Test]
    public void WhenCacheIsNullThenExceptionIsThrown()
    {
        Assert.That(() => new GroundFinder(new BlockCorpseDisintigrationFixConfig(5, WORLD_HEIGHT, 0, CACHE_PERSISTANCE, true, false), location => fakeWorld.GetBlockAt(location).IsCollideMovement, null), Throws.ArgumentNullException);
    }
}

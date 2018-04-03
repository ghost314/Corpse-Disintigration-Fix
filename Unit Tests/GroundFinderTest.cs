using NUnit.Framework;


public class GroundFinderTest
{
    private const int WORLD_HEIGHT = 20;
    private const int GROUND_HEIGHT = 10;
    private FakeWorld fakeWorld;
    private GroundFinder groundFinder;

    [OneTimeSetUp]
    public void Init()
    {
        Configuration config = new Configuration(WORLD_HEIGHT, 0, 5);
        fakeWorld = new FakeWorld(config);
        groundFinder = new GroundFinder(config, fakeWorld.GetBlockAt);
    }

    [SetUp]
    public void SetUp()
    {
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
}

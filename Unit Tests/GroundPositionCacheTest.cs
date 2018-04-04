using NUnit.Framework;

public class GroundPositionCacheTest
{
    private GroundPositionCache cache = new GroundPositionCache();

    [SetUp]
    public void Setup()
    {
        cache.Clear();
    }

    [Test]
    public void WhenGroundPositionIsCachedForACertainLocationThenItCanBeRecalled()
    {
        int groundHeight = 27;
        Vector3i startingPosition = new Vector3i(2, 3, 7);

        cache.CacheGroundPositionForSingleLocation(startingPosition, groundHeight);

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeight));
    }

    [Test]
    public void WhenNoGroundPositionIsCachedForACertainLocationThenNoExceptionsAreThrown()
    {
        Vector3i startingPosition = new Vector3i(2, 3, 7);

        int cachedHeight = 27;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.False);
        Assert.That(cachedHeight, Is.EqualTo(0));
    }

    [Test]
    public void WhenCacheIsClearedThenStoredValuesAreLost()
    {
        int groundHeight = 27;
        Vector3i startingPosition = new Vector3i(2, 3, 7);

        cache.CacheGroundPositionForSingleLocation(startingPosition, groundHeight);
        cache.Clear();

        int cachedHeight = 27;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.False);
        Assert.That(cachedHeight, Is.EqualTo(0));
    }

    [Test]
    public void WhenGroundPositionIsCachedForACertainLocationTwiceThenOldValueIsOverwritten()
    {
        int groundHeight = 36;
        Vector3i startingPosition = new Vector3i(2, 3, 7);

        cache.CacheGroundPositionForSingleLocation(startingPosition, 27);
        cache.CacheGroundPositionForSingleLocation(startingPosition, groundHeight);

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeight));
    }

    [Test]
    public void WhenGroundPositionIsCachedForACertainLocationThenAnyLocationInBetweenHasSameGroundPosition([Range(3, 10)] int queryHeight)
    {
        int groundHeight = 10;
        Vector3i startingPosition = new Vector3i(2, 3, 7);

        cache.CacheGroundPositionForSingleLocation(startingPosition, groundHeight);
        startingPosition.y = queryHeight;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeight));
    }
    
    [Test]
    public void WhenGroundPositionIsCachedForAHigherUpLocationThenAnyLocationInBetweenHasSameGroundPosition([Range(10, 3)] int queryHeight)
    {
        int groundHeight = 3;
        Vector3i startingPosition = new Vector3i(2, 10, 7);

        cache.CacheGroundPositionForSingleLocation(startingPosition, groundHeight);
        startingPosition.y = queryHeight;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeight));
    }

    [Test]
    public void WhenGroundPositionIsCachedAsNonExistantThenAnyLocationWithSameXAndZHasNoGroundPosition([Range(0, 10)] int queryHeight)
    {
        int groundHeight = -1;
        Vector3i startingPosition = new Vector3i(2, 1, 7);

        cache.CacheGroundPositionForSingleLocation(startingPosition, groundHeight);
        startingPosition.y = queryHeight;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeight));
    }

    [Test]
    public void WhenGroundPositionIsCachedForOneLocationThenItIsNotCachedForAllLocationsWithSameXAndZ([Range(6, 16)] int queryHeight)
    {
        int groundHeight = 5;
        Vector3i startingPosition = new Vector3i(2, 1, 7);

        cache.CacheGroundPositionForSingleLocation(startingPosition, groundHeight);
        startingPosition.y = queryHeight;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPosition, out cachedHeight);
        Assert.That(valueExists, Is.False);
        Assert.That(cachedHeight, Is.EqualTo(0));
    }

    [Test]
    public void WhenGroundPositionIsCachedForTwoLocationsWithSameXAndZThenBothCanBeRetrieved()
    {
        int groundHeightOne = 5;
        int groundHeightTwo = 16;
        Vector3i startingPositionOne = new Vector3i(2, 1, 7);
        Vector3i startingPositionTwo = new Vector3i(2, 25, 7);

        cache.CacheGroundPositionForSingleLocation(startingPositionOne, groundHeightOne);
        cache.CacheGroundPositionForSingleLocation(startingPositionTwo, groundHeightTwo);
        startingPositionOne.y = 3;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPositionOne, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightOne));

        startingPositionTwo.y = 20;

        cachedHeight = -1;
        valueExists = cache.GetGroundPositionFor(startingPositionTwo, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightTwo));
    }

    [Test]
    public void WhenGroundPositionIsCachedForTwoLocationsWithDifferentXAndZThenBothCanBeRetrieved()
    {
        int groundHeightOne = 5;
        int groundHeightTwo = 16;
        Vector3i startingPositionOne = new Vector3i(5, 6, 7);
        Vector3i startingPositionTwo = new Vector3i(2, 10, 8);

        cache.CacheGroundPositionForSingleLocation(startingPositionOne, groundHeightOne);
        cache.CacheGroundPositionForSingleLocation(startingPositionTwo, groundHeightTwo);
        startingPositionOne.y = 5;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPositionOne, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightOne));

        startingPositionTwo.y = 12;

        cachedHeight = -1;
        valueExists = cache.GetGroundPositionFor(startingPositionTwo, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightTwo));
    }

    [Test]
    public void WhenGroundPositionIsCachedForTwoLocationsWithDifferentXAndZButSameYThenBothCanBeRetrieved()
    {
        int groundHeightOne = 5;
        int groundHeightTwo = 16;
        Vector3i startingPositionOne = new Vector3i(5, 6, 7);
        Vector3i startingPositionTwo = new Vector3i(2, 6, 8);

        cache.CacheGroundPositionForSingleLocation(startingPositionOne, groundHeightOne);
        cache.CacheGroundPositionForSingleLocation(startingPositionTwo, groundHeightTwo);
        startingPositionOne.y = 6;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPositionOne, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightOne));

        startingPositionTwo.y = 6;

        cachedHeight = -1;
        valueExists = cache.GetGroundPositionFor(startingPositionTwo, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightTwo));
    }

    [Test]
    public void WhenGroundPositionIsCachedForTwoLocationsWithDifferentXThenBothCanBeRetrieved()
    {
        int groundHeightOne = 10;
        int groundHeightTwo = 16;
        Vector3i startingPositionOne = new Vector3i(5, 6, 7);
        Vector3i startingPositionTwo = new Vector3i(2, 6, 7);

        cache.CacheGroundPositionForSingleLocation(startingPositionOne, groundHeightOne);
        cache.CacheGroundPositionForSingleLocation(startingPositionTwo, groundHeightTwo);
        startingPositionOne.y = 8;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPositionOne, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightOne));

        startingPositionTwo.y = 8;

        cachedHeight = -1;
        valueExists = cache.GetGroundPositionFor(startingPositionTwo, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightTwo));
    }

    [Test]
    public void WhenGroundPositionIsCachedForTwoLocationsWithDifferentZThenBothCanBeRetrieved()
    {
        int groundHeightOne = 10;
        int groundHeightTwo = 16;
        Vector3i startingPositionOne = new Vector3i(5, 6, 2);
        Vector3i startingPositionTwo = new Vector3i(5, 6, 7);

        cache.CacheGroundPositionForSingleLocation(startingPositionOne, groundHeightOne);
        cache.CacheGroundPositionForSingleLocation(startingPositionTwo, groundHeightTwo);
        startingPositionOne.y = 8;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPositionOne, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightOne));

        startingPositionTwo.y = 8;

        cachedHeight = -1;
        valueExists = cache.GetGroundPositionFor(startingPositionTwo, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeightTwo));
    }

    [Test]
    public void WhenGroundPositionIsCachedForTwoLocationsAboveAndBelowGroundThenBothLocationsHaveSameGroundLevel()
    {
        int groundHeight = 10;
        Vector3i startingPositionOne = new Vector3i(5, 6, 2);
        Vector3i startingPositionTwo = new Vector3i(5, 15, 2);

        cache.CacheGroundPositionForSingleLocation(startingPositionOne, groundHeight);
        cache.CacheGroundPositionForSingleLocation(startingPositionTwo, groundHeight);
        startingPositionOne.y = 8;

        int cachedHeight = -1;
        bool valueExists = cache.GetGroundPositionFor(startingPositionOne, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeight));

        startingPositionTwo.y = 12;

        cachedHeight = -1;
        valueExists = cache.GetGroundPositionFor(startingPositionTwo, out cachedHeight);
        Assert.That(valueExists, Is.True);
        Assert.That(cachedHeight, Is.EqualTo(groundHeight));
    }
}

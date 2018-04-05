using NUnit.Framework;

[TestFixture]
class CacheTimerTest
{
    private const uint CACHE_PERSISTANCE = 5;
    private ulong tick;
    private ICacheTimer cacheTimer;

    private ulong GetTick()
    {
        return tick;
    }

    [SetUp]
    public void Setup()
    {
        tick = 0;
        cacheTimer = new CacheTimer(CACHE_PERSISTANCE, GetTick);
    }

    [Test]
    public void WhenTickIsLessThanCachePersistanceThenCacheIsStillValid()
    {
        Assert.That(cacheTimer.IsCacheStillValid(), Is.True);
    }

    [Test]
    public void WhenTickIsOneLessThanCachePersistanceThenCacheIsStillValid()
    {
        tick = CACHE_PERSISTANCE - 1;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.True);
    }

    [Test]
    public void WhenValidCacheIsCheckedMultipleTimesAndTickDoesNotChangeThenCacheRemainsValid()
    {
        tick = CACHE_PERSISTANCE - 1;

        for (int i = 0; i < 10; i++)
            Assert.That(cacheTimer.IsCacheStillValid(), Is.True);
    }

    [Test]
    public void WhenTickIsEqualToCachePersistanceThenCacheIsNotValid()
    {
        tick = CACHE_PERSISTANCE;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);
    }

    [Test]
    public void WhenTickIsGreaterThanCachePersistanceThenCacheIsNotValid()
    {
        tick = ulong.MaxValue;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);
    }

    [Test]
    public void WhenCacheIsResetAndTickStopsChangingThenCacheRemainsValid()
    {
        tick = CACHE_PERSISTANCE;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);

        for (int i = 0; i < 10; i++)
            Assert.That(cacheTimer.IsCacheStillValid(), Is.True);
    }

    [Test]
    public void WhenTickContinuesIncreasingThenCacheIsPeriodicallyReset()
    {
        tick = CACHE_PERSISTANCE;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);

        tick += CACHE_PERSISTANCE;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);
    }

    [Test]
    public void WhenCacheIsResetAndTickIncrementsByOneLessThanCachePersistanceThenCacheIsStillValid()
    {
        tick = CACHE_PERSISTANCE;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);

        tick += CACHE_PERSISTANCE - 1;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.True);
    }

    [Test]
    public void WhenTickCounterOverflowsByAmountEqualToCachePersistanceThenCacheIsNotValid()
    {
        tick = ulong.MaxValue;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);

        tick += CACHE_PERSISTANCE;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);
    }

    [Test]
    public void WhenTickCounterOverflowsByAmountLessThanCachePersistanceThenCacheIsStillValid()
    {
        tick = ulong.MaxValue;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.False);

        tick += CACHE_PERSISTANCE - 1;

        Assert.That(cacheTimer.IsCacheStillValid(), Is.True);
    }

    [Test]
    public void WhenTimerIsNullThenExceptionIsThrown()
    {
        Assert.That(() => new CacheTimer(CACHE_PERSISTANCE, null), Throws.ArgumentNullException);
    }
}

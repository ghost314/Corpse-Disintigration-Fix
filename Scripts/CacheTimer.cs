using System;

/// <summary>
/// This implementation of ICacheTimer, relies on a supplied delegate function, to retrieve the current time.
/// </summary>
public class CacheTimer : ICacheTimer
{
    /// <summary>
    /// This delegate is used to retrieve the current 'time elapsed'.
    /// </summary>
    /// <returns>The total time elapsed, ad a ulong.</returns>
    public delegate ulong Timer();
    private readonly uint CACHE_PERSISTANCE;
    private readonly Timer timer;
    private ulong lastResetTime;

    /// <summary>
    /// Creates a new timer for managing some external data cache.
    /// </summary>
    /// <param name="cachePersistance">This value represents how much time must have elapsed since the last time the cache was refreshed, before it needs to be refreshed again.</param>
    /// <param name="timer">This delegate method will be used to track time elapsed, to determine when to refresh the cache.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="timer"/> is null.</exception>
    public CacheTimer(uint cachePersistance, Timer timer)
    {
        CACHE_PERSISTANCE = cachePersistance;
        if (timer == null)
            throw new ArgumentNullException("timer", "The given timer must not be null");
        this.timer = timer;
        lastResetTime = 0;
    }

    /// <summary>
    /// Checks whether or not the cache needs to be refreshed again, based on time elapsed since the last time this method returned false.
    /// </summary>
    /// <returns>True if the cache does <i>not</i> need to be refreshed, false otherwise.</returns>
    public bool IsCacheStillValid()
    {
        ulong time = timer();
        if (time - lastResetTime < CACHE_PERSISTANCE)
            return true;
        lastResetTime = time;
        return false;
    }
}

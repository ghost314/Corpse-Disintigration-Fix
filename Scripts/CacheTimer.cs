public class CacheTimer : ICacheTimer
{
    public delegate ulong Timer();
    private readonly uint CACHE_PERSISTANCE;
    private readonly Timer timer;
    private ulong lastResetTime;

    public CacheTimer(uint cachePersistance, Timer timer)
    {
        CACHE_PERSISTANCE = cachePersistance;
        this.timer = timer;
        lastResetTime = 0;
    }

    public bool IsCacheStillValid()
    {
        ulong time = timer();
        if(time - lastResetTime < CACHE_PERSISTANCE)
            return true;
        lastResetTime = time;
        return false;
    }
}

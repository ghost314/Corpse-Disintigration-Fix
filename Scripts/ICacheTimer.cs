/// <summary>
/// This interface denotes a timer mechanism that can be used to keep track of when a cache should refresh its data.
/// </summary>
public interface ICacheTimer
{
    /// <summary>
    /// Checks whether or not the cache needs to be refreshed again, based on time elapsed since the last time this method returned false.
    /// </summary>
    /// <returns>True if the cache does <i>not</i> need to be refreshed, false otherwise.</returns>
    bool IsCacheStillValid();
}

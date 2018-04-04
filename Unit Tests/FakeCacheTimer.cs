class FakeCacheTimer : ICacheTimer
{
    private bool isCacheValid = true;

    public bool IsCacheValid
    {
        get { return isCacheValid; }
        set
        {
            isCacheValid = value;
        }
    }

    public bool IsCacheStillValid()
    {
        return IsCacheValid;
    }
}

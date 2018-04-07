/// <summary>
/// This class defines the configuration data used by the rest of the mod.
/// </summary>
public class BlockCorpseDisintigrationFixConfig : Block, IConfiguration
{
    private const string BLOCK_TYPE_NAME = "CorpseDisintigrationFixConfig";

    private readonly int maxSearchRadius;
    private readonly int maxHeight;
    private readonly int minHeight;
    private readonly uint cachePersistance;
    private readonly bool debugMode;
    private readonly bool spawnOnSpikes;

    /// <summary>
    /// Creates a new configuration, with dummy config values. This constructor only exists to facilitate automatic loading from 7D2D code.
    /// </summary>
    public BlockCorpseDisintigrationFixConfig() : this(0, 0, 0, 0, false, false)
    {
    }

    /// <summary>
    /// Creates a new configuration, based on the provided parameters.
    /// </summary>
    /// <param name="maxSearchRadius">The farthest distance from the killed zombie, that the corpse positioner algorithm should ever try to scan.</param>
    /// <param name="maxHeight">The highest point that the corpse positioner algorithm should ever try to scan.</param>
    /// <param name="minHeight">The lowest point that the corpse positioner algorithm should ever try to scan.</param>
    /// <param name="cachePersistance">How long the corpse positioner algorithm should remember scanned blocks, before resetting.</param>
    /// <param name="debugMode">Indicates if the mod is currently running in debug mode.</param>
    /// <param name="spawnOnSpikes">Indicates if the corpse positioner algorithm should avoid spawning corpses on top of spikes.</param>
    public BlockCorpseDisintigrationFixConfig(int maxSearchRadius, int maxHeight, int minHeight, uint cachePersistance, bool debugMode, bool spawnOnSpikes)
    {
        this.maxSearchRadius = maxSearchRadius;
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        this.cachePersistance = cachePersistance;
        this.debugMode = debugMode;
        this.spawnOnSpikes = spawnOnSpikes;
    }

    /// <summary>
    /// The farthest distance from the killed zombie, that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    public int MAX_SEARCH_RADIUS
    {
        get
        {
            return maxSearchRadius;
        }
    }

    /// <summary>
    /// The highest point that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    public int MAX_HEIGHT
    {
        get
        {
            return maxHeight;
        }
    }

    /// <summary>
    /// The lowest point that the corpse positioner algorithm should ever try to scan.
    /// </summary>
    public int MIN_HEIGHT
    {
        get
        {
            return minHeight;
        }
    }

    /// <summary>
    /// How long the corpse positioner algorithm should remember scanned blocks, before resetting.
    /// </summary>
    public uint CACHE_PERSISTANCE
    {
        get
        {
            return cachePersistance;
        }
    }

    /// <summary>
    /// Indicates if the mod is currently running in debug mode.
    /// </summary>
    public bool DEBUG_MODE
    {
        get
        {
            return debugMode;
        }
    }

    /// <summary>
    /// Indicates if the corpse positioner algorithm should avoid spawning corpses on top of spikes.
    /// </summary>
    public bool SPAWN_ON_SPIKES
    {
        get
        {
            return spawnOnSpikes;
        }
    }

    /// <summary>
    /// Retrieves the instance of this class that was created by the core 7D2D code, when it loaded in the blocks from blocks.xml
    /// </summary>
    /// <returns>An instance of this class, with config data loaded from blocks.xml</returns>
    public static IConfiguration GetLoadedInstance()
    {
        Block loadedBlock = GetBlockValue(BLOCK_TYPE_NAME).Block;

        int maxSearchRadius = loadedBlock.Properties.GetInt("MAX_SEARCH_RADIUS");
        int maxHeight = loadedBlock.Properties.GetInt("MAX_HEIGHT");
        int minHeight = loadedBlock.Properties.GetInt("MIN_HEIGHT");
        uint cachePersistance = (uint)loadedBlock.Properties.GetInt("CACHE_PERSISTANCE");
        bool debugMode = loadedBlock.Properties.GetBool("DEBUG_MODE");
        bool spawnOnSpikes = loadedBlock.Properties.GetBool("SPAWN_ON_SPIKES");

        return new BlockCorpseDisintigrationFixConfig(maxSearchRadius, maxHeight, minHeight, cachePersistance, debugMode, spawnOnSpikes);
    }
}

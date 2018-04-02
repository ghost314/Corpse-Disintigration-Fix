public class BlockWrapper : IBlock
{
    private Block block;

    public BlockWrapper(Block block)
    {
        this.block = block;
    }

    public BlockTags BlockTag
    {
        get
        {
            return block.BlockTag;
        }
    }

    public bool IsCollideMovement
    {
        get
        {
            return block.IsCollideMovement;
        }
    }
}

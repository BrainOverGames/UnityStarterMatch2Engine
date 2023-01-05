namespace BOG
{
    /// <summary>
    /// Base class for tile on grid
    /// </summary>
    [System.Serializable]
    public class LevelTile
    {
        public BlockerType blockerType;
    }

    /// <summary>
    /// block tile on grid
    /// </summary>
    [System.Serializable]
    public class BlockTile : LevelTile
    {
        public BlockType type;
    }
}

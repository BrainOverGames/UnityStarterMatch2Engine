using System.Collections.Generic;

namespace BOG
{
    /// <summary>
    /// Data container of game state
    /// </summary>
    [System.Serializable]
    public class GameStateData
    {
        public int levelId;
        public LimitType limitType;
        public int limit;
        public int penalty;
        public List<CollectedBlockData> collectedBlocksList = new List<CollectedBlockData>();

        public Dictionary<BlockType, int> collectedBlocksDic = new Dictionary<BlockType, int>();
    }

    /// <summary>
    /// Data container of blocks that are collected
    /// </summary>
    [System.Serializable]
    public class CollectedBlockData
    {
        public BlockType blockType;
        public int amountCollected;
    }
}

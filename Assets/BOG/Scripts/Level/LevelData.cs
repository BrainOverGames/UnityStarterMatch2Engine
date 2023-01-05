using System.Collections.Generic;
using NaughtyAttributes;

namespace BOG
{
    /// <summary>
    /// types of limits
    /// </summary>
    public enum LimitType
    {
        Moves,
    }

    /// <summary>
    /// data container of each level
    /// </summary>
    [System.Serializable]
    public class LevelData
    {
        public int id;

        public int width;
        public int height;
        [InfoBox("ONLY for GenerateRandomBlockTiles", EInfoBoxType.Warning)]
        public List<BlockTile> tiles = new List<BlockTile>();

        public LimitType limitType;
        public int limit;
        public int penalty;

        public List<CollectBlockGoal> goals = new List<CollectBlockGoal>();
        public List<ColorBlockType> availableColors = new List<ColorBlockType>();
        public bool awardBoostersWithRemainingMoves;
    }
}

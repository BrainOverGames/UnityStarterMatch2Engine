using System;
using System.Collections.Generic;

namespace BOG
{
    /// <summary>
    ///  This class stores the state of a game in game scene at a given point in time.
    /// </summary>
    public class GameState
    {
        public Dictionary<BlockType, int> collectedBlocks = new Dictionary<BlockType, int>();

        public void Reset()
        {
            collectedBlocks.Clear();
            foreach (var value in Enum.GetValues(typeof(BlockType)))
            {
                collectedBlocks.Add((BlockType)value, 0);
            }
        }
    }
}

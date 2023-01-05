using System.Collections.Generic;
using System.Linq;

namespace BOG
{
    /// <summary>
    /// Utility conversion class for blocks
    /// </summary>
    internal static class BlockConversionUtils
    {
        internal static List<CollectedBlockData> DicToListConversion(Dictionary<BlockType, int> blocksDic)
        {
            List<CollectedBlockData> collectedBlocksList = new List<CollectedBlockData>();
            for (int i = 0; i < blocksDic.Count; i++)
            {
                collectedBlocksList.Add(new CollectedBlockData()
                {
                    blockType = blocksDic.Keys.ElementAt(i),
                    amountCollected = blocksDic.Values.ElementAt(i)
                });
            }
            return collectedBlocksList;
        }

        internal static Dictionary<BlockType, int> ListToDicConversion(List<CollectedBlockData> blocksList)
        {
            Dictionary<BlockType, int> collectedBlocksDic = new Dictionary<BlockType, int>();
            for(int i = 0; i < blocksList.Count; i++)
            {
                collectedBlocksDic.Add(blocksList[i].blockType, blocksList[i].amountCollected);
            }
            return collectedBlocksDic;
        }
    }
}

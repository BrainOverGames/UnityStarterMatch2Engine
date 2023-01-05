namespace BOG
{
    /// <summary>
    /// Base grid's goal class
    /// </summary>
    public abstract class Goal
    {
        public abstract bool IsComplete(GameState state);
    }

    /// <summary>
    /// collected block grid's goal class
    /// </summary>
    [System.Serializable]
    public class CollectBlockGoal : Goal
    {
        public BlockType blockType;
        public int amount;

        public override bool IsComplete(GameState state)
        {
            return state.collectedBlocks[blockType] >= amount;
        }

        public override string ToString()
        {
            return "Collect " + amount + " " + blockType;
        }
    }
}

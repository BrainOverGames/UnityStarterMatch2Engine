using UnityEngine;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle goal UI functionality
    /// </summary>
    public class GoalUi : MonoBehaviour
    {
        [SerializeField]
        public HorizontalLayoutGroup group;

        public void UpdateGoals(GameState state)
        {
            foreach (var element in group.GetComponentsInChildren<GoalUiElement>())
            {
                element.UpdateGoal(state);
            }
        }
    }
}

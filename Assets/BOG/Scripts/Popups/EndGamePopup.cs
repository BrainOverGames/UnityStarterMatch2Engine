using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle popup functionality when game has ended
    /// </summary>
    public class EndGamePopup : Popup
    {
        [SerializeField]
        private Text levelText;

        [SerializeField]
        private GameObject goalGroup;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(levelText);
            Assert.IsNotNull(goalGroup);
        }

        public void OnReplayButtonPressed()
        {
            var gameScene = parentScene as GameSceneHandler;
            if (gameScene != null)
            {
                gameScene.RestartGame();
                Close();
            }
        }

        public void SetLevel(int level)
        {
            levelText.text = "Level " + level;
        }

        public void SetGoals(GameObject group)
        {
            foreach (var goal in group.GetComponentsInChildren<GoalUiElement>())
            {
                var goalObject = Instantiate(goal);
                goalObject.transform.SetParent(goalGroup.transform, false);
                goalObject.GetComponent<GoalUiElement>().SetCompletedTick(goal.isCompleted);
            }
        }
    }
}

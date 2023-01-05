using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle popup whenever level game has been started
    /// </summary>
    public class StartGamePopup : Popup
    {
        [SerializeField]
        private Text levelText;
        [SerializeField]
        private GameObject goalPrefab;
        [SerializeField]
        private GameObject goalGroup;
        [SerializeField]
        private Text goalText;
        [SerializeField]
        private Text scoreGoalTitleText;
        [SerializeField]
        private Text scoreGoalAmountText;

        private int numLevel;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(levelText);
            Assert.IsNotNull(goalPrefab);
            Assert.IsNotNull(goalGroup);
            Assert.IsNotNull(goalText);
            Assert.IsNotNull(scoreGoalTitleText);
            Assert.IsNotNull(scoreGoalAmountText);
        }

        public void LoadLevelData(int levelNum)
        {
            numLevel = levelNum;

            string loadPath = "Levels/Level" + numLevel;
            var level = FileUtils.LoadJsonFile<LevelData>(loadPath);
            levelText.text = "Level " + numLevel;
            foreach (var goal in level.goals)
            {
                if (goal is CollectBlockGoal)
                {
                    var goalObject = Instantiate(goalPrefab);
                    goalObject.transform.SetParent(goalGroup.transform, false);
                    goalObject.GetComponent<GoalUiElement>().Fill(goal);
                }
            }
            scoreGoalTitleText.gameObject.SetActive(false);
            scoreGoalAmountText.gameObject.SetActive(false);
        }

        public void OnPlayButtonPressed()
        {
            GameManager.Instance.lastSelectedLevel = numLevel;
            GetComponent<SceneTransition>().PerformTransition();
        }

        public void OnCloseButtonPressed()
        {
            Close();
        }
    }
}

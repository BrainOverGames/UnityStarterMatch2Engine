using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BOG
{
    /// <summary>
    /// popup handling level goals related functionality
    /// </summary>
    public class LevelGoalsPopup : Popup
    {
        [SerializeField]
        private GameObject goalGroup;

        [SerializeField]
        private GameObject goalPrefab;

        [SerializeField]
        private GameObject goalHeadline;

        [SerializeField]
        private GameObject scoreGoalHeadline;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(goalGroup);
            Assert.IsNotNull(goalPrefab);
            Assert.IsNotNull(goalHeadline);
            Assert.IsNotNull(scoreGoalHeadline);
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(AutoKill());
        }

        private IEnumerator AutoKill()
        {
            yield return new WaitForSeconds(2.4f);
            Close();
            var gameScene = parentScene as GameSceneHandler;
            if (gameScene != null)
            {
                gameScene.StartGame();
            }
        }

        public void SetGoals(List<CollectBlockGoal> goals)
        {
            foreach (var goal in goals)
            {
                if (goal is CollectBlockGoal)
                {
                    var goalObject = Instantiate(goalPrefab);
                    goalObject.transform.SetParent(goalGroup.transform, false);
                    goalObject.GetComponent<GoalUiElement>().Fill(goal);
                }
            }
            scoreGoalHeadline.SetActive(false);
        }
    }
}

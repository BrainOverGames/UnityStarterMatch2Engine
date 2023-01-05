using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle game UI functionality of level's grid
    /// </summary>
    public class GameUi : MonoBehaviour
    {
        public Text limitTitleText;
        public Text limitText;
        public GoalUi goalUi;
        [SerializeField]
        private GameObject goalHeadline;
        [SerializeField]
        private GameObject scoreGoalHeadline;

        private void Awake()
        {
            Assert.IsNotNull(goalHeadline);
            Assert.IsNotNull(scoreGoalHeadline);
        }

        private void Start()
        {
            goalHeadline.SetActive(false);
            scoreGoalHeadline.SetActive(false);
        }

        public void SetGoals(List<CollectBlockGoal> goals, GameObject itemGoalPrefab)
        {
            var childrenToRemove = goalUi.group.GetComponentsInChildren<GoalUiElement>().ToList();
            foreach (var child in childrenToRemove)
            {
                Destroy(child.gameObject);
            }

            foreach (var goal in goals)
            {
                if (!(goal is CollectBlockGoal))
                {
                    continue;
                }
                var goalObject = Instantiate(itemGoalPrefab);
                goalObject.transform.SetParent(goalUi.group.transform, false);
                goalObject.GetComponent<GoalUiElement>().Fill(goal);
            }
            scoreGoalHeadline.SetActive(false);
            goalHeadline.SetActive(true);
        }
    }
}

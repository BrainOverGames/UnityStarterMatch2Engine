using UnityEngine;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle goal UI functionality of each element
    /// </summary>
    public class GoalUiElement : MonoBehaviour
    {
        public Image image;
        public Text amountText;
        public Image tickImage;
        public Image crossImage;

        public bool isCompleted { get; private set; }

        private Goal currentGoal;
        private int targetAmount;
        private int currentAmount;

        private void Awake()
        {
            tickImage.gameObject.SetActive(false);
            crossImage.gameObject.SetActive(false);
        }

        public virtual void Fill(Goal goal)
        {
            currentGoal = goal;
            var blockGoal = goal as CollectBlockGoal;
            if (blockGoal != null)
            {
                var specificGoal = blockGoal;
                var texture = Resources.Load("GameIcons/" + specificGoal.blockType) as Texture2D;
                if (texture != null)
                {
                    image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f), 100);
                }
                targetAmount = specificGoal.amount;
                amountText.text = targetAmount.ToString();
            }
        }

        public virtual void UpdateGoal(GameState state)
        {
            var newAmount = 0;
            var blockGoal = currentGoal as CollectBlockGoal;
            if (blockGoal != null)
            {
                newAmount = state.collectedBlocks[blockGoal.blockType];
            }

            if (newAmount == currentAmount)
            {
                return;
            }

            currentAmount = newAmount;
            

            if (currentAmount >= targetAmount)
            {
                currentAmount = targetAmount;
                SetCompletedTick(true);
                SoundManager.Instance.PlaySound("ReachedGoal");
            }
            tickImage.gameObject.SetActive(currentAmount >= targetAmount);
            amountText.gameObject.SetActive(currentAmount < targetAmount);
            amountText.text = (targetAmount - currentAmount).ToString();
        }

        internal void SetCompletedTick(bool completed)
        {
            isCompleted = completed;
            amountText.gameObject.SetActive(false);
            if (completed)
            {
                tickImage.gameObject.SetActive(true);
            }
            else
            {
                crossImage.gameObject.SetActive(true);
            }
        }
    }
}

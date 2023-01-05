using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle functionality of level btn present in Level Scene
    /// </summary>
    public class LevelButton : MonoBehaviour
    {
        public int numLevel;
        [SerializeField]
        private Sprite currentButtonSprite;
        [SerializeField]
        private Sprite playedButtonSprite;
        [SerializeField]
        private Sprite lockedButtonSprite;
        [SerializeField]
        private Image buttonImage;
        [SerializeField]
        private Text numLevelText;
        [SerializeField]
        private GameObject shineAnimation;

        public static System.Action<int> StartGamePopupEvent;

        private void Awake()
        {
            Assert.IsNotNull(currentButtonSprite);
            Assert.IsNotNull(playedButtonSprite);
            Assert.IsNotNull(lockedButtonSprite);
            Assert.IsNotNull(buttonImage);
            Assert.IsNotNull(numLevelText);
            Assert.IsNotNull(shineAnimation);
        }

        private void Start()
        {
            numLevelText.text = (numLevel).ToString();
            var nextLevel = PlayerPrefs.GetInt("next_level");
            if (nextLevel == 0)
            {
                nextLevel = 1;
            }
            if (numLevel == nextLevel)
            {
                buttonImage.sprite = currentButtonSprite;
                shineAnimation.SetActive(true);
            }
            else if (numLevel < nextLevel)
            {
                buttonImage.sprite = playedButtonSprite;
            }
            else
            {
                buttonImage.sprite = lockedButtonSprite;
            }
        }

        /// <summary>
        /// Called when the button is pressed.
        /// </summary>
        public void OnButtonPressed()
        {
            if (buttonImage.sprite == lockedButtonSprite)
            {
                return;
            }
            StartGamePopupEvent?.Invoke(numLevel);
        }
    }
}

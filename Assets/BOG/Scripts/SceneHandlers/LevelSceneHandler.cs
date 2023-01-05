using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle level scene functionality
    /// </summary>
    public class LevelSceneHandler : BaseSceneHandler
    {
        [SerializeField]
        private int numberOfLevels = 3;
        [SerializeField]
        private ScrollRect scrollRect;

        private void OnEnable()
        {
            Assert.IsNotNull(scrollRect);
        }

        private void OnDestroy()
        {
            LevelButton.StartGamePopupEvent -= OpenStartGamePopup;
        }

        private void Start()
        {
            scrollRect.vertical = false;
            LevelButton.StartGamePopupEvent += OpenStartGamePopup;
        }

        private void OpenStartGamePopup(int numLevel)
        {
            OpenPopup<StartGamePopup>("Popups/StartGamePopupPrefab", popup =>
            {
                popup.LoadLevelData(numLevel);
            });
        }
    }
}

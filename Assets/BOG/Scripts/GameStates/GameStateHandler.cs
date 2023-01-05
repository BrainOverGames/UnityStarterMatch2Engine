using UnityEngine;

namespace BOG
{
    /// <summary>
    /// Responsible to handle states functionality persent in game scene
    /// </summary>
    public class GameStateHandler : BaseStateHandler
    {
        [SerializeField] private GameSceneHandler gameSceneHandler;

        protected override void Start()
        {
            string fileName = "GameState" + GameManager.Instance.lastSelectedLevel + ".json";
            savePath = savePathPrefix + fileName;
            base.Start();
        }

        protected override void OnSaveBtnClicked()
        {
            GameStateManager.Instance.SaveGameData(gameSceneHandler.CurrentGameStateData, savePath);
            base.OnSaveBtnClicked();
        }

        protected override void OnLoadBtnClicked()
        {
            base.OnLoadBtnClicked();
            loadPath = "GameStates/GameState" + GameManager.Instance.lastSelectedLevel;
            var loadedGameStateData = GameStateManager.Instance.LoadGameData(loadPath);
            gameSceneHandler.LoadSavedGameState(loadedGameStateData);
        }
    }
}

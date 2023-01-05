using UnityEngine;

namespace BOG
{
    /// <summary>
    /// Manages all states functionality present in entire game 
    /// </summary>
    public class GameStateManager : PersistentSingleton<GameStateManager>   
    {
        protected override void Awake()
        {
            base.Awake();
        }

        internal void SaveGameData<T>(T currentGameStateData, string savePath) where T : GameStateData
        {
            ScreenCapture.CaptureScreenshot("Screenshot", 1);
            currentGameStateData.collectedBlocksList = BlockConversionUtils.DicToListConversion(currentGameStateData.collectedBlocksDic);
            FileUtils.SaveToJson(currentGameStateData, savePath);
        }

        internal GameStateData LoadGameData(string loadPath)
        {
            return FileUtils.LoadJsonFile<GameStateData>(loadPath);
        }
    }
}

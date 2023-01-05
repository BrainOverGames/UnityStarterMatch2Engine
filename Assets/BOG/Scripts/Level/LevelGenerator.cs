using UnityEngine;
using NaughtyAttributes;

namespace BOG
{
    /// <summary>
    /// Level editor
    /// </summary>
    [CreateAssetMenu(fileName = "LevelGenerator", menuName = "ScriptableObjects/Level/LevelGenerator", order = 0)]
    public class LevelGenerator : ScriptableObject
    { 
        [SerializeField] private LevelData levelData;
        [InfoBox("ONLY for LoadFromJSON", EInfoBoxType.Warning)]
        [SerializeField] private int levelToLoad;

        private const string savePathPrefix = "Assets/BOG/Resources/Levels/";
        internal LevelData LevelData { get { return levelData; } }

        [Button("GenerateRandomBlockTiles")]
        internal void GenerateRandomBlockTiles()
        {
            levelData.tiles.Clear();
            for (int i = 0; i < levelData.width * levelData.height; i++)
            {
                BlockTile blockTile = new BlockTile { blockerType = BlockerType.None, type = BlockType.RandomBlock };
                levelData.tiles.Add(blockTile);
            }
        }

        [Button("SaveToJSON")]
        internal void SaveJson()
        {
            if(levelData != null)
            {
                string fileName = "Level" + levelData.id + ".json";
                string path = savePathPrefix + fileName;
                FileUtils.SaveToJson(levelData, path);
            }
        }

        [Button("LoadFromJSON")]
        internal void LoadJson()
        {
            string loadPath = "Levels/Level" + levelToLoad;
            levelData = FileUtils.LoadJsonFile<LevelData>(loadPath);
        }
    }
}

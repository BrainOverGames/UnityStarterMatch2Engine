using UnityEngine;
using System.IO;

/// <summary>
/// Responsible to Save/Load JSON file to resources folder
/// </summary>
namespace BOG
{
    internal static class FileUtils
    {
        internal static void SaveToJson<T>(T dataToSave, string savePath)
        {
            string jsonStr = JsonUtility.ToJson(dataToSave);
            File.WriteAllText(savePath, jsonStr);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        internal static T LoadJsonFile<T>(string loadPath)
        {
            var jsonLoadText = Resources.Load<TextAsset>(loadPath);
            var loadedLevelData = JsonUtility.FromJson<T>(jsonLoadText.text);
            return loadedLevelData;
        }
    }
}

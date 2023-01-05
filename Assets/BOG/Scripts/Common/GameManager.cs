using UnityEngine;

namespace BOG
{
    /// <summary>
    /// Main Manager for entire game
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        public int lastSelectedLevel;
        public bool unlockedNextLevel;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log(1 % 7);
            Debug.Log(1 / 7);
            if (!PlayerPrefs.HasKey("sound_enabled"))
            {
                PlayerPrefs.SetInt("sound_enabled", 1);
            }
            if (!PlayerPrefs.HasKey("music_enabled"))
            {
                PlayerPrefs.SetInt("music_enabled", 1);
            }
            if (!PlayerPrefs.HasKey("next_level"))
            {
                PlayerPrefs.SetInt("next_level", 0);
            }
        }
    }
}

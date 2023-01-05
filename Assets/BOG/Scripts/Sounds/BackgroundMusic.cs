using UnityEngine;

namespace BOG
{
    /// <summary>
    /// Responsible to handle functionality of Bg Music
    /// </summary>
    public class BackgroundMusic : PersistentSingleton<BackgroundMusic>
    {
        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            var music = PlayerPrefs.GetInt("music_enabled");
            audioSource.mute = music == 0;
            audioSource.Play();
        }
    }
}

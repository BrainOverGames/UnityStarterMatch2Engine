using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to handle functionality of setting popup
    /// </summary>
    public class SettingsPopup : Popup
    {
        [SerializeField]
        private Slider soundSlider;
        [SerializeField]
        private Slider musicSlider;
        private int currentSound;
        private int currentMusic;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(soundSlider);
            Assert.IsNotNull(musicSlider);
        }

        protected override void Start()
        {
            base.Start();
            soundSlider.value = PlayerPrefs.GetInt("sound_enabled");
            musicSlider.value = PlayerPrefs.GetInt("music_enabled");
        }

        public void OnCloseButtonPressed()
        {
            Close();
        }

        public void OnSaveButtonPressed()
        {
            SoundManager.Instance.SetSoundEnabled(currentSound == 1);
            SoundManager.Instance.SetMusicEnabled(currentMusic == 1);
            Close();
        }

        public void OnSoundSliderValueChanged()
        {
            currentSound = (int) soundSlider.value;
        }

        public void OnMusicSliderValueChanged()
        {
            currentMusic = (int) musicSlider.value;
        }
    }
}

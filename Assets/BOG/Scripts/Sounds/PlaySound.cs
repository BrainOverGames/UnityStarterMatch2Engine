using UnityEngine;

namespace BOG
{
    /// <summary>
    /// Responsible to play sound FX
    /// </summary>
    public class PlaySound : MonoBehaviour
    {
        public void Play(string soundName)
        {
            SoundManager.Instance.PlaySound(soundName);
        }
    }
}

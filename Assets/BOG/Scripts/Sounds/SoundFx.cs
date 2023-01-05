using UnityEngine;
using UnityEngine.Assertions;

namespace BOG
{
    /// <summary>
    /// SFX component
    /// </summary>
    public class SoundFx : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            Assert.IsTrue(audioSource != null);
        }

        public void Play(AudioClip clip, bool loop = false)
        {
            if (clip == null)
            {
                return;
            }
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();
            Invoke("DisableSoundFx", clip.length + 0.1f);
        }

        private void DisableSoundFx()
        {
            GetComponent<PooledObject>().pool.ReturnObject(gameObject);
        }
    }
}

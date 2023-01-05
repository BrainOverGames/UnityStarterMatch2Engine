using UnityEngine;
using UnityEngine.UI;

namespace BOG
{
    /// <summary>
    /// Responsible to swap the sprites of Image
    /// </summary>
    public class SpriteSwapper : MonoBehaviour
    {
        [SerializeField]
        private Sprite enabledSprite;

        [SerializeField]
        private Sprite disabledSprite;

        private Image image;

        public void Awake()
        {
            image = GetComponent<Image>();
        }

        public void SwapSprite()
        {
            image.sprite = image.sprite == enabledSprite ? disabledSprite : enabledSprite;
        }

        public void SetEnabled(bool spriteEnabled)
        {
            image.sprite = spriteEnabled ? enabledSprite : disabledSprite;
        }
    }
}

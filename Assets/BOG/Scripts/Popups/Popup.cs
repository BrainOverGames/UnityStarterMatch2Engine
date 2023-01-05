using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BOG
{
    /// <summary>
    /// Base class for all popups
    /// </summary>
    public class Popup : MonoBehaviour
    {
        [HideInInspector] public BaseSceneHandler parentScene;

        public UnityEvent onOpen;
        public UnityEvent onClose;

        private Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            onOpen.Invoke();
        }

        public void Close()
        {
            onClose.Invoke();
            if (parentScene != null)
            {
                parentScene.ClosePopup();
            }
            if (animator != null)
            {
                animator.Play("Close");
                StartCoroutine(DestroyPopup());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual IEnumerator DestroyPopup()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}

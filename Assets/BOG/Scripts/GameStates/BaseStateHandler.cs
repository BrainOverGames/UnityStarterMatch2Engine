using UnityEngine.UI;
using UnityEngine;

namespace BOG
{
    /// <summary>
    /// Base class for handling functionality of the game's states
    /// </summary>
    public class BaseStateHandler : MonoBehaviour
    {
        [SerializeField] private Button saveBtn;
        [SerializeField] private Button loadBtn;

        protected string savePathPrefix;
        protected string savePath;

        protected string loadPath;

        protected virtual void OnEnable()
        {
            savePathPrefix = "Assets/BOG/Resources/GameStates/";
            saveBtn.onClick.AddListener(OnSaveBtnClicked);
            loadBtn.onClick.AddListener(OnLoadBtnClicked);
        }

        protected virtual void OnDisable()
        {
            saveBtn.onClick.RemoveListener(OnSaveBtnClicked);
            loadBtn.onClick.RemoveListener(OnLoadBtnClicked);
        }

        protected virtual void Start()
        {
            loadBtn.interactable = System.IO.File.Exists(savePath);
        }

        protected virtual void OnSaveBtnClicked()
        {
            loadBtn.interactable = System.IO.File.Exists(savePath);
        }

        protected virtual void OnLoadBtnClicked()
        {

        }
    }
}

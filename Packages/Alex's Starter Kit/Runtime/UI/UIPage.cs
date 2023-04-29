using UnityEngine;

namespace ASK.UI
{
    public class UIPage : MonoBehaviour
    {
        private RectTransform _rectTransform;
        [SerializeField] private Vector3 hidePos;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (hidePos == default) hidePos = _rectTransform.anchoredPosition;
        }
        
        public void Show(bool e)
        {
            _rectTransform.anchoredPosition = e ? Vector3.zero : hidePos;
        }
    }
}
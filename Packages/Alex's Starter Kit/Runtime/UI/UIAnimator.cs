using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ASK.UI
{
    [RequireComponent(typeof(Image))]
    public class FakeAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] Frames;

        [SerializeField] private float fps = 0.05f;

        private Image _img;

        private int _curIndex = 0;

        private Coroutine _curRoutine;

        private void Awake()
        {
            _img = GetComponent<Image>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _curIndex = 0;
            _curRoutine = StartCoroutine(AnimateRoutine());
        }

        public void Hide()
        {
            if (_curRoutine != null) StopCoroutine(_curRoutine);
            gameObject.SetActive(false);
        }
        
        private IEnumerator AnimateRoutine()
        {
            yield return new WaitForSeconds(fps);
            _curIndex = (_curIndex + 1) % (Frames.Length);
            _img.sprite = Frames[_curIndex];
            StartCoroutine(AnimateRoutine());
        }
    }
}
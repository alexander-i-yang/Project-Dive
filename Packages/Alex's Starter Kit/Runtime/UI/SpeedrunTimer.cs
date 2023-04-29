using ASK.Core;
using ASK.Helpers;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ASK.UI
{
    public class SpeedrunTimer : MonoBehaviour
    {
        private Text _text;
        private float _bestTime;
        private string _bestTimeStr;
        private string _savePropertyId;
        
        [FormerlySerializedAs("_running")] [SerializeField] private bool running = true;
        private float _curTime;

        protected virtual void Awake()
        {
            _text = GetComponent<Text>();
            _savePropertyId = SceneManager.GetActiveScene().name + "_best_time";
            _bestTime = PlayerPrefs.GetFloat(_savePropertyId, -1);
            _bestTimeStr = _bestTime < 0 ? "None" : Helper.FormatTime(_bestTime);
        }

        protected virtual void OnEnable()
        {
            SpeedrunTimerToggleReceiver.ToggleEvent += SetShowing;
            Game.Instance.OnDebugEvent += GrayOut;
        }

        protected virtual void OnDisable()
        {
            SpeedrunTimerToggleReceiver.ToggleEvent -= SetShowing;
            Game.Instance.OnDebugEvent -= GrayOut;
        }

        public void StopTimer()
        {
            running = false;
            float bestTime = _bestTime < 0 ? _curTime : Mathf.Min(_bestTime, _curTime);
            if (!Game.Instance.IsDebug) PlayerPrefs.SetFloat(_savePropertyId, bestTime);
            _text.text = GetText(_curTime, Helper.FormatTime(bestTime));
        }

        public string GetText(float t, string bestTimeStr)
        {
            return Helper.FormatTime(t) + $"\nBest Time: {bestTimeStr}";
        }

        private void Update()
        {
            if (running)
            {
                _curTime = Game.Instance.Time;
                _text.text = GetText(_curTime, _bestTimeStr);
            }
        }

        private void SetShowing(bool e)
        {
            _text.SetAlpha(e ? 1 : 0);
        }

        private void GrayOut()
        {
            float prevAlpha = _text.color.a;
            _text.color = Color.gray;
            _text.SetAlpha(prevAlpha);
        }
    }
}
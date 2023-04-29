using System.Collections;
using ASK.Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ASK.Helpers
{
    public class FadeLightAnimator : MonoBehaviour
    {
        [SerializeField] private AnimationCurve animCurve;
        [SerializeField] private float animTime = 0.5f;

        [SerializeField] private float endIntensity = 0;
        private float _startIntensity;
        
        private Light2D _light2D;
        private LightFlicker _flicker;

        private Coroutine _curRoutine;

        void Awake()
        {
            _light2D = GetComponent<Light2D>();
            _flicker = GetComponent<LightFlicker>();
            _startIntensity = _light2D.intensity;
        }
        
        public void FadeOut()
        {
            if (_flicker != null)
            {
                _flicker.enabled = false;
            }

            FadeBetween(_startIntensity, endIntensity);
        }

        public void FadeIn()
        {
            FadeBetween(endIntensity, _startIntensity, () =>
            {
                if (_flicker != null)
                {
                    _flicker.enabled = true;
                }
            });
        }

        public void FadeBetween(float startI, float endI, System.Action onEnd = null)
        {
            if (_curRoutine != null) StopCoroutine(_curRoutine);
            _curRoutine = StartCoroutine(Animate(startI, endI, onEnd));
        }
        
        private IEnumerator Animate(float startI, float endI, System.Action onEnd = null)
        {
            float t = 0f;
            while (t <= animTime)
            {
                t += Game.Instance.DeltaTime;
                float s = animCurve.Evaluate(t/animTime);
                _light2D.intensity = Mathf.Lerp(startI, endI, s);
                yield return null;
            }
            
            if (onEnd != null) onEnd.Invoke();
        }
    }
}
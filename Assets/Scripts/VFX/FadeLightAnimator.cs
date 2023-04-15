using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VFX
{
    public class FadeLightAnimator : MonoBehaviour
    {
        [SerializeField] private AnimationCurve animCurve;
        [SerializeField] private float animTime = 0.5f;

        [SerializeField] private float endIntensity = 0;
        private float _startIntensity;
        
        private Light2D _light2D;

        void Awake()
        {
            _light2D = GetComponent<Light2D>();
        }
        
        public void Fade()
        {
            _startIntensity = _light2D.intensity;

            LightFlicker l = GetComponent<LightFlicker>();
            if (l != null)
            {
                l.enabled = false;
            }

            StartCoroutine(Animate());
        }
        
        private IEnumerator Animate()
        {
            float t = 0f;
            while (t <= animTime)
            {
                t += Game.Instance.DeltaTime;
                float s = animCurve.Evaluate(t);
                _light2D.intensity = Mathf.Lerp(_startIntensity, endIntensity, s);
                yield return null;
            }
        }
    }
}
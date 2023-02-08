using System;
using System.Collections;
using Core;
using Helpers;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Mechanics
{
    public class MushroomUnlockLogic : MonoBehaviour
    {
        [SerializeField] private float cutsceneLengthStart;
        [SerializeField] private float cutsceneLengthWait;
        [SerializeField] private float cutsceneLengthFade;
        

        private Light2D _light;
        [SerializeField] private float startIntensity;
        [SerializeField] private float endIntensity;
        [SerializeField] private float endRadius;
        
        [SerializeField] private AnimationCurve startCurve;
        [SerializeField] private AnimationCurve endCurve;
        
        [SerializeField] LayerMask mask = default;

        private Crystal[] _mushrooms;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            _mushrooms = FindObjectsOfType<Crystal>();
        }

        public void Unlock()
        {
            StartCoroutine(UnlockCoroutine());
        }

        IEnumerator UnlockCoroutine()
        {
            float r;
            for (float t = 0; t < cutsceneLengthStart; t += Game.Instance.DeltaTime)
            {
                r = Mathf.Lerp(0, endRadius, t);
                _light.pointLightOuterRadius = r;
                _light.intensity = Mathf.Lerp(startIntensity, endIntensity, startCurve.Evaluate(t/cutsceneLengthStart));

                UnlockOverlapCircle(r);
                yield return null;
            }

            UnlockAllMushrooms();

            yield return Helper.Delay(cutsceneLengthWait);
            
            float curIntensity = _light.intensity;
            for (float t = 0; t < cutsceneLengthFade; t += Game.Instance.DeltaTime)
            {
                _light.intensity = Mathf.Lerp(curIntensity, 0, endCurve.Evaluate(t/cutsceneLengthFade));
                yield return null;
            }
        }

        private void UnlockOverlapCircle(float radius)
        {
            Collider2D[] c = Physics2D.OverlapCircleAll(
                transform.position,
                radius,
                mask
            );
            foreach (var mushroomCol in c)
            {
                Crystal mushroom = mushroomCol.gameObject.GetComponent<Crystal>();
                print(mushroomCol.gameObject);
                if (mushroom != null && !mushroom.unlocked)
                {
                    mushroom.Unlock();
                }
            }
        }

        private void UnlockAllMushrooms()
        {
            foreach (var mushroom in _mushrooms)
            {
                if (!mushroom.unlocked) mushroom.Unlock();
            }
        }
    }
}
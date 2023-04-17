﻿using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VFX;

namespace UI
{
    public class FireflyIndicatorInner : MonoBehaviour
    {
        [SerializeField] private AnimationCurve animCurve;
        [SerializeField] private float animLength;

        private FadeLightAnimator _fadeAnimator;
        private Coroutine _curRoutine;

        private bool _showing;

        private void Awake()
        {
            _fadeAnimator = GetComponentInChildren<FadeLightAnimator>();
            GetComponentInChildren<Light2D>().intensity = 0;
            transform.localScale = Vector3.zero;
        }

        public void Show()
        {
            if (_curRoutine != null) StopCoroutine(_curRoutine);
            _curRoutine = StartCoroutine(ShowRoutine());
            _fadeAnimator.FadeBetween(1.5f, 0);
            _showing = true;
        }
        
        public void Hide()
        {
            if (_curRoutine != null) StopCoroutine(_curRoutine);
            if (_showing)
            {
                _curRoutine = StartCoroutine(HideRoutine());
                _fadeAnimator.FadeBetween(0, 1.5f);
            }
            _showing = false;
        }

        private IEnumerator ShowRoutine()
        {
            for (float t = 0; t < animLength; t += Game.Instance.DeltaTime)
            {
                float mult = animCurve.Evaluate(t / animLength);
                transform.localScale = Vector3.one * mult;
                yield return null;
            }
        }
        
        private IEnumerator HideRoutine()
        {
            for (float t = 0; t < animLength; t += Game.Instance.DeltaTime)
            {
                float mult = animCurve.Evaluate(t / animLength);
                transform.localScale = Vector3.one * (1-mult);
                yield return null;
            }
        }
    }
}
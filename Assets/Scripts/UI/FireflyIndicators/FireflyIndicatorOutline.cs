using System;
using Collectibles;
using Helpers;
using UnityEngine;

namespace UI
{
    public class FireflyIndicatorOutline : MonoBehaviour
    {
        private Vector2 _startPos;
        private Vector2 _endPos;

        private FireflyAnimator _animator;
        private FireflyIndicatorInner _inner;

        private Coroutine _moveRoutine;
        private Coroutine _innerRoutine;

        public void Init(Vector2 endPos)
        {
            _endPos = endPos;
            _startPos = transform.position;
            _animator = GetComponent<FireflyAnimator>();
            _inner = GetComponentInChildren<FireflyIndicatorInner>();
            transform.localScale = Vector3.zero;
        }

        public void Show(float delay)
        {
            AnimateTo(delay, _endPos, AnimHookScale, () => transform.localScale = Vector3.one);
        }

        public void ShowInner(float delay)
        {
            _innerRoutine = StartCoroutine(Helper.DelayAction(delay, _inner.Show));
        }

        public void Hide(float delay)
        {
            AnimateTo(delay, _startPos, t => AnimHookScale(1 - t), null);
            if (_innerRoutine != null) StopCoroutine(_innerRoutine);
            _inner.Hide();
        }

        private void AnimateTo(float delay, Vector2 whereTo, Action<float> hook, Action onFinish)
        {
            _animator.EndPos = whereTo;
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            
            _moveRoutine = StartCoroutine(
                Helper.DelayAction(
                    delay,
                    () =>
                    {
                        _animator.PlayAnimation(OnAnimationFinish: () =>
                        {
                            transform.position = whereTo;
                            if (onFinish != null) onFinish();
                        }, hook: hook);
                    }
                )
            );
        }

        public void SpecialFinish()
        {
            _inner.SpecialFinish();
        }

        private void AnimHookScale(float t)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(0, 1, t);
        }
    }
}
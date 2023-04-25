using System;
using System.Collections;
using System.Collections.Generic;
using Collectibles;
using Helpers;
using Mechanics;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace UI
{
    enum FICState{
        SHOWING,
        FILLING,
        FILLED,
        HIDDEN,
        FINISHED,
    }

    public class FireflyIndicatorController : MonoBehaviour
    {
        [SerializeField] private GameObject indicator;
        
        [SerializeField] private int startAngle;
        [SerializeField] private int endAngle;
        [SerializeField] private int radius;
        
        [SerializeField] private float timeStagger;
        [SerializeField] private float innerTimeStagger;
        [SerializeField] private float innerDelay;

        private FireflyIndicatorOutline[] _indicators;
        private Coroutine _showInnerRoutine;
        private Coroutine _finishRoutine;

        private FICState _state;
        private int _curFireflies;
        private Action _onFillInnerFinish;

        private void Awake()
        {
            int numIndicators = GetComponentInParent<Gate>().RequiredFireflies;
            Vector2[] points = CalcPoints(numIndicators);
            _indicators = InstantiateIndicators(points);

            Firefly.OnCollectAnimFinish += OnFireflyCollect;
            _state = FICState.HIDDEN;
        }

        private Vector2[] CalcPoints(int numIndicators)
        {
            List<Vector2> ret = new List<Vector2>();
            for (int i = 0; i < numIndicators; ++i)
            {
                float curAngle = Mathf.Lerp(startAngle, endAngle, i / ((float)numIndicators - 1));
                curAngle *= Mathf.Deg2Rad;
                Vector3 offset = new Vector3((float)Math.Cos(curAngle), (float)Math.Sin(curAngle)) * radius;
                offset = new Vector3((int)offset.x, (int)offset.y);
                ret.Add(transform.position + offset);
            }

            return ret.ToArray();
        }

        private FireflyIndicatorOutline[] InstantiateIndicators(Vector2[] pts)
        {
            List<FireflyIndicatorOutline> ret = new List<FireflyIndicatorOutline>();
            for (int i = 0; i < pts.Length; ++i)
            {
                FireflyIndicatorOutline newIndicatorOutline = Instantiate(indicator, transform).GetComponent<FireflyIndicatorOutline>();
                newIndicatorOutline.Init(pts[i]);
                ret.Add(newIndicatorOutline);
            }

            return ret.ToArray();
        }

        public void Show(int numCollected, Action onFinish)
        {
            _state = FICState.SHOWING;
            _curFireflies = numCollected;
            _onFillInnerFinish = onFinish;
            for (var i = 0; i < _indicators.Length; i++)
            {
                FireflyIndicatorOutline indicatorOutline = _indicators[i];
                indicatorOutline.Show(timeStagger * i);
            }

            _showInnerRoutine = StartCoroutine(ShowInnerRoutine(innerDelay));
        }

        public void OnFireflyCollect(int quantity)
        {
            _curFireflies = quantity;
            if (quantity > _indicators.Length) return;
            
            switch (_state)
            {
                case FICState.SHOWING:
                    break;
                case FICState.FILLED:
                    FireflyIndicatorOutline indicatorOutline = _indicators[quantity-1];
                    indicatorOutline.ShowInner(0);
                    RunFinishRoutine(innerTimeStagger);
                    break;
                case FICState.FILLING:
                    break;
                case FICState.HIDDEN:
                    break;
            }
        }

        public void SpecialFinish(float delayTime)
        {
            _state = FICState.FINISHED;
            StartCoroutine(Helper.DelayAction(delayTime / 2, () =>
            {
                foreach (var ind in _indicators) ind.SpecialFinish();
            }));
        }

        public void RunFinishRoutine(float delay)
        {
            _finishRoutine = StartCoroutine(
                Helper.DelayAction(delay, () =>
                {
                    _state = FICState.FILLED;
                    _onFillInnerFinish();
                })
            );
        }

        private IEnumerator ShowInnerRoutine(float initialDelay)
        {
            var numCollected = Math.Min(_curFireflies, _indicators.Length);
            yield return new WaitForSeconds(initialDelay);
            RunFinishRoutine(innerTimeStagger * (numCollected+1));
            _state = FICState.FILLING;
            for (var i = 0; i < numCollected; i++)
            {
                numCollected = Math.Min(_curFireflies, _indicators.Length);
                FireflyIndicatorOutline indicatorOutline = _indicators[i];
                indicatorOutline.ShowInner(0);
                yield return new WaitForSeconds(innerTimeStagger);
            }
        }
        
        public void Hide()
        {
            if (_state == FICState.FINISHED) return;
            _state = FICState.HIDDEN;
            if (_showInnerRoutine != null) StopCoroutine(_showInnerRoutine);
            if (_finishRoutine != null) StopCoroutine(_finishRoutine);
            for (var i = 0; i < _indicators.Length; i++)
            {
                FireflyIndicatorOutline indicatorOutline = _indicators[i];
                indicatorOutline.Hide(timeStagger * i);
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            int numIndicators = GetComponentInParent<Gate>().RequiredFireflies;
            Vector2[] pts = CalcPoints(numIndicators);
            foreach (var p in pts)
            {
                Handles.DrawLine(transform.position, p);
            }
        }
#endif
    }
}
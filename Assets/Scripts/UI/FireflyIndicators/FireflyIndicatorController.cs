using System;
using System.Collections.Generic;
using System.Numerics;
using Helpers;
using Mechanics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace UI
{
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

        private void Awake()
        {
            int numIndicators = GetComponentInParent<Gate>().RequiredFireflies;
            Vector2[] points = CalcPoints(numIndicators);
            _indicators = InstantiateIndicators(points);
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

        public void Show()
        {
            for (var i = 0; i < _indicators.Length; i++)
            {
                FireflyIndicatorOutline indicatorOutline = _indicators[i];
                indicatorOutline.Show(timeStagger * i);
            }

            _showInnerRoutine = StartCoroutine(Helper.DelayAction(innerDelay, () => ShowInners()));
        }

        private void ShowInners()
        {
            int numCollected = 4;
            for (var i = 0; i < numCollected; i++)
            {
                FireflyIndicatorOutline indicatorOutline = _indicators[i];
                indicatorOutline.ShowInner(innerTimeStagger * i);
            }
        }
        
        public void Hide()
        {
            if (_showInnerRoutine != null) StopCoroutine(_showInnerRoutine);
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
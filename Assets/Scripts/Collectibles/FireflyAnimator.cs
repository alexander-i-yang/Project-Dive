using System.Collections.Generic;

using UnityEngine;

using Core;
using Helpers;
using Helpers.Animation;

namespace Collectibles
{
    public class FireflyAnimator : MonoBehaviour, ICurveAnimProvider
    {
        private const int numSamplesVisual = 50;

        //[SerializeField] private Transform target;
        [SerializeField] private float startSpeed;
        [SerializeField, Range(0f, 360f)] private float startAngleDeg;
        [SerializeField, Range(0f, 360f)] private float endAngleDeg;
        [SerializeField] private float endSpeed;
        [SerializeField] private float animSpeed;

        private CurveAnimator _curveAnim;

        private Vector3 _offScreenPoint => new Vector3(1, 1, _camera.nearClipPlane);

        private Camera _camera => Game.Instance.MainCamera;

        private void Awake()
        {
            _curveAnim = new CurveAnimator(this, this);
        }

        public void PlayAnimation(System.Action OnAnimationFinish)
        {
            _curveAnim.PlayAnimation(OnAnimationFinish);
        }

        public CubicCurve2D GetCurve()
        {
            Vector2 startPos = _camera.WorldToViewportPoint(transform.position);
            Vector2 endPos = _offScreenPoint;
            Vector2 dirToGate = (endPos - startPos).normalized;
            Vector2 startVel = startSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * startAngleDeg);
            Vector2 endVel = endSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * endAngleDeg);
            return new HermiteCurve2D(startPos, endPos, startVel, endVel);
        }

        public float GetAnimSpeed()
        {
            return animSpeed;
        }

        public CurveRelativeTo GetRelativeTo()
        {
            return CurveRelativeTo.Viewport;
        }

        private void OnDrawGizmosSelected()
        {
            if (_curveAnim == null)
            {
                _curveAnim = new CurveAnimator(this, this);
            }

            _curveAnim.DrawCurve(numSamplesVisual);
        }
    }
}
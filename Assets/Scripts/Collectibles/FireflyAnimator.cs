using System.Collections.Generic;

using UnityEngine;

using Core;
using Helpers;
using Helpers.Animation;

namespace Collectibles
{
    public class FireflyAnimator : MonoBehaviour, ICurveAnimProvider
    {
        protected const int numSamplesVisual = 50;

        //[SerializeField] private Transform target;
        [SerializeField] protected float startSpeed;
        [SerializeField, Range(0f, 360f)] protected float startAngleDeg;
        [SerializeField, Range(0f, 360f)] protected float endAngleDeg;
        [SerializeField] protected float endSpeed;
        [SerializeField] protected float animSpeed;

        protected CurveAnimator _curveAnim;

        public Vector2 EndPos;

        private void Awake()
        {
            _curveAnim = new CurveAnimator(this, this);
        }

        public void PlayAnimation(System.Action OnAnimationFinish, System.Action<float> hook = null)
        {
            _curveAnim.PlayAnimation(OnAnimationFinish, hook);
        }
        
        public void StopAnimation() {_curveAnim.StopAnimation();}

        public virtual CubicCurve2D GetCurve()
        {
            Vector2 startPos = transform.position;
            Vector2 dirToGate = (EndPos - startPos).normalized;
            Vector2 startVel = startSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * startAngleDeg);
            Vector2 endVel = endSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * endAngleDeg);
            return new HermiteCurve2D(startPos, EndPos, startVel, endVel);
        }

        public float GetAnimSpeed()
        {
            return animSpeed;
        }

        public virtual CurveRelativeTo GetRelativeTo()
        {
            return CurveRelativeTo.World;
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
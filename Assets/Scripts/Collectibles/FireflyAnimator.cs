using System.Collections.Generic;

using UnityEngine;

using Helpers;
using Helpers.Animation;

namespace Collectibles
{
    public class FireflyAnimator : MonoBehaviour, ICurveAnimProvider
    {
        private const int numSamplesVisual = 50;

        [SerializeField] private Transform target;
        [SerializeField] private float startSpeed;
        [SerializeField, Range(0f, 360f)] private float startAngleDeg;
        [SerializeField, Range(0f, 360f)] private float endAngleDeg;
        [SerializeField] private float endSpeed;
        [SerializeField] private float animSpeed;

        private CurveAnimator _curveAnim;

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
            Vector2 dirToGate = (target.transform.position - transform.position).normalized;
            Vector2 startVel = startSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * startAngleDeg);
            Vector2 endVel = endSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * endAngleDeg);
            return new HermiteCurve2D(transform.position, target.transform.position, startVel, endVel);
        }

        public float GetAnimSpeed()
        {
            return animSpeed;
        }

        public CurveRelativeTo GetRelativeTo()
        {
            return CurveRelativeTo.World;
        }

        private void OnDrawGizmosSelected()
        {
            List<Vector2> samples = GetCurve().SamplePoints(numSamplesVisual);

            Gizmos.color = Color.green;
            foreach (Vector2 sample in samples)
            {
                Gizmos.DrawSphere(sample, 2);
            }
        }
    }
}
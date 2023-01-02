using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyBox;

using Core;

namespace Helpers.Animation
{
    public class ViewportCurveAnimator : MonoBehaviour
    {
        [SerializeField] private Vector2 startVelocity;
        [SerializeField] private Vector2 endVelocity;
        [SerializeField] private float animSpeed = 1f;

        private const int numSamplesVisual = 50;

        private CubicCurve2D _animCurve;
        private Coroutine _animCorout;

        private Camera _camera => Game.Instance.MainCamera;
        private Vector2 _offScreenPoint => new Vector3(1, 1, _camera.nearClipPlane);

        public void PlayAnimation(System.Action onAnimationFinish)
        {
            _animCurve = GetAnimCurve();

            _animCorout = StartCoroutine(AnimateCurve());
            _animCorout.OnComplete(onAnimationFinish);
        }

        private IEnumerator AnimateCurve()
        {
            float t = 0;
            while (t < 1f)
            {
                Vector2 newPosViewport = _animCurve.Evaluate(Mathf.Clamp01(t));
                Vector2 newPos = _camera.ViewportToWorldPoint(newPosViewport);
                transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
                t += Game.Instance.DeltaTime * animSpeed;
                yield return null;
            }
        }

        private CubicCurve2D GetAnimCurve()
        {
            Vector3 posOnViewport = _camera.WorldToViewportPoint(transform.position);

            return new HermiteCurve2D(posOnViewport, _offScreenPoint, startVelocity, endVelocity);
        }

        private void OnDrawGizmosSelected()
        {
            CubicCurve2D animCurve = GetAnimCurve();
            List<Vector2> samples = animCurve.SamplePoints(numSamplesVisual);

            Gizmos.color = Color.green;
            foreach (Vector2 sample in samples)
            {
                Gizmos.DrawSphere(_camera.ViewportToWorldPoint(sample), 2);
            }
        }
    }
}
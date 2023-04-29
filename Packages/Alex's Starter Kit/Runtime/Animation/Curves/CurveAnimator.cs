using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyBox;

using ASK.Core;

namespace ASK.Animation
{
    public class CurveAnimator
    {
        private ICurveAnimProvider _curveProvider;
        private MonoBehaviour _target;

        private CubicCurve2D _animCurve;
        private Coroutine _animCorout;

        private Camera _camera => Game.Instance.CamController.MainCamera;

        public CurveAnimator(ICurveAnimProvider curveProvider, MonoBehaviour target)
        {
            _curveProvider = curveProvider;
            _target = target;
        }

        public void PlayAnimation(System.Action onAnimationFinish, System.Action<float> hook)
        {
            _animCurve = _curveProvider.GetCurve();
            _animCorout = _target.StartCoroutine(AnimateCurve(hook));
            _animCorout.OnComplete(onAnimationFinish);
        }

        public void StopAnimation()
        {
            if (_animCorout != null) _target.StopCoroutine(_animCorout);
        }

        private IEnumerator AnimateCurve(System.Action<float> hook)
        {
            for (float t = 0; t <= 1f; t += Game.Instance.DeltaTime * _curveProvider.GetAnimSpeed())
            {
                Vector2 newPos = GetWorldPos(_animCurve.Evaluate(Mathf.Clamp01(t)));
                _target.transform.position = new Vector3(newPos.x, newPos.y, _target.transform.position.z);

                if (hook != null) hook(t);
                yield return null;
            }
        }

        public Vector2 GetWorldPos(Vector2 curvePos)
        {
            switch (_curveProvider.GetRelativeTo())
            {
                case CurveRelativeTo.Viewport:
                    return _camera.ViewportToWorldPoint(curvePos);
                case CurveRelativeTo.Screen:
                    return _camera.ScreenToWorldPoint(curvePos);
                case CurveRelativeTo.World:
                default:
                    return curvePos;
            }
        }

        public void DrawCurve(int numSamples)
        {
            List<Vector2> samples = _curveProvider.GetCurve().SamplePoints(numSamples);

            Gizmos.color = Color.green;
            foreach (Vector2 sample in samples)
            {
                Gizmos.DrawSphere(GetWorldPos(sample), 2);
            }
        }
    }
}
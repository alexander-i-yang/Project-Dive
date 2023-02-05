using System.Collections.Generic;

using UnityEngine;

using Core;
using Helpers;
using Helpers.Animation;

namespace Collectibles
{
    public class FireflyAnimatorEnd : FireflyAnimator
    {
        protected Camera _camera => Game.Instance.MainCamera;
        private Vector3 _offScreenPoint => new Vector3(1, 1, _camera.nearClipPlane);
        
        public override CubicCurve2D GetCurve()
        {
            Vector2 startPos = _camera.WorldToViewportPoint(transform.position);
            Vector2 endPos = _offScreenPoint;
            Vector2 dirToGate = (endPos - startPos).normalized;
            Vector2 startVel = startSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * startAngleDeg);
            Vector2 endVel = endSpeed * Helper.RotateVector2(dirToGate, Mathf.Deg2Rad * endAngleDeg);
            return new HermiteCurve2D(startPos, endPos, startVel, endVel);
        }

        public override CurveRelativeTo GetRelativeTo()
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
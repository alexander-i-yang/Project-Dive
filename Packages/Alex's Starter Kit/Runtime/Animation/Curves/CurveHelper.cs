using UnityEngine;
using System.Collections;
using ASK.Core;

namespace ASK.Animation
{
    public class CurveHelper
    {
        public static IEnumerator AnimateCurve(GameObject go, CubicCurve2D curve, System.Action OnAnimationFinish = null)
        {
            yield return AnimateCurve(go, curve, 1, OnAnimationFinish);
        }

        public static IEnumerator AnimateCurve(GameObject go, CubicCurve2D curve, float animSpeed = 1, System.Action OnAnimationFinish = null)
        {
            float t = 0;
            while (t < 1f)
            {
                Vector2 newPos = curve.Evaluate(Mathf.Clamp01(t));
                go.transform.position = newPos;
                t += Game.Instance.DeltaTime * animSpeed;
                yield return null;
            }

            OnAnimationFinish();
        }
    }

}
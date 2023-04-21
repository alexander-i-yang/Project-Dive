using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

using Core;

public class GateAnimator : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float animTime = 0.5f;

    private Coroutine _animCorout;

    public void PlayAnimation(System.Action OnAnimationFinish = null)
    {
        _animCorout = StartCoroutine(Animate());
        _animCorout.OnComplete(OnAnimationFinish);
    }

    private IEnumerator Animate()
    {
        float t = 0f;
        while (t <= animTime)
        {
            t += Game.Instance.DeltaTime;
            float s = animCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos.position, endPos.position, s);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(startPos.position, 2);
        Gizmos.DrawSphere(endPos.position, 2);
    }
}

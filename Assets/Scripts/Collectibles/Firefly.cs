using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MyBox;

using Core;
using Helpers;
using Helpers.Animation;

namespace Collectibles {
    public class Firefly : Collectible, IFilterLoggerTarget
    {
        [SerializeField] private float animSpeed;

        public override string ID => "Firefly";

        public override void OnTouched(Collector collector)
        {
            FilterLogger.Log(this, $"{gameObject.name} Touched {collector}");
            StartCoroutine(CurveHelper.AnimateCurve(gameObject, GetAnimCurve(), animSpeed, () => OnFinishCollected(collector)));
        }


        private void OnFinishCollected(Collector collector)
        {
            collector.OnCollectFinished(this);
            Destroy(gameObject);
        }

        private Vector3 GetOffScreenPoint()
        {
            return Game.Instance.MainCamera.ViewportToWorldPoint(new Vector3(1, 1, Game.Instance.MainCamera.nearClipPlane));
        }

        private CubicCurve2D GetAnimCurve()
        {
            return new HermiteCurve2D(transform.position, GetOffScreenPoint(), -200 * Vector2.right, 50 * (Vector2.up + Vector2.right));
        }

        private void OnDrawGizmosSelected()
        {
            CubicCurve2D animCurve = GetAnimCurve();
            List<Vector2> samples = animCurve.SamplePoints(50);

            Gizmos.color = Color.green;
            foreach (Vector2 sample in samples)
            {;
                Gizmos.DrawSphere(sample, 2);
            }
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
}
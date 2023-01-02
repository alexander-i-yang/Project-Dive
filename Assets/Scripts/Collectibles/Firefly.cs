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
        private ViewportCurveAnimator _animator;
        public override string ID => "Firefly";

        private void Awake()
        {
            _animator = GetComponent<ViewportCurveAnimator>();
        }
        public override void OnTouched(Collector collector)
        {
            FilterLogger.Log(this, $"{gameObject.name} Touched {collector}");
            _animator.PlayAnimation(() => OnFinishCollected(collector));
        }

        private void OnFinishCollected(Collector collector)
        {
            collector.OnCollectFinished(this);
            Destroy(gameObject);
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Info;
        }
    }
}
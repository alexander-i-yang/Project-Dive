using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MyBox;

using Helpers;
using Mechanics;

namespace Collectibles {
    public class Firefly : Collectible, IFilterLoggerTarget
    {
        //[SerializeField] private Gate targetGate;

        private FireflyAnimator _animator;

        private bool _touched = false;

        public override string ID => "Firefly";

        public static string s_ID => "Firefly";

        private void Awake()
        {
            _animator = GetComponent<FireflyAnimator>();
        }

        public override void OnTouched(Collector collector)
        {
            if (!_touched)
            {
                _touched = true;
                FilterLogger.Log(this, $"{gameObject.name} Touched {collector}");
                _animator.PlayAnimation(() => OnFinishCollected(collector));
            }
        }

        private void OnFinishCollected(Collector collector)
        {
            collector.OnCollectFinished(this);
            Destroy(gameObject);
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
}
using System.Collections.Generic;
using Helpers;
using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics {
    public class Spike : Solid, IFilterLoggerTarget {
        public bool Charged { get; private set; } = true;
        public float RechargeTime = 0.5f;
        private Coroutine _reEnableCoroutine;
        private SpriteRenderer _mySR;

        protected new void Start()
        {
            base.Start();
            _mySR = GetComponent<SpriteRenderer>();
        }

        public override bool Collidable() {
            return false;
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            ISpikeResponse response = p.GetComponent<ISpikeResponse>();
            FilterLogger.Log(this, $"Spike Response?: {response}");

            if (response != null)
            {
                response.OnSpikeEnter(this);
            }

            return base.OnCollide(p, direction);
        }

        public override bool IsGround(PhysObj b) {
            return false;
        }

        public virtual void Discharge(HashSet<Spike> dogoDisabledSpikes) {
            Charged = false;
            _mySR.SetAlpha(0.2f);
            // _mySR.color = Color.red;
            if (_reEnableCoroutine != null) {
                StopCoroutine(_reEnableCoroutine);
                _reEnableCoroutine = null;
            }
            dogoDisabledSpikes.Add(this);
        }

        public void Recharge() {
            _reEnableCoroutine = StartCoroutine(Helper.DelayAction(RechargeTime, () => {
                Charged = true;
                _mySR.SetAlpha(1);
            }));
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }
    }
}
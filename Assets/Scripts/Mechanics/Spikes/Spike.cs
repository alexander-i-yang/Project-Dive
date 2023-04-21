using System;
using System.Collections.Generic;
using Helpers;
using MyBox;
using Phys;
using UnityEngine;
using World;

namespace Mechanics {
    public class Spike : Actor, IFilterLoggerTarget, IResettable {
        public bool Charged { get; private set; } = true;
        public float RechargeTime = 0.5f;
        private Coroutine _reEnableCoroutine;
        private SpriteRenderer _mySR;

        [SerializeField] private float recoilMultiplier = -1;

        protected new void Start()
        {
            base.Start();
        }

        private void OnEnable()
        {
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

        public override bool Squish(PhysObj p, Vector2 d)
        {
            return false;
        }

        public virtual void Discharge(HashSet<Spike> dogoDisabledSpikes)
        {
            DischargeLogic(dogoDisabledSpikes, DischargeAnimation);
        }

        protected void DischargeLogic(HashSet<Spike> dogoDisabledSpikes, Action dischargeAnimation)
        {
            Charged = false;
            dischargeAnimation();
            if (_reEnableCoroutine != null) {
                StopCoroutine(_reEnableCoroutine);
                _reEnableCoroutine = null;
            }
            dogoDisabledSpikes.Add(this);
        }

        protected void DischargeAnimation()
        {
            _mySR.SetAlpha(0.2f);
        }

        public void Recharge() {
            Charged = true;
            _reEnableCoroutine = StartCoroutine(
                Helper.DelayAction(RechargeTime, () => { _mySR.SetAlpha(1); })
            );
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }

        public void Reset()
        {
            Charged = true;
            _mySR.SetAlpha(1);
        }

        public bool CanReset()
        {
            return gameObject != null && gameObject.activeSelf;
        }

        public Vector2 RecoilFunc(Vector2 v)
        {
            return -v * recoilMultiplier;
        }
    }
}
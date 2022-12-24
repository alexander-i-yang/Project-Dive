using Helpers;
using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics {
    public class Spike : Solid {
        public bool Charged { get; private set; } = true;
        public float RechargeTime = 0.5f;
        private Coroutine _reEnableCoroutine;
        private SpriteRenderer _mySR; // for some reason autoProperty wasn't working for this

        protected new void Start() {
            base.Start();
            _mySR = GetComponent<SpriteRenderer>();
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            return p.EnterSpike(this);
        }

        public override bool IsGround(PhysObj b) {
            return false;
        }

        /*public void OnDrawGizmosSelected() {
            
        }*/

        public void DiveEnter() {
            Charged = false;
            _mySR.SetAlpha(0.2f);
            // _mySR.color = Color.red;
            if (_reEnableCoroutine != null) {
                StopCoroutine(_reEnableCoroutine);
                _reEnableCoroutine = null;
            }
        }

        public void Recharge() {
            _reEnableCoroutine = StartCoroutine(Helper.DelayAction(RechargeTime, () => {
                Charged = true;
                _mySR.SetAlpha(1);
            }));
        }
    }
}
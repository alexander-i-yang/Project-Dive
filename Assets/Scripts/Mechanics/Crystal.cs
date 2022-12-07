using System.Collections;
using Phys;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Mechanics {
    public class Crystal : Solid {
        private bool _broken = false;
        public double rechargeTime = 1;
        private SpriteRenderer _mySR;
        private Light2D _light;
        
        new void Start() {
            _mySR = GetComponent<SpriteRenderer>();
            _light = GetComponentInChildren<Light2D>();
            base.Start();
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            if (!_broken) return p.EnterCrystal(this);
            return false;
        }

        public override bool IsGround(PhysObj p) {
            return false;
        }

        public void Break() {
            _broken = true;
            StartCoroutine(BreakCoroutine());
        }

        public IEnumerator BreakCoroutine() {
            _mySR.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            var oldLightIntensity = _light.intensity;
            _light.intensity = 0;
            for (double i = 0; i < rechargeTime; i += Game.DeltaTime) {
                yield return null;
            }
            _mySR.color = Color.white;
            _broken = false;
            _light.intensity = oldLightIntensity;
        }
    }
}
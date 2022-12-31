using Core;

using System.Collections;
using Phys;
using Helpers;
using UnityEngine;


namespace Mechanics {
    public class Crystal : Solid, IFilterLoggerTarget {
        private bool _broken = false;
        public double rechargeTime = 1;
        private SpriteRenderer _mySR;
        private UnityEngine.Rendering.Universal.Light2D _light;

        new void Start() {
            _mySR = GetComponent<SpriteRenderer>();
            _light = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
            base.Start();
        }

        public override bool Collidable()
        {
            return false;
        }

        public override bool OnCollide(PhysObj p, Vector2 direction)
        {
            FilterLogger.Log(this, $"Crystal Collided with {p}");
            if (!_broken)
            {
                ICrystalResponse response = p.GetComponent<ICrystalResponse>();
                FilterLogger.Log(this, $"Crystal Response?: {response}");

                if (response != null)
                {
                    response.OnCrystalEnter(this);
                }
            }
            return base.OnCollide(p, direction);
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
            for (double i = 0; i < rechargeTime; i += Game.Instance.DeltaTime) {
                yield return null;
            }
            _mySR.color = Color.white;
            _broken = false;
            _light.intensity = oldLightIntensity;
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }
    }
}
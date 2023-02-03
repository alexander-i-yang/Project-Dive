using System;
using Core;

using System.Collections;
using Phys;
using Helpers;
using UnityEngine;
using World;


namespace Mechanics {
    public class Crystal : Solid, IResettable, IFilterLoggerTarget {
        private bool _broken = false;
        public double rechargeTime = 1;
        
        private SpriteRenderer _mySR;
        
        private UnityEngine.Rendering.Universal.Light2D _light;
        private float _lightIntensityStart;
        [SerializeField] private bool flicker;
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;
        
        private IEnumerator _breakCoroutine;
        private Floater _floater;

        new void Start() {
            _mySR = GetComponent<SpriteRenderer>();
            _floater = GetComponent<Floater>();
            _light = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
            _lightIntensityStart = _light.intensity;
            base.Start();
        }

        private void Update() {
            if (flicker && !_broken) Flicker();
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
            _breakCoroutine = BreakCoroutine();
            StartCoroutine(_breakCoroutine);
        }

        public IEnumerator BreakCoroutine() {
            Discharge();
            for (double i = 0; i < rechargeTime; i += Game.Instance.DeltaTime) {
                yield return null;
            }
            Recharge();
        }

        void Discharge()
        {
            _broken = true;
            _mySR.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            _light.intensity = 0;
            _floater.enabled = false;
        }

        void Recharge()
        {
            _mySR.color = Color.white;
            _broken = false;
            _light.intensity = _lightIntensityStart;
            _floater.enabled = true;
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }

        public void Flicker() {
            _light.intensity = _lightIntensityStart + Mathf.Sin (Game.Instance.Time * Mathf.PI * frequency+transform.position.x*0.1f) * amplitude;
        }

        public void Reset()
        {
            if (_breakCoroutine != null) StopCoroutine(_breakCoroutine);
            Recharge();
        }
    }
}
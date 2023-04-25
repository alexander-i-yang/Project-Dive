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
        
        private SpriteRenderer _sr;
        private ParticleSystem _particleSystem;
        private Color _dischargedColor = new(0.5f, 0.5f, 0.5f, 0.5f);
        
        private UnityEngine.Rendering.Universal.Light2D _light;
        private CrystalAnimationStateManager _animator;
        private float _lightIntensityStart = 5f;
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;
        
        private IEnumerator _breakCoroutine;
        private Floater _floater;

        public bool unlocked = false;

        private void OnEnable()
        {
            _sr = GetComponentInChildren<SpriteRenderer>(includeInactive:true);
            _floater = GetComponentInChildren<Floater>(includeInactive:true);
            _animator = GetComponentInChildren<CrystalAnimationStateManager>(includeInactive:true);
            _light = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>(includeInactive:true);
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            if (!unlocked) Discharge();
        }

        private void Update() {
            if (!_broken) Flicker();
        }

        public override bool Collidable()
        {
            return false;
        }

        public override bool OnCollide(PhysObj p, Vector2 direction)
        {
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
            //This eventually calls OnBounceAnimationEnd once the animation is done.
            _broken = true;
            _particleSystem.Emit(15);
            _animator.Play(CrystalAnimations.BOUNCE);
            for (double i = 0; i < rechargeTime; i += Game.Instance.DeltaTime) {
                yield return null;
            }
            Recharge();
        }
        
        public void OnBounceAnimationEnd()
        {
            Discharge();
            _animator.Play(CrystalAnimations.IDLE);
        }

        void Discharge()
        {
            _broken = true;
            _sr.color = _dischargedColor;
            _light.intensity = 0;
            _floater.enabled = false;
        }

        void Recharge()
        {
            _sr.color = Color.white;
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
            if (unlocked)
            {
                _animator.Play(CrystalAnimations.IDLE);
                Recharge();
            }
        }

        public bool CanReset()
        {
            return gameObject != null && gameObject.activeSelf && _animator != null && _animator.gameObject.activeSelf && unlocked;
        }

        public void Unlock()
        {
            unlocked = true;
            Recharge();
        }
    }
}
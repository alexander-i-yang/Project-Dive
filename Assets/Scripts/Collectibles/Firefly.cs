using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

using Helpers;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using World;

namespace Collectibles {
    public class Firefly : Collectible, IResettable, IFilterLoggerTarget
    {
        //[SerializeField] private Gate targetGate;

        private FireflyAnimator _animator;
        private FireflyAnimatorEnd _animatorEnd;

        private ParticleSystem[] _particleSystems;

        private bool _moving = false;
        private bool _collected = false;

        public override string ID => "Firefly";

        public static string s_ID => "Firefly";
    
        [SerializeField] private List<Vector2> _coords = null;
        private int _coordInd;

        private Vector3 _startPos;
        
        public delegate void CollectAnimFinish(int quantity);
        public static event CollectAnimFinish OnCollectAnimFinish;

        [SerializeField] private float timeTolerance;
        private GameTimer _movementTimer;
    
        private void Awake()
        {
            _coords = ReadCoords();
            _startPos = _coords[0];
        }

        private void OnEnable()
        {
            if (_collected) gameObject.SetActive(false);
            _animator = GetComponent<FireflyAnimator>();
            _animatorEnd = GetComponent<FireflyAnimatorEnd>();
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        public List<Vector2> ReadCoords()
        {
            List<Vector2> ret = new();
            FireflyPoint f;
            GameObject g = transform.parent.gameObject;
            while (g != null)
            {
                f = g.GetComponent<FireflyPoint>();
                ret.Add(f.transform.position);
                g = f.Next;
            }
            return ret;
        }

        void FixedUpdate()
        {
            GameTimer.FixedUpdate(_movementTimer);
        }

        public override void OnTouched(Collector collector)
        {
            bool shouldTouch = !_moving;
            
            if (GameTimer.GetTimerState(_movementTimer) == TimerState.Finished)
            {
                shouldTouch = true;
            }

            if (!shouldTouch) return;
            
            _moving = true;
            _movementTimer = GameTimer.StartNewTimer(1/_animator.GetAnimSpeed() - timeTolerance);
            FilterLogger.Log(this, $"{gameObject.name} Touched {collector}");
            _coordInd++;
            if (_coordInd < _coords.Count)
            {
                _animator.EndPos = _coords[_coordInd];
                _animator.PlayAnimation(OnTouchAnimFinish);
                // _particleSystem.emission.rateOverTime = ;
                /*StartCoroutine(Helper.DelayAction(
                    _animator.GetAnimSpeed() * graceTouchMultipier, 
                    () =>
                    {
                        print("Not moving");
                        _moving = false;
                    }
                ));*/
            }
            else
            {
                collector.OnCollectFinished(this);
                var pInventory = collector.GetComponent<PlayerInventory>();
                _collected = true;
                _animatorEnd.PlayAnimation(() => OnFinishCollected(pInventory));
            }
        }

        private void OnTouchAnimFinish()
        {
            _moving = false;
        }

        private void OnFinishCollected(PlayerInventory p)
        {
            _moving = false;
            Disable();
            OnCollectAnimFinish?.Invoke(p.NumCollectibles(s_ID));
        }

        private void Disable()
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponentInChildren<Light2D>().gameObject.SetActive(false);
            GetComponentInChildren<Collider2D>().enabled = false;
            GetComponentInChildren<Floater>().enabled = false;
            foreach (var p in _particleSystems)
            {
                var emissionModule = p.emission;
                emissionModule.enabled = false;
                // p.SetCustomParticleData(emissionModule);
            }
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }

        public void Reset()
        {
            if (_collected) return;
            foreach (var p in _particleSystems) if (p != null) p.Pause();
            transform.position = _startPos;
            _coordInd = 0;
            _moving = false;
            _animator.StopAnimation();
            _animatorEnd.StopAnimation();
            foreach (var p in _particleSystems) if (p != null) p?.Play();
        }

        public bool CanReset()
        {
            return !_collected && gameObject.activeSelf;
        }
    }
}
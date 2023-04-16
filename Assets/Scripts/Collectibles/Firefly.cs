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
        [SerializeField] private float graceTouchMultipier;

        public override string ID => "Firefly";

        public static string s_ID => "Firefly";
    
        [SerializeField] private List<Vector2> _coords = null;
        private int _coordInd;

        private Vector3 _startPos;
    
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

        public override void OnTouched(Collector collector)
        {
            if (!_moving)
            {
                _moving = true;
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
                    _animatorEnd.PlayAnimation(() => OnFinishCollected(collector));
                }
            }
        }

        private void OnTouchAnimFinish()
        {
            _moving = false;
            
            Collider2D[] hits = new Collider2D[0];
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.NameToLayer("Interactable"));
            GetComponent<Collider2D>().OverlapCollider(filter, hits);

            foreach (var hit in hits)
            {
                print(hit);
            }
        }

        private void OnFinishCollected(Collector collector)
        {
            collector.OnCollectFinished(this);
            _moving = false;
            Disable();
            _collected = true;
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
            foreach (var p in _particleSystems) p.Pause();
            transform.position = _startPos;
            _coordInd = 0;
            _moving = false;
            _animator.StopAnimation();
            _animatorEnd.StopAnimation();
            foreach (var p in _particleSystems) p.Play();
        }

        public bool CanReset()
        {
            return !_collected && gameObject.activeSelf;
        }
    }
}
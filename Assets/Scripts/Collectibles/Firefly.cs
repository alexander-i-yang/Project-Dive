using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using MyBox;

using Helpers;
using Mechanics;

namespace Collectibles {
    public class Firefly : Collectible, IFilterLoggerTarget
    {
        //[SerializeField] private Gate targetGate;

        private FireflyAnimator _animator;
        private FireflyAnimatorEnd _animatorEnd;

        private bool _moving = false;

        public override string ID => "Firefly";

        public static string s_ID => "Firefly";
    
        [SerializeField] private List<Vector2> _coords = null;
        private int _coordInd;

        public GameObject Next;
    
        private void Awake()
        {
            _animator = GetComponent<FireflyAnimator>();
            _animatorEnd = GetComponent<FireflyAnimatorEnd>();
            _coords = ReadCoords(Next);
            foreach (var i in _coords) print(i);
        }
        
        public List<Vector2> ReadCoords(GameObject g)
        {
            List<Vector2> ret = new();
            FireflyPoint f;
            print("READ COORDS");
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
                print(_coordInd);
                if (_coordInd < _coords.Count)
                {
                    _animator.EndPos = _coords[_coordInd];
                    _animator.PlayAnimation(OnTouchAnimFinish);
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
            _coordInd++;
        }

        private void OnFinishCollected(Collector collector)
        {
            collector.OnCollectFinished(this);
            _moving = false;
            Destroy(gameObject);
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
}
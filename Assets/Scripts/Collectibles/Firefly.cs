using System.Collections.Generic;
using UnityEngine;

using Helpers;
using UnityEditor;
using World;

namespace Collectibles {
    public class Firefly : Collectible, IResettable, IFilterLoggerTarget
    {
        //[SerializeField] private Gate targetGate;

        private FireflyAnimator _animator;
        private FireflyAnimatorEnd _animatorEnd;

        private bool _moving = false;
        [SerializeField] private float graceTouchMultipier;

        public override string ID => "Firefly";

        public static string s_ID => "Firefly";
    
        [SerializeField] private List<Vector2> _coords = null;
        private int _coordInd;

        public GameObject Next;
        private Vector3 _startPos;
    
        private void Awake()
        {
            _animator = GetComponent<FireflyAnimator>();
            _animatorEnd = GetComponent<FireflyAnimatorEnd>();
            _coords = ReadCoords(Next);
            _startPos = transform.position;
        }
        
        public List<Vector2> ReadCoords(GameObject g)
        {
            List<Vector2> ret = new();
            FireflyPoint f;
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
                if (_coordInd < _coords.Count)
                {
                    _animator.EndPos = _coords[_coordInd];
                    _animator.PlayAnimation(OnTouchAnimFinish);
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
            print("Finish");
            _coordInd++;
        }

        private void OnFinishCollected(Collector collector)
        {
            collector.OnCollectFinished(this);
            _moving = false;
            gameObject.SetActive(false);
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }

        public void Reset()
        {
            transform.position = _startPos;
            _coordInd = 0;
            _moving = false;
            _animator.StopAnimation();
            _animatorEnd.StopAnimation();
        }
    }
}
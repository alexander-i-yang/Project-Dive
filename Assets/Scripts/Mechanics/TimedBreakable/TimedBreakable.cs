using System;
using System.Collections.Generic;
using Helpers;
using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics {
    public class TimedBreakable : Solid {
        // public GameObject ParticlePrefab;
        //private bool _breaking = true;
        private GameTimer _breakingTimer;
        [AutoProperty, SerializeField] private SpriteRenderer _spriteR;
        [SerializeField] private float BreakTime = 0.16f;
        
        public override bool Collidable() {
            return true;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            /*if (p.IsDiving() && direction.y < 0) {
                Break();
                return false;
            }
            
            return base.PlayerCollide(p, direction);*/
            ITimedBreakableResponse response = p.GetComponent<ITimedBreakableResponse>();
            // FilterLogger.Log(this, $"Spike Response?: {response}");

            if (response != null)
            {
                return response.OnBreakableEnter(this);
            }

            return base.OnCollide(p, direction);
        }

        public override bool IsGround(PhysObj whosAsking) {
            /*PlayerActor p = whosAsking as PlayerActor;
            if (p != null) {
                return !p.IsDiving();
            }*/
            return true;
        }

        public void Break() {
            gameObject.SetActive(false);
            // Instantiate(ParticlePrefab, transform.position, Quaternion.identity);
        }

        public void StartBreaking(HashSet<TimedBreakable> _dogoingBreakables)
        {
            TimerState tState = GameTimer.GetTimerState(_breakingTimer);
            Debug.Log(tState);
            if (_breakingTimer == null || tState == TimerState.Inactive)
            {
                _breakingTimer = GameTimer.StartNewTimer(BreakTime);
            }
            else if(tState == TimerState.Paused)
            {
                GameTimer.UnPause(_breakingTimer);
            }
            else if (tState == TimerState.Running)
            {
                Debug.Log(_breakingTimer.TimerValue);
            }
            _dogoingBreakables.Add(this);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            GameTimer.FixedUpdate(_breakingTimer);
            var breakStatus = GameTimer.GetTimerState(_breakingTimer);
            if (breakStatus == TimerState.Running)
            {
                var time = _breakingTimer.TimerValue;
                Debug.Log(time);
                _spriteR.SetAlpha(time/BreakTime);
            } else if (breakStatus == TimerState.Finished)
            {
                Break();
            }
        }

        /*public void StopBreaking()
        {
            GameTimer.Pause(_breakingTimer);
        }*/
    }
}
using Helpers;
using MyBox;
using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Dead : PlayerState
        {
            private GameTimer _deathTimer;
            
            public override void Enter(PlayerStateInput i)
            {
                MySM._deathAnim.Trigger();
                /*MySM._playerAnim.StartCoroutine(Helper.DelayAction(0.1f, () =>
                {
                    
                }));*/
                // _deathTimer = GameTimer.StartNewTimer(PlayerCore.DeathTime);
            }

            /*
            public override void Update()
            {
                GameTimer.Update(_deathTimer);
                if (GameTimer.GetTimerState(_deathTimer) == TimerState.Finished)
                {
                    MySM.OnPlayerRespawn?.Invoke();
                }
            }*/
        }
    }
}
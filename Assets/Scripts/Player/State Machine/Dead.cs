using Helpers;
using MyBox;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Dead : PlayerState
        {
            private GameTimer _deathTimer;
            
            public override void Enter(PlayerStateInput i)
            {
                _deathTimer = GameTimer.StartNewTimer(PlayerCore.DeathTime);
            }

            public override void Update()
            {
                GameTimer.Update(_deathTimer);
                if (GameTimer.GetTimerState(_deathTimer) == TimerState.Finished)
                {
                    MySM.OnPlayerRespawn?.Invoke();
                }
            }
        }
    }
}
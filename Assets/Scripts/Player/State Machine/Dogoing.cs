using Core;

using System;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Dogoing : PlayerState
        {
            private double _oldVelocity;
            private double _dogoXVBufferTimer;

            public override void Enter(PlayerStateInput i)
            {
                _oldVelocity = Player.Dogo();
                _dogoXVBufferTimer = Player.DogoConserveXVTime;
            }

            public override void Exit(PlayerStateInput i)
            {
                i.OldVelocity = _oldVelocity;
                i.DogoXVBufferTimer = _dogoXVBufferTimer;
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                MySM.Transition<DogoJumping>();
            }

            public override void FixedUpdate()
            {
                if (_dogoXVBufferTimer > 0)
                {
                    _dogoXVBufferTimer = Math.Max(0, _dogoXVBufferTimer - Game.Instance.FixedDeltaTime);
                }

                base.FixedUpdate();
            }
        }
    }
}
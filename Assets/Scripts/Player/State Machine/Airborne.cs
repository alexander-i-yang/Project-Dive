using Core;

using System;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Airborne : PlayerState
        {
            private float _jumpCoyoteTimer;

            public override void Enter(PlayerStateInput i)
            {
                _jumpCoyoteTimer = Player.JumpCoyoteTime;
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                bool justLeftGround = _jumpCoyoteTimer > 0 && !MySM._jumpedFromGround;
                if (justLeftGround)
                {
                    MySM.JumpFromInput();
                }
                else if (MySM._canDoubleJump)
                {
                    Player.DoubleJump();
                    MySM._canDoubleJump = false;
                }
            }

            public override void JumpReleased()
            {
                base.JumpReleased();
                Player.TryJumpCut();
            }

            public override void DivePressed()
            {
                base.DivePressed();
                if (MySM._canDive)
                {
                    MySM.Transition<Diving>();
                }
            }

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (isGrounded)
                {
                    MySM.Transition<Grounded>();
                }
            }

            public override void MoveX(int moveDirection)
            {
                Player.UpdateMovementX(Player.MaxAirAcceleration);
            }

            public override void FixedUpdate()
            {
                Player.Fall();
                if (_jumpCoyoteTimer > 0)
                {
                    _jumpCoyoteTimer = Math.Max(0, _jumpCoyoteTimer - Game.Instance.FixedDeltaTime);
                }
            }
        }
    }
}
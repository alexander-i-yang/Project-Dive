using Helpers;

using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Airborne : PlayerState
        {
            private GameTimer _jumpCoyoteTimer;

            public override void Enter(PlayerStateInput i)
            {
                if (!Input.jumpedFromGround)
                {
                    _jumpCoyoteTimer = GameTimer.StartNewTimer(PlayerInfo.JumpCoyoteTime, "Jump Coyote Timer");
                }
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                TimerState coyoteState = GameTimer.GetTimerState(_jumpCoyoteTimer);
                if (coyoteState == TimerState.Running)
                {
                    JumpFromGround();
                }
                else if (Input.canDoubleJump)
                {
                    DoubleJump();
                }
            }

            public override void JumpReleased()
            {
                base.JumpReleased();
                TryJumpCut();
            }

            public override void DivePressed()
            {
                base.DivePressed();
                if (Input.canDive)
                {
                    MySM.Transition<Diving>();
                }
            }

            public override void SetGrounded(bool isGrounded, bool isMovingUp)
            {
                base.SetGrounded(isGrounded, isMovingUp);
                if (!isMovingUp && isGrounded) {
                    MySM.Transition<Grounded>();
                }
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
                PlayerAction.UpdateMovementX(moveDirection, PlayerInfo.MaxAirAcceleration);
            }

            public override void FixedUpdate()
            {
                PlayerAction.Fall();
                GameTimer.FixedUpdate(_jumpCoyoteTimer);
            }
        }
    }
}
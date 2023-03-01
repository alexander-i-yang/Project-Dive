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
                PlayerAnim.Play(PlayerAnimations.JUMP_INIT);
                if (!Input.jumpedFromGround)
                {
                    _jumpCoyoteTimer = GameTimer.StartNewTimer(PlayerCore.JumpCoyoteTime, "Jump Coyote Timer");
                }
            }

            public override void JumpPressed()
            {
                if (Input.canDoubleJump)
                {
                    DoubleJump();
                    return;
                }
                
                base.JumpPressed();
                TimerState coyoteState = GameTimer.GetTimerState(_jumpCoyoteTimer);
                if (coyoteState == TimerState.Running)
                {
                    JumpFromGround();
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
                    PlayerAnim.Play(PlayerAnimations.LANDING);
                    MySM.Transition<Grounded>();
                }
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
                Actor.UpdateMovementX(moveDirection, PlayerCore.MaxAirAcceleration);
            }

            public override void FixedUpdate()
            {
                Actor.Fall();
                GameTimer.FixedUpdate(_jumpCoyoteTimer);
            }
        }
    }
}
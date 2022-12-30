using Helpers;

using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine {
        public class Grounded : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                Input.jumpedFromGround = false;
                RefreshAbilities();
                PlayerAction.Land();
                if (GameTimer.GetTimerState(Input.jumpBufferTimer) == TimerState.Running && !MySM.PrevStateEquals<Diving>())
                {
                    JumpFromGround();
                }
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                JumpFromGround();
            }

            public override void SetGrounded(bool isGrounded, bool isMovingUp)
            {
                base.SetGrounded(isGrounded, isMovingUp);
                if (!isGrounded)
                {
                    MySM.Transition<Airborne>();
                }
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
                PlayerAnim.ChangeState(moveDirection != 0 ? PlayerAnimations.RUNNING : PlayerAnimations.IDLE);
                int acceleration = moveDirection == 0 ? PlayerInfo.MaxAcceleration : PlayerInfo.MaxDeceleration;
                PlayerAction.UpdateMovementX(moveDirection, acceleration);
            }
        }
    }
}
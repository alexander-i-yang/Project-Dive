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

                Player.Land();
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

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (!isGrounded)
                {
                    Debug.Log("Grounded to Airborne");
                    MySM.Transition<Airborne>();
                }
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
                int acceleration = moveDirection == 0 ? Player.MaxAcceleration : Player.MaxDeceleration;
                Player.UpdateMovementX(moveDirection, acceleration);
            }
        }
    }
}
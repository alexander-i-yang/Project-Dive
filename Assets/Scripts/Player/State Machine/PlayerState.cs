using Helpers;
using Mechanics;

using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput>
        {
            public PlayerActor Actor => PlayerCore.Actor;
            public PlayerSpawnManager SpawnManager => PlayerCore.SpawnManager;
            public PlayerAnimationStateManager PlayerAnim => MySM._playerAnim;

            public virtual void JumpPressed()
            {
                Input.jumpBufferTimer = GameTimer.StartNewTimer(PlayerCore.JumpBufferTime, "Jump Buffer Timer");
            }

            public virtual void JumpReleased() { }
            public virtual void DivePressed() { }
            public virtual void SetGrounded(bool isGrounded, bool isMovingUp) { }
            public virtual void MoveX(int moveDirection) { }

            public void RefreshAbilities()
            {
                Input.canDoubleJump = true;
                Input.canDive = true;
            }

            protected void UpdateSpriteFacing(int moveDirection)
            {
                if (moveDirection != 0)
                {
                    MySM._spriteR.flipX = moveDirection == -1;
                }
            }

            protected void JumpFromGround()
            {
                Input.jumpedFromGround = true;
                Input.canJumpCut = true;
                PlayerAnim.Play(PlayerAnimations.JUMP_INIT);
                Actor.Jump(PlayerCore.JumpHeight);
                SetGrounded(false, true);
                //GameTimer.Clear(Input.jumpBufferTimer);
            }

            protected void DoubleJump()
            {
                Input.canJumpCut = true;
                Actor.DoubleJump(PlayerCore.DoubleJumpHeight, Input.moveDirection);
                Input.canDoubleJump = false;
                SetGrounded(false, true);
            }

            protected void TryJumpCut()
            {
                if (Input.canJumpCut)
                {
                    Actor.JumpCut();
                    Input.canJumpCut = false;
                }
            }
        }
    }
}
﻿using Helpers;
using Mechanics;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput>
        {
            public PlayerActor PlayerActions => PlayerCore.Actor;
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

            public virtual void OnDeath() {
                SpawnManager.Respawn();
                MySM.Transition<Grounded>();
                MySM.OnPlayerDeath();
            }

            public void RefreshAbilities()
            {
                Input.canDoubleJump = true;
                Input.canDive = true;
            }

            protected void UpdateSpriteFacing(int moveDirection)
            {
                if (moveDirection != 0)
                {

                    MySM.CurrInput.facing = moveDirection;
                    MySM._spriteR.flipX = moveDirection == -1;
                }
            }

            protected void JumpFromGround()
            {
                Input.jumpedFromGround = true;
                Input.canJumpCut = true;
                PlayerAnim.Play(PlayerAnimations.JUMP_INIT);
                PlayerActions.Jump(PlayerCore.JumpHeight);
                SetGrounded(false, true);
                //GameTimer.Clear(Input.jumpBufferTimer);
            }

            protected void DoubleJump()
            {
                Input.canJumpCut = true;
                PlayerActions.DoubleJump(PlayerCore.DoubleJumpHeight, Input.moveDirection);
                Input.canDoubleJump = false;
                SetGrounded(false, true);
            }

            protected void TryJumpCut()
            {
                if (Input.canJumpCut)
                {
                    PlayerActions.JumpCut();
                    Input.canJumpCut = false;
                }
            }
        }
    }
}
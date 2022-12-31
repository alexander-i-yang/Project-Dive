using Helpers;
using Mechanics;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput>
        {
            public IPlayerInfoProvider PlayerInfo => MySM._playerInfo;
            public IPlayerActionHandler PlayerAction => MySM._playerAction;
            public PlayerSpawnManager SpawnManager => MySM._spawnManager;
            public PlayerAnimationStateManager PlayerAnim => MySM._playerAnim;

            public virtual void JumpPressed()
            {
                Input.jumpBufferTimer = GameTimer.StartNewTimer(PlayerInfo.JumpBufferTime, "Jump Buffer Timer");
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
                    MySM._spriteR.flipX = moveDirection == -1;
                }
            }

            protected void JumpFromGround()
            {
                Input.jumpedFromGround = true;
                Input.canJumpCut = true;
                PlayerAction.Jump();
                SetGrounded(false, true);

                //GameTimer.Clear(Input.jumpBufferTimer);
            }

            protected void DoubleJump()
            {
                Input.canJumpCut = true;
                PlayerAction.DoubleJump(Input.moveDirection);
                Input.canDoubleJump = false;
                SetGrounded(false, true);
            }

            protected void TryJumpCut()
            {
                if (Input.canJumpCut)
                {
                    PlayerAction.JumpCut();
                    Input.canJumpCut = false;
                }
            }
        }
    }
}
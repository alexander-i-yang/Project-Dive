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

            public virtual void JumpPressed()
            {
                Input.jumpBufferTimer = GameTimer.StartNewTimer(PlayerInfo.JumpBufferTime, "Jump Buffer Timer");
            }

            public virtual void JumpReleased() { }
            public virtual void DivePressed() { }
            public virtual void SetGrounded(bool isGrounded) { }
            public virtual bool EnterCrystal(Crystal c) { return false; }
            public virtual void MoveX(int moveDirection) { }

            public virtual void OnDeath() {
                SpawnManager.Respawn();
            }

            public virtual bool EnterSpike(Spike spike)
            {
                if (spike.Charged)
                {
                    PlayerAction.Die();
                }
                return false;
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
                SetGrounded(false);

                //GameTimer.Clear(Input.jumpBufferTimer);
            }

            protected void DoubleJump()
            {
                Input.canJumpCut = true;
                PlayerAction.DoubleJump(Input.moveDirection);
                Input.canDoubleJump = false;
                SetGrounded(false);
            }

            protected void TryJumpCut()
            {
                if (Input.canJumpCut)
                {
                    PlayerAction.JumpCut();
                    Input.canJumpCut = false;
                }
            }

            protected void RefreshAbilities()
            {
                Input.canDoubleJump = true;
                Input.canDive = true;
            }
        }
    }
}
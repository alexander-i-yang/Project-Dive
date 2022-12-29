using Helpers;
using Mechanics;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput>
        {
            public PlayerActor Player => MySM._player;

            public virtual void JumpPressed()
            {
                Input.jumpBufferTimer = GameTimer.StartNewTimer(Player.JumpBufferTime, "Jump Buffer Timer");
            }

            public virtual void JumpReleased() { }
            public virtual void DivePressed() { }
            public virtual void SetGrounded(bool isGrounded) { }
            public virtual bool EnterCrystal(Crystal c) { return false; }
            public virtual void MoveX(int moveDirection) { }

            public virtual bool EnterSpike(Spike spike)
            {
                if (spike.Charged)
                {
                    Player.Die();
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
                Player.Jump();

                //GameTimer.Clear(Input.jumpBufferTimer);
            }

            protected void DoubleJump()
            {
                Input.canJumpCut = true;
                Player.DoubleJump(Input.moveDirection);
                Input.canDoubleJump = false;
            }

            protected void TryJumpCut()
            {
                if (Input.canJumpCut)
                {
                    Player.JumpCut();
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
using Helpers;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Dogoing : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                i.oldVelocity = Player.Dogo();
                i.dogoXVBufferTimer = GameTimer.StartNewTimer(Player.DogoConserveXVTime);
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                MySM.Transition<DogoJumping>();
            }

            public override void FixedUpdate()
            {
                GameTimer.FixedUpdate(Input.dogoXVBufferTimer);

                base.FixedUpdate();
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
            }
        }
    }
}
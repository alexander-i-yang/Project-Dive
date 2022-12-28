namespace Player
{
    public partial class PlayerStateMachine {
        public class Grounded : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                MySM._jumpedFromGround = false;
                MySM.Refill();

                Player.Land();
                if (MySM.JumpBufferTimer > 0 && !MySM.PrevStateEquals<Diving>())
                {
                    MySM.JumpFromInput();
                }
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                MySM.JumpFromInput();
            }

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (!isGrounded) MySM.Transition<Airborne>();
            }

            public override void MoveX(int moveDirection)
            {
                Player.UpdateMovementX(moveDirection == 0 ? Player.MaxAcceleration : Player.MaxDeceleration);
            }
        }
    }
}
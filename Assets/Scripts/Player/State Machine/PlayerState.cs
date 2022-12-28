using Mechanics;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public abstract class PlayerState : State<PlayerStateMachine, PlayerState, PlayerStateInput>
        {
            public PlayerActor Player => MySM.Player;   //This is just a shortcut because it's used so much.
            //L: Changed from abstract functions to virtual bc not every state needs to implement
            public virtual void JumpPressed()
            {
                MySM.JumpBufferTimer = MySM.Player.JumpBufferTime;
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
                    MySM.Player.Die();
                }
                return false;
            }
        }
    }
}
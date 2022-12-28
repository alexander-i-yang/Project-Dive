using System.Collections.Generic;
using Mechanics;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Diving : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                MySM.Player.Dive();
                MySM._canDive = false;
                MySM.DogoDisableSpikes = new HashSet<Spike>();
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                if (MySM._canDoubleJump)
                {
                    MySM.Player.DoubleJump();
                    MySM._canDoubleJump = false;
                    MySM.Transition<Airborne>();
                }
            }

            public override void SetGrounded(bool isGrounded)
            {
                base.SetGrounded(isGrounded);
                if (isGrounded)
                {
                    MySM.Transition<Dogoing>();
                }
            }

            public override void FixedUpdate()
            {
                if (MySM.Player.FallVelocityExceedsMax())
                {
                    MySM.Player.DiveDecelUpdate();
                }
                else
                {
                    MySM.Player.Fall();
                }
            }

            public override bool EnterCrystal(Crystal c)
            {
                MySM.Transition<Airborne>();
                MySM.Player.CrystalJump();
                MySM.Refill();
                c.Break();
                return false;
            }

            public override bool EnterSpike(Spike s)
            {
                MySM.DogoDisableSpikes.Add(s);
                s.DiveEnter();
                return false;
            }
        }
    }
}
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
                Player.Dive();
                Input.canDive = false;
                Input.dogoDisabledSpikes = new HashSet<Spike>();
            }

            public override void JumpPressed()
            {
                base.JumpPressed();
                if (Input.canDoubleJump)
                {
                    DoubleJump();
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
                if (Player.FallVelocityExceedsMax())
                {
                    Player.DiveDecelUpdate();
                }
                else
                {
                    Player.Fall();
                }
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
            }

            public override bool EnterCrystal(Crystal c)
            {
                Input.canJumpCut = false;
                Player.CrystalJump();
                RefreshAbilities();
                c.Break();
                MySM.Transition<Airborne>();
                return false;
            }

            public override bool EnterSpike(Spike s)
            {
                Input.dogoDisabledSpikes.Add(s);
                s.DiveEnter();
                return false;
            }
        }
    }
}
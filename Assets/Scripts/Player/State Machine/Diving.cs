using System.Collections.Generic;
using Mechanics;
using UnityEngine;

namespace Player
{
    public partial class PlayerStateMachine
    {
        public class Diving : PlayerState
        {
            public override void Enter(PlayerStateInput i)
            {
                PlayAnimation(PlayerAnimations.DIVING);
                Actor.Dive();
                Input.canDive = false;
                Input.canJumpCut = false;
                Input.dogoDisabledSpikes = new HashSet<Spike>();
                
                // var divePEmission = MySM._diveParticles.emission;
                // divePEmission.enabled = true;
                
                MySM._screenshakeActivator.ScreenShakeContinuousOn(MySM._screenshakeActivator.DiveData);
            }

            public override void Exit(PlayerStateInput i)
            {
                MySM._screenshakeActivator.ScreenShakeContinuousOff(MySM._screenshakeActivator.DiveData);
                // var divePEmission = MySM._diveParticles.emission;
                // divePEmission.enabled = false;
                base.Exit(i);
                
                //MySM._drillEmitter.Stop();
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

            public override void SetGrounded(bool isGrounded, bool isMovingUp)
            {
                base.SetGrounded(isGrounded, isMovingUp);
                if (isGrounded)
                {
                    MySM.Transition<Dogoing>();
                }
            }

            public override void FixedUpdate() {
                Actor.UpdateWhileDiving();
            }

            public override void MoveX(int moveDirection)
            {
                Actor.UpdateMovementX(moveDirection, PlayerCore.MaxAirAcceleration);
                UpdateSpriteFacing(moveDirection);
            }
        }
    }
}
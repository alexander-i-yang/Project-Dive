﻿using System.Collections.Generic;
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
                PlayerAnim.Play(PlayerAnimations.DIVING);
                PlayerActions.Dive();
                Input.canDive = false;
                Input.canJumpCut = false;
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

            public override void SetGrounded(bool isGrounded, bool isMovingUp)
            {
                base.SetGrounded(isGrounded, isMovingUp);
                if (isGrounded)
                {
                    MySM.Transition<Dogoing>();
                }
            }

            public override void FixedUpdate() {
                PlayerActions.UpdateWhileDiving();
            }

            public override void MoveX(int moveDirection)
            {
                PlayerActions.UpdateMovementX(moveDirection, PlayerCore.MaxAirAcceleration);
                UpdateSpriteFacing(moveDirection);
            }
        }
    }
}
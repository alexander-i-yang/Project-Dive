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
                PlayerAction.Dive();
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

            public override void SetGrounded(bool isGrounded, bool isMovingUp)
            {
                base.SetGrounded(isGrounded, isMovingUp);
                if (isGrounded)
                {
                    MySM.Transition<Dogoing>();
                }
            }

            public override void FixedUpdate() {
                PlayerAction.UpdateWhileDiving();
            }

            public override void MoveX(int moveDirection)
            {
                UpdateSpriteFacing(moveDirection);
            }

            public override void EnterDiveMechanic(IDiveMechanic d) {
                Input.canJumpCut = false;
                RefreshAbilities();
                if (d.OnDiveEnter(PlayerAction)) {
                    MySM.Transition<Airborne>();
                }
            }

            public override bool EnterSpike(Spike s) {
                Input.dogoDisabledSpikes.Add(s);
                s.DiveEnter();
                return false;
            }
        }
    }
}
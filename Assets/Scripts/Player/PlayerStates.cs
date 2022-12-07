using System;
using Mechanics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Player {
    public class Grounded : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.JumpedFromGround = false;
            MySM.JumpBeingHeld = false;
            MySM.pActor.Land();
            MySM.DoubleJumpLeft = true;
            MySM.DiveLeft = true;
            if (MySM.jumpBufferTimer > 0 && !MySM.PrevStateEquals<Diving>()) {
                JumpFromGround();
            }
        }

        public override void JumpPressed() {
            JumpFromGround();
        }

        public override void SetGrounded(bool isGrounded) {
            if (!isGrounded) MySM.Transition<Airborne>();
        }

        private void JumpFromGround()
        {
            MySM.pActor.Jump();
            MySM.JumpedFromGround = true;
            MySM.JumpBeingHeld = true;
        }
    }

    public class Airborne : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.jumpCoyoteTimer = MySM.pActor.JumpCoyoteTime;
        }

        public override void JumpPressed() {
            bool justLeftGround = MySM.jumpCoyoteTimer > 0 && !MySM.JumpedFromGround;
            if (justLeftGround) {
                MySM.pActor.Jump();
                MySM.JumpBeingHeld = true;
            } else if (MySM.DoubleJumpLeft) {
                MySM.Transition<DoubleJumping>();
            }
        }

        public override void JumpReleased()
        {
            MySM.JumpCut();
        }

        public override void DivePressed() {
            if (MySM.DiveLeft) {
                MySM.Transition<Diving>();
            }
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) {
                MySM.Transition<Grounded>();
            }
        }

        public override void FixedUpdate() {
            MySM.pActor.Fall();
            if (MySM.jumpCoyoteTimer > 0) {
                MySM.jumpCoyoteTimer = Math.Max(0, MySM.jumpCoyoteTimer - Game.FixedDeltaTime);
            }
        }
    }

    public class DoubleJumping : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.pActor.DoubleJump();
            MySM.DoubleJumpLeft = false;
            MySM.JumpBeingHeld = true;
        }

        public override void JumpReleased()
        {
            MySM.JumpCut();
        }

        public override void DivePressed() {
            if (MySM.DiveLeft) {
                MySM.Transition<Diving>();
            }
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) {
                MySM.Transition<Grounded>();
            }
        }

        public override void FixedUpdate() {
            MySM.pActor.Fall();
        }
    }

    public class Diving : PlayerState {
        private bool _diveDone = false;

        public override void Enter(PlayerStateInput i) {
            MySM.pActor.Dive();
            MySM.DiveLeft = false;
            _diveDone = false;
        }

        public override void JumpPressed() {
            if (MySM.DoubleJumpLeft) {
                MySM.Transition<DoubleJumping>();
            }
        }

        public override void SetGrounded(bool isGrounded)
        {
            if (isGrounded)
            {
                MySM.Transition<Grounded>();
            }
        }

        public override void FixedUpdate() {
            if (_diveDone) {
                MySM.pActor.Fall();
            } else {
                _diveDone = MySM.pActor.DiveDecelUpdate();
            }
        }

        public override void EnterCrystal(Crystal c) {
            MySM.Transition<Airborne>();
            MySM.pActor.Jump();
            MySM.DoubleJumpLeft = true;
            MySM.DiveLeft = true;
            c.Break();
        }
    }
}
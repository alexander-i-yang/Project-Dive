using System;
using DefaultNamespace;
using Mechanics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Player {
    public class Grounded : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.JumpedFromGround = false;
            MySM.P.Land();
            MySM.DoubleJumpLeft = true;
            MySM.DiveLeft = true;
            if (MySM.JJP > 0 && !MySM.PrevStateEquals<Diving>()) {
                Jump();
            }
        }

        private void Jump() {
            MySM.P.Jump();
            MySM.JumpedFromGround = true;
        }

        public override void SetJumpPressed(bool pressed) {
            if (pressed) {
                Jump();
            }
        }

        public override void SetDivePressed(bool pressed) {
        }


        public override void SetGrounded(bool isGrounded) {
            if (!isGrounded) MySM.Transition<Airborne>();
        }

        public override bool EnterCrystal(Crystal c) {
            return false;
        }
    }

    public class Airborne : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.CoyoteTime = MySM.P.CoyoteTime;
        }

        public override void SetJumpPressed(bool pressed) {
            if (pressed) {
                //Just left the ground
                if (MySM.CoyoteTime > 0 && !MySM.JumpedFromGround) {
                    MySM.P.Jump();
                } else if (MySM.DoubleJumpLeft) {
                    //Otherwise, double jump
                    MySM.Transition<DoubleJumping>();
                }
            }
        }

        public override void SetDivePressed(bool pressed) {
            if (pressed && MySM.DiveLeft) {
                MySM.Transition<Diving>();
            }
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) {
                MySM.Transition<Grounded>();
            }
        }

        public override void FixedUpdate() {
            MySM.P.Fall();
            if (MySM.CoyoteTime > 0) {
                MySM.CoyoteTime = Math.Max(0, MySM.CoyoteTime - Game.FixedDeltaTime);
            }
        }

        public override bool EnterCrystal(Crystal c) {
            return false;
        }
    }

    public class DoubleJumping : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.P.DoubleJump();
            MySM.DoubleJumpLeft = false;
        }

        public override void SetJumpPressed(bool pressed) {
            
        }
        
        public override void SetDivePressed(bool pressed) {
            if (pressed && MySM.DiveLeft) {
                MySM.Transition<Diving>();
            }
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) {
                MySM.Transition<Grounded>();
            }
        }

        public override void FixedUpdate() {
            MySM.P.Fall();
        }

        public override bool EnterCrystal(Crystal c) {
            return false;
        }
    }

    public class Diving : PlayerState {
        private bool _diveDone = false;
        public override void Enter(PlayerStateInput i) {
            MySM.P.Dive();
            MySM.DiveLeft = false;
            _diveDone = false;
        }

        public override void SetJumpPressed(bool pressed) {
            if (pressed && MySM.DoubleJumpLeft) {
                MySM.Transition<DoubleJumping>();
            }
        }

        public override void FixedUpdate() {
            if (_diveDone) {
                MySM.P.Fall();
            } else {
                _diveDone = MySM.P.DiveDecelUpdate();
            }
        }

        public override bool EnterCrystal(Crystal c) {
            MySM.Transition<Airborne>();
            MySM.P.Jump();
            MySM.DoubleJumpLeft = true;
            MySM.DiveLeft = true;
            c.Break();
            return false;
        }

        public override void SetDivePressed(bool pressed) {
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) {
                MySM.Transition<Grounded>();
            }
        }
    }
}
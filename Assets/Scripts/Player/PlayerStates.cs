using DefaultNamespace;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Player {
    public class Grounded : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.JumpedFromGround = false;
            MySM.P.Land();
            MySM.DoubleJumpLeft = true;
            MySM.DiveLeft = true;
        }

        public override void SetJumpPressed(bool pressed) {
            if (pressed) {
                MySM.P.Jump();
                MySM.JumpedFromGround = true;
            }
        }

        public override void SetDivePressed(bool pressed) {
        }


        public override void SetGrounded(bool isGrounded) {
            if (!isGrounded) MySM.Transition<Airborne>();
        }
    }

    public class Airborne : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.CoyoteTime = MySM.P.CoyoteTime;
        }

        public override void Update(PlayerStateInput i) {
            if (MySM.CoyoteTime > 0) {
                MySM.CoyoteTime--;
            }
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
    }

    public class Diving : PlayerState {
        public override void Enter(PlayerStateInput i) {
            MySM.P.Dive();
            MySM.DiveLeft = false;
        }

        public override void SetJumpPressed(bool pressed) {
            if (pressed && MySM.DoubleJumpLeft) {
                MySM.Transition<DoubleJumping>();
            }
        }

        public override void FixedUpdate() {
            if (MySM.P.DiveDecelUpdate()) {
                MySM.Transition<Airborne>();
            }
        }

        public override void SetDivePressed(bool pressed) {
        }

        public override void SetGrounded(bool isGrounded) {
            if (isGrounded) MySM.Transition<Grounded>();
        }
    }
}